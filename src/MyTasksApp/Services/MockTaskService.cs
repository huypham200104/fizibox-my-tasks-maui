using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Services;

/// <summary>
/// Dịch vụ quản lý dữ liệu In-Memory nạp sẵn bộ Mock Data thực tế 16–22/11/2026
/// Hỗ trợ cờ giả lập Loading, Empty, đồng bộ state realtime và ngăn xếp Undo.
/// </summary>
public class MockTaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = new();
    private readonly List<Project> _projects = new();
    private readonly Stack<TaskItem> _undoStack = new();
    private int _nextId = 1;

    public event EventHandler? TasksChanged;
    public event EventHandler? ProjectsChanged;

    public bool SimulateLoading { get; set; }
    public bool SimulateEmpty { get; set; }

    public MockTaskService()
    {
        InitializeDefaultMockData();
        InitializeDefaultProjects();
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks.Select(CloneTask).ToList();
        }
    }

    public async Task<List<TaskItem>> GetTodayTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        DateTime today = DateTime.Today;
        lock (_tasks)
        {
            // Điểm tự quyết số 1:
            // 1. Quá hạn: Hạn trước hôm nay & Chưa hoàn thành
            // 2. Hôm nay: Hạn là hôm nay & Chưa hoàn thành
            // 3. Xong: Có hạn hôm nay đã hoàn thành HOẶC vừa hoàn thành trong ngày hôm nay
            return _tasks
                .Where(t =>
                    (t.DueDate.Date < today && t.Status != TaskStatus.Completed) ||
                    (t.DueDate.Date == today && t.Status != TaskStatus.Completed) ||
                    (t.Status == TaskStatus.Completed && (t.DueDate.Date == today || (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today))))
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.DueTime)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<List<TaskItem>> GetTasksByDateAsync(DateTime date)
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks
                .Where(t => t.DueDate.Date == date.Date)
                .OrderBy(t => t.Status == TaskStatus.Completed)
                .ThenBy(t => t.DueTime)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<List<TaskItem>> GetCompletedTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks
                .Where(t => t.Status == TaskStatus.Completed)
                .OrderByDescending(t => t.CompletedAt ?? t.DueDate)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return task != null ? CloneTask(task) : null;
        }
    }

    public async Task<bool> AddTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            task.Id = _nextId++;
            _tasks.Add(CloneTask(task));
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing == null) return false;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.ProjectName = task.ProjectName;
            existing.DueDate = task.DueDate;
            existing.DueTime = task.DueTime;
            existing.Status = task.Status;
            existing.Note = task.Note;
            existing.CompletedAt = task.CompletedAt;
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> ToggleTaskStatusAsync(int taskId)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
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
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> SetTaskStatusAsync(int taskId, TaskStatus status)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            task.Status = status;
            task.CompletedAt = status == TaskStatus.Completed ? DateTime.Now : null;
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            _tasks.Remove(task);
            _undoStack.Push(task);
        }

        OnTasksChanged();
        return true;
    }

    public async Task<TaskItem?> UndoDeleteAsync()
    {
        await DelaySimulateAsync();
        TaskItem? restoredTask = null;

        lock (_tasks)
        {
            if (_undoStack.Count > 0)
            {
                restoredTask = _undoStack.Pop();
                _tasks.Add(restoredTask);
            }
        }

        if (restoredTask != null)
        {
            OnTasksChanged();
        }

        return restoredTask;
    }

    public Task ResetMockDataAsync()
    {
        lock (_tasks)
        {
            _tasks.Clear();
            _projects.Clear();
            _undoStack.Clear();
            SimulateLoading = false;
            SimulateEmpty = false;
        }

        OnTasksChanged();
        OnProjectsChanged();
        return Task.CompletedTask;
    }

    #region Project CRUD Operations

    public Task<List<Project>> GetAllProjectsAsync()
    {
        lock (_projects)
        {
            return Task.FromResult(_projects.Select(p => new Project { Id = p.Id, Name = p.Name, ColorHex = p.ColorHex }).ToList());
        }
    }

    public Task<bool> AddProjectAsync(Project project)
    {
        if (project == null || string.IsNullOrWhiteSpace(project.Name)) return Task.FromResult(false);

        lock (_projects)
        {
            if (string.IsNullOrEmpty(project.Id))
            {
                project.Id = Guid.NewGuid().ToString("N");
            }
            _projects.Add(new Project { Id = project.Id, Name = project.Name, ColorHex = project.ColorHex });
        }

        OnProjectsChanged();
        return Task.FromResult(true);
    }

    public Task<bool> UpdateProjectAsync(Project project)
    {
        if (project == null || string.IsNullOrWhiteSpace(project.Name)) return Task.FromResult(false);

        bool updated = false;
        lock (_projects)
        {
            var existing = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existing != null)
            {
                existing.Name = project.Name;
                existing.ColorHex = project.ColorHex;
                updated = true;
            }
        }

        if (updated)
        {
            OnProjectsChanged();
        }
        return Task.FromResult(updated);
    }

    public Task<bool> DeleteProjectAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId)) return Task.FromResult(false);

        bool deleted = false;
        lock (_projects)
        {
            var existing = _projects.FirstOrDefault(p => p.Id == projectId);
            if (existing != null)
            {
                _projects.Remove(existing);
                deleted = true;
            }
        }

        if (deleted)
        {
            OnProjectsChanged();
        }
        return Task.FromResult(deleted);
    }

    private void InitializeDefaultProjects()
    {
        _projects.Clear();
        _projects.AddRange(new List<Project>
        {
            new Project { Id = "proj_1", Name = "Công việc cá nhân", ColorHex = "#4F46E5" },
            new Project { Id = "proj_2", Name = "Dự án Mobile App", ColorHex = "#0284C7" },
            new Project { Id = "proj_3", Name = "Dự án Backend", ColorHex = "#16A34A" },
            new Project { Id = "proj_4", Name = "MyTasks App", ColorHex = "#D97706" }
        });
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

    private void InitializeDefaultMockData()
    {
        _tasks.Clear();
        var defaultTasks = TaskDataSeeder.GenerateDefaultTasks();
        _tasks.AddRange(defaultTasks);
        _nextId = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
    }
}
