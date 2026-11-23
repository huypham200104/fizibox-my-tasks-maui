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
    private readonly ILiteCollection<Project> _projectsCollection;
    private readonly Stack<TaskItem> _undoStack = new();
    private readonly object _lock = new();

    public event EventHandler? TasksChanged;
    public event EventHandler? ProjectsChanged;

    public bool SimulateLoading { get; set; }
    public bool SimulateEmpty { get; set; }

    public LiteDbTaskService(string? customDbPath = null)
    {
        _dbPath = customDbPath ?? GetDefaultDatabasePath();
        _db = new LiteDatabase($"Filename={_dbPath};Connection=shared");
        _collection = _db.GetCollection<TaskItem>("tasks");
        _projectsCollection = _db.GetCollection<Project>("projects");

        // Đảm bảo chỉ mục (Indexes) để truy vấn nhanh
        _collection.EnsureIndex(x => x.DueDate);
        _collection.EnsureIndex(x => x.Status);
        _projectsCollection.EnsureIndex(x => x.Name);

        // Nạp các dự án mặc định nếu danh sách dự án trong database rỗng
        InitializeDefaultProjectsIfEmpty();

        // Không tự động nạp dữ liệu mẫu khi database rỗng; ứng dụng khởi động với danh sách trống (0 việc).
        // Khi cần kiểm thử dữ liệu mẫu, người dùng/reviewer có thể dùng nút "Đặt lại dữ liệu mẫu" trong màn hình Cài đặt.
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

        DateTime start = date.Date;
        DateTime end = start.AddDays(1);
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return _collection
                    .Find(t => t.DueDate >= start && t.DueDate < end)
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
                return _collection
                    .Find(t => t.Status == TaskStatus.Completed)
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
                _projectsCollection.DeleteAll();
                _undoStack.Clear();
                SimulateLoading = false;
                SimulateEmpty = false;
            }
        });

        Preferences.Default.Set("ProjectsSeeded", true);

        OnTasksChanged();
        OnProjectsChanged();
    }

    #region Project CRUD Operations

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        return await Task.Run(() =>
        {
            lock (_lock)
            {
                return _projectsCollection.FindAll().OrderBy(p => p.Name).ToList();
            }
        });
    }

    public async Task<bool> AddProjectAsync(Project project)
    {
        if (project == null || string.IsNullOrWhiteSpace(project.Name)) return false;

        await Task.Run(() =>
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(project.Id))
                {
                    project.Id = Guid.NewGuid().ToString("N");
                }
                _projectsCollection.Insert(project);
            }
        });

        OnProjectsChanged();
        return true;
    }

    public async Task<bool> UpdateProjectAsync(Project project)
    {
        if (project == null || string.IsNullOrWhiteSpace(project.Name)) return false;

        bool updated = await Task.Run(() =>
        {
            lock (_lock)
            {
                return _projectsCollection.Update(project);
            }
        });

        if (updated)
        {
            OnProjectsChanged();
        }
        return updated;
    }

    public async Task<bool> DeleteProjectAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId)) return false;

        bool deleted = await Task.Run(() =>
        {
            lock (_lock)
            {
                return _projectsCollection.Delete(projectId);
            }
        });

        if (deleted)
        {
            OnProjectsChanged();
        }
        return deleted;
    }

    private void InitializeDefaultProjectsIfEmpty()
    {
        lock (_lock)
        {
            if (Preferences.Default.Get("ProjectsSeeded", false))
            {
                return;
            }

            if (_projectsCollection.Count() == 0)
            {
                var defaults = new List<Project>
                {
                    new Project { Id = "proj_1", Name = "Công việc cá nhân", ColorHex = "#4F46E5" },
                    new Project { Id = "proj_2", Name = "Dự án Mobile App", ColorHex = "#0284C7" },
                    new Project { Id = "proj_3", Name = "Dự án Backend", ColorHex = "#16A34A" },
                    new Project { Id = "proj_4", Name = "MyTasks App", ColorHex = "#D97706" }
                };
                _projectsCollection.InsertBulk(defaults);
                Preferences.Default.Set("ProjectsSeeded", true);
            }
        }
    }

    private void OnProjectsChanged()
    {
        ProjectsChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

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
