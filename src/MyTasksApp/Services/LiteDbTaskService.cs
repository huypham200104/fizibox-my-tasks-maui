using LiteDB;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Services;

/// <summary>
/// Dịch vụ lưu trữ dữ liệu NoSQL cục bộ vĩnh viễn sử dụng LiteDB
/// Khởi tạo database rỗng theo lựa chọn của người dùng, tự động đồng bộ realtime qua TasksChanged
/// </summary>
public class LiteDbTaskService : ITaskService, IDisposable
{
    private readonly string _dbPath;
    private readonly LiteDatabase _db;
    private readonly ILiteCollection<TaskItem> _collection;
    private readonly Stack<TaskItem> _undoStack = new();
    private readonly object _lock = new();

    public event EventHandler? TasksChanged;

    public bool SimulateLoading { get; set; }
    public bool SimulateEmpty { get; set; }

    public LiteDbTaskService(string? customDbPath = null)
    {
        _dbPath = customDbPath ?? GetDefaultDatabasePath();
        _db = new LiteDatabase($"Filename={_dbPath};Connection=shared");
        _collection = _db.GetCollection<TaskItem>("tasks");

        // Đảm bảo chỉ mục (Indexes) để truy vấn nhanh
        _collection.EnsureIndex(x => x.DueDate);
        _collection.EnsureIndex(x => x.Status);

        // Tự động seed dữ liệu mẫu 19/11 - 28/11 nếu database đang rỗng
        lock (_lock)
        {
            if (_collection.Count() == 0)
            {
                var seedTasks = TaskDataSeeder.GenerateDefaultTasks();
                _collection.InsertBulk(seedTasks);
            }
        }
    }

    private static string GetDefaultDatabasePath()
    {
        try
        {
            var appData = FileSystem.AppDataDirectory;
            if (!string.IsNullOrEmpty(appData))
            {
                return Path.Combine(appData, "mytasks.db");
            }
        }
        catch
        {
            // Dự phòng cho môi trường Unit Tests hoặc không có FileSystem context
        }

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mytasks.db");
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return _collection.FindAll().Select(CloneTask).ToList();
            }
        });
    }

    public async Task<List<TaskItem>> GetTodayTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        DateTime today = DateTime.Today;
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var all = _collection.FindAll().ToList();
                return all
                    .Where(t =>
                        (t.DueDate.Date < today && t.Status != TaskStatus.Completed) ||
                        (t.DueDate.Date == today && t.Status != TaskStatus.Completed) ||
                        (t.Status == TaskStatus.Completed && (t.DueDate.Date == today || (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today))))
                    .OrderBy(t => t.DueDate)
                    .ThenBy(t => t.DueTime)
                    .Select(CloneTask)
                    .ToList();
            }
        });
    }

    public async Task<List<TaskItem>> GetTasksByDateAsync(DateTime date)
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var all = _collection.FindAll().ToList();
                return all
                    .Where(t => t.DueDate.Date == date.Date)
                    .OrderBy(t => t.Status == TaskStatus.Completed)
                    .ThenBy(t => t.DueTime)
                    .Select(CloneTask)
                    .ToList();
            }
        });
    }

    public async Task<List<TaskItem>> GetCompletedTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var all = _collection.FindAll().ToList();
                return all
                    .Where(t => t.Status == TaskStatus.Completed)
                    .OrderByDescending(t => t.CompletedAt ?? t.DueDate)
                    .Select(CloneTask)
                    .ToList();
            }
        });
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        await DelaySimulateAsync();
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                var item = _collection.FindById(id);
                return item != null ? CloneTask(item) : null;
            }
        });
    }

    public async Task<bool> AddTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        await Task.Run(() =>
        {
            lock (_lock)
            {
                var clone = CloneTask(task);
                _collection.Insert(clone);
                task.Id = clone.Id;
            }
        });

        OnTasksChanged();
        return true;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        bool success = await Task.Run(() =>
        {
            lock (_lock)
            {
                var existing = _collection.FindById(task.Id);
                if (existing == null) return false;

                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.ProjectName = task.ProjectName;
                existing.DueDate = task.DueDate;
                existing.DueTime = task.DueTime;
                existing.Status = task.Status;
                existing.Note = task.Note;
                existing.CompletedAt = task.CompletedAt;

                return _collection.Update(existing);
            }
        });

        if (success)
        {
            OnTasksChanged();
        }
        return success;
    }

    public async Task<bool> ToggleTaskStatusAsync(int taskId)
    {
        await DelaySimulateAsync();
        bool success = await Task.Run(() =>
        {
            lock (_lock)
            {
                var task = _collection.FindById(taskId);
                if (task == null) return false;

                if (task.Status == TaskStatus.Completed)
                {
                    task.Status = TaskStatus.Pending;
                    task.CompletedAt = null;
                }
                else
                {
                    task.Status = TaskStatus.Completed;
                    task.CompletedAt = DateTime.Now;
                }

                return _collection.Update(task);
            }
        });

        if (success)
        {
            OnTasksChanged();
        }
        return success;
    }

    public async Task<bool> SetTaskStatusAsync(int taskId, TaskStatus status)
    {
        await DelaySimulateAsync();
        bool success = await Task.Run(() =>
        {
            lock (_lock)
            {
                var task = _collection.FindById(taskId);
                if (task == null) return false;

                task.Status = status;
                task.CompletedAt = status == TaskStatus.Completed ? DateTime.Now : null;

                return _collection.Update(task);
            }
        });

        if (success)
        {
            OnTasksChanged();
        }
        return success;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        await DelaySimulateAsync();
        bool success = await Task.Run(() =>
        {
            lock (_lock)
            {
                var task = _collection.FindById(taskId);
                if (task == null) return false;

                _undoStack.Push(CloneTask(task));
                return _collection.Delete(taskId);
            }
        });

        if (success)
        {
            OnTasksChanged();
        }
        return success;
    }

    public async Task<TaskItem?> UndoDeleteAsync()
    {
        await DelaySimulateAsync();
        TaskItem? restored = null;

        await Task.Run(() =>
        {
            lock (_lock)
            {
                if (_undoStack.Count > 0)
                {
                    restored = _undoStack.Pop();
                    _collection.Insert(restored);
                }
            }
        });

        if (restored != null)
        {
            OnTasksChanged();
        }
        return restored;
    }

    public async Task ResetMockDataAsync()
    {
        await Task.Run(() =>
        {
            lock (_lock)
            {
                _collection.DeleteAll();
                _undoStack.Clear();
                SimulateLoading = false;
                SimulateEmpty = false;

                var seedTasks = TaskDataSeeder.GenerateDefaultTasks();
                _collection.InsertBulk(seedTasks);
            }
        });

        OnTasksChanged();
    }

    private void OnTasksChanged()
    {
        TasksChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task DelaySimulateAsync()
    {
        if (SimulateLoading)
        {
            await Task.Delay(1500);
        }
        else
        {
            await Task.CompletedTask;
        }
    }

    private static TaskItem CloneTask(TaskItem source)
    {
        return new TaskItem
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            ProjectName = source.ProjectName,
            DueDate = source.DueDate,
            DueTime = source.DueTime,
            Status = source.Status,
            Note = source.Note,
            CompletedAt = source.CompletedAt
        };
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
