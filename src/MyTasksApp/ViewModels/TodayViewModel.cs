using System.Collections.ObjectModel;
using System.Globalization;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.ViewModels.Base;
using MyTasksApp.Views;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.ViewModels;

/// <summary>
/// ViewModel quản lý dữ liệu và tương tác cho màn hình Hôm nay (TodayPage)
/// Tuân thủ MVVM thuần C#, Clean Architecture và các quy tắc nghiệp vụ trong BUSINESS.md
/// Hỗ trợ Optimistic UI + UX trượt hoàn thành 400ms mượt mà
/// </summary>
public class TodayViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly ILocalizationService? _localizationService;
    private readonly CultureInfo _viCulture = new("vi-VN");

    private string _currentDateText = string.Empty;
    private int _completedTasksCount;
    private int _totalTasksCount;
    private double _progressRatio;
    private string _progressPercentageText = "0%";
    private string _progressRatioText = "0/0 việc hoàn thành";
    private bool _isRefreshing;
    private bool _isInternalUpdate;
    private bool _isDataLoaded;

    // Quản lý Modal tạo việc nhanh (Bottom Sheet)
    private bool _isCreateModalVisible;
    private string _newTaskTitle = string.Empty;
    private string _newTaskDescription = string.Empty;
    private string _newTaskProject = "Công việc cá nhân";
    private DateTime _newTaskDueDate = DateTime.Today;
    private TimeSpan _newTaskDueTime = new(9, 0, 0);

    // Quản lý Snackbar / Toast nổi
    private bool _isSnackbarVisible;
    private string _snackbarMessage = string.Empty;
    private bool _canUndo;
    private CancellationTokenSource? _snackbarCts;

    public ObservableCollection<TaskItem> OverdueTasks { get; } = new();
    public ObservableCollection<TaskItem> TodayTasks { get; } = new();
    public ObservableCollection<TaskItem> CompletedTasks { get; } = new();

    public string CurrentDateText
    {
        get => _currentDateText;
        private set => SetProperty(ref _currentDateText, value);
    }

    public int CompletedTasksCount
    {
        get => _completedTasksCount;
        private set => SetProperty(ref _completedTasksCount, value);
    }

    public int TotalTasksCount
    {
        get => _totalTasksCount;
        private set => SetProperty(ref _totalTasksCount, value);
    }

    public double ProgressRatio
    {
        get => _progressRatio;
        private set => SetProperty(ref _progressRatio, value);
    }

    public string ProgressPercentageText
    {
        get => _progressPercentageText;
        private set => SetProperty(ref _progressPercentageText, value);
    }

    public string ProgressRatioText
    {
        get => _progressRatioText;
        private set => SetProperty(ref _progressRatioText, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool HasOverdueTasks => OverdueTasks.Count > 0;
    public bool HasTodayTasks => TodayTasks.Count > 0;
    public bool HasCompletedTasks => CompletedTasks.Count > 0;
    public bool IsEmpty => !IsBusy && !HasOverdueTasks && !HasTodayTasks && !HasCompletedTasks;

    // Properties cho Create Task Modal
    public bool IsCreateModalVisible
    {
        get => _isCreateModalVisible;
        set => SetProperty(ref _isCreateModalVisible, value);
    }

    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set => SetProperty(ref _newTaskTitle, value);
    }

    public string NewTaskDescription
    {
        get => _newTaskDescription;
        set => SetProperty(ref _newTaskDescription, value);
    }

    public string NewTaskProject
    {
        get => _newTaskProject;
        set => SetProperty(ref _newTaskProject, value);
    }

    public DateTime NewTaskDueDate
    {
        get => _newTaskDueDate;
        set => SetProperty(ref _newTaskDueDate, value);
    }

    public TimeSpan NewTaskDueTime
    {
        get => _newTaskDueTime;
        set => SetProperty(ref _newTaskDueTime, value);
    }

    // Properties cho Snackbar
    public bool IsSnackbarVisible
    {
        get => _isSnackbarVisible;
        set => SetProperty(ref _isSnackbarVisible, value);
    }

    public string SnackbarMessage
    {
        get => _snackbarMessage;
        set => SetProperty(ref _snackbarMessage, value);
    }

    public bool CanUndo
    {
        get => _canUndo;
        set => SetProperty(ref _canUndo, value);
    }

    // Commands
    public AsyncRelayCommand LoadTasksCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand ToggleTaskStatusCommand { get; }
    public AsyncRelayCommand DeleteTaskCommand { get; }
    public AsyncRelayCommand UndoDeleteCommand { get; }
    public AsyncRelayCommand OpenTaskDetailCommand { get; }
    public RelayCommand OpenCreateModalCommand { get; }
    public RelayCommand CloseCreateModalCommand { get; }
    public AsyncRelayCommand SaveNewTaskCommand { get; }
    public RelayCommand DismissSnackbarCommand { get; }

    public string EmptyMessage => _localizationService?["TodayEmptyMessage"] ?? "Hôm nay bạn không có việc cần làm!";
    public string EmptySubMessage => _localizationService?["TodayEmptySubMessage"] ?? "Hãy nghỉ ngơi hoặc lên kế hoạch cho những ngày tiếp theo.";
    public string SectionOverdueTitle => _localizationService?["SectionOverdue"] ?? "QUÁ HẠN";
    public string SectionTodayTitle => _localizationService?["SectionToday"] ?? "HÔM NAY";
    public string SectionCompletedTitle => _localizationService?["SectionCompleted"] ?? "XONG";
    public string ProgressTodayTitle => _localizationService?["ProgressTodayTitle"] ?? "Tiến độ hôm nay";
    public string AppSubtitle => _localizationService?["AppSubtitle"] ?? "Việc của tôi";

    public TodayViewModel(ITaskService taskService, ILocalizationService? localizationService = null)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _localizationService = localizationService;
        Title = "Hôm nay";

        UpdateDateText();

        LoadTasksCommand = new AsyncRelayCommand(ExecuteLoadTasksAsync);
        RefreshCommand = new AsyncRelayCommand(ExecuteRefreshAsync);
        ToggleTaskStatusCommand = new AsyncRelayCommand(ExecuteToggleTaskStatusAsync);
        DeleteTaskCommand = new AsyncRelayCommand(ExecuteDeleteTaskAsync);
        UndoDeleteCommand = new AsyncRelayCommand(ExecuteUndoDeleteAsync);
        OpenTaskDetailCommand = new AsyncRelayCommand(ExecuteOpenTaskDetailAsync);
        OpenCreateModalCommand = new RelayCommand(ExecuteOpenCreateModal);
        CloseCreateModalCommand = new RelayCommand(ExecuteCloseCreateModal);
        SaveNewTaskCommand = new AsyncRelayCommand(ExecuteSaveNewTaskAsync);
        DismissSnackbarCommand = new RelayCommand(ExecuteDismissSnackbar);

        // Lắng nghe sự kiện dữ liệu thay đổi để cập nhật realtime
        _taskService.TasksChanged += OnTasksChanged;

        if (_localizationService != null)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
        }
    }

    private void UpdateDateText()
    {
        DateTime now = DateTime.Now;
        if (_localizationService?.CurrentLanguage == AppLanguage.English)
        {
            CurrentDateText = now.ToString("dddd, MMM d", CultureInfo.InvariantCulture);
        }
        else
        {
            string dateRaw = now.ToString("dddd, dd 'tháng' MM", _viCulture);
            if (!string.IsNullOrEmpty(dateRaw))
            {
                CurrentDateText = char.ToUpper(dateRaw[0], _viCulture) + dateRaw[1..];
            }
            else
            {
                CurrentDateText = now.ToString("dd/MM/yyyy");
            }
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateDateText();
            UpdateProgressMetrics();
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptySubMessage));
            OnPropertyChanged(nameof(SectionOverdueTitle));
            OnPropertyChanged(nameof(SectionTodayTitle));
            OnPropertyChanged(nameof(SectionCompletedTitle));
            OnPropertyChanged(nameof(ProgressTodayTitle));
            OnPropertyChanged(nameof(AppSubtitle));
        });
    }

    public async Task EnsureDataLoadedAsync()
    {
        if (_isDataLoaded) return;
        await ExecuteLoadTasksAsync();
    }

    private async Task ExecuteLoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsEmpty));

            UpdateDateText();
            var tasks = await _taskService.GetTodayTasksAsync();
            OrganizeTasks(tasks);
            _isDataLoaded = true;
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    private async Task ExecuteRefreshAsync()
    {
        try
        {
            IsRefreshing = true;
            UpdateDateText();
            var tasks = await _taskService.GetTodayTasksAsync();
            OrganizeTasks(tasks);
        }
        finally
        {
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    private void OrganizeTasks(List<TaskItem> tasks)
    {
        DateTime today = DateTime.Today;

        OverdueTasks.Clear();
        TodayTasks.Clear();
        CompletedTasks.Clear();

        foreach (var task in tasks)
        {
            if (task.Status == TaskStatus.Completed)
            {
                CompletedTasks.Add(task);
            }
            else if (task.DueDate.Date < today)
            {
                OverdueTasks.Add(task);
            }
            else
            {
                TodayTasks.Add(task);
            }
        }

        UpdateProgressMetrics();
    }

    private void UpdateProgressMetrics()
    {
        TotalTasksCount = OverdueTasks.Count + TodayTasks.Count + CompletedTasks.Count;
        CompletedTasksCount = CompletedTasks.Count;
        string format = _localizationService?["ProgressCompletedFormat"] ?? "{0}/{1} việc hoàn thành";

        if (TotalTasksCount > 0)
        {
            ProgressRatio = (double)CompletedTasksCount / TotalTasksCount;
            int percentage = (int)Math.Round(ProgressRatio * 100);
            ProgressPercentageText = $"{percentage}%";
            ProgressRatioText = string.Format(format, CompletedTasksCount, TotalTasksCount);
        }
        else
        {
            ProgressRatio = 0;
            ProgressPercentageText = "0%";
            ProgressRatioText = string.Format(format, 0, 0);
        }

        OnPropertyChanged(nameof(HasOverdueTasks));
        OnPropertyChanged(nameof(HasTodayTasks));
        OnPropertyChanged(nameof(HasCompletedTasks));
        OnPropertyChanged(nameof(IsEmpty));
    }

    private async Task ExecuteToggleTaskStatusAsync(object? parameter)
    {
        if (parameter is not TaskItem task) return;

        bool willBeCompleted = task.Status != TaskStatus.Completed;

        if (willBeCompleted)
        {
            // Bước 1: Tick đổi màu xanh + gạch ngang chữ ngay tại chỗ
            task.Status = TaskStatus.Completed;
            task.CompletedAt = DateTime.Now;
            UpdateProgressMetrics();

            // Bước 2: Giữ 400ms để người dùng thấy hoàn thành
            await Task.Delay(400);

            // Bước 3: Chuyển xuống danh sách XONG
            OverdueTasks.Remove(task);
            TodayTasks.Remove(task);
            if (!CompletedTasks.Contains(task))
            {
                CompletedTasks.Insert(0, task);
            }
            UpdateProgressMetrics();

            ShowSnackbar("Đã hoàn thành công việc", canUndo: false);
        }
        else
        {
            // Mở lại công việc
            // Bước 1: Phản hồi thị giác tức thì - Đổi trạng thái và cập nhật thanh tiến độ
            task.Status = TaskStatus.Pending;
            task.CompletedAt = null;
            UpdateProgressMetrics();

            // Bước 2: Giữ 350ms phản hồi thị giác để người dùng thấy rõ tick được gỡ
            await Task.Delay(350);

            // Bước 3: Di chuyển công việc về danh sách Hôm nay hoặc Quá hạn
            CompletedTasks.Remove(task);

            DateTime today = DateTime.Today;
            if (task.DueDate.Date < today)
            {
                if (!OverdueTasks.Contains(task))
                    OverdueTasks.Add(task);
            }
            else
            {
                if (!TodayTasks.Contains(task))
                    TodayTasks.Insert(0, task);
            }
            UpdateProgressMetrics();

            ShowSnackbar("Đã mở lại công việc", canUndo: false);
        }

        // Đồng bộ ngầm xuống Service
        _isInternalUpdate = true;
        try
        {
            await _taskService.SetTaskStatusAsync(task.Id, task.Status);
        }
        finally
        {
            _isInternalUpdate = false;
        }
    }

    private async Task ExecuteDeleteTaskAsync(object? parameter)
    {
        if (parameter is not TaskItem task) return;

        OverdueTasks.Remove(task);
        TodayTasks.Remove(task);
        CompletedTasks.Remove(task);
        UpdateProgressMetrics();

        ShowSnackbar("Đã xóa công việc", canUndo: true);

        _isInternalUpdate = true;
        try
        {
            await _taskService.DeleteTaskAsync(task.Id);
        }
        finally
        {
            _isInternalUpdate = false;
        }
    }

    private async Task ExecuteUndoDeleteAsync()
    {
        var restored = await _taskService.UndoDeleteAsync();
        if (restored != null)
        {
            ShowSnackbar($"Đã hoàn tác: {restored.Title}", canUndo: false);
            var tasks = await _taskService.GetTodayTasksAsync();
            OrganizeTasks(tasks);
        }
    }

    private async Task ExecuteOpenTaskDetailAsync(object? parameter)
    {
        if (parameter is not TaskItem task) return;

        await Shell.Current.GoToAsync($"{nameof(TaskDetailPage)}?id={task.Id}");
    }

    private void ExecuteOpenCreateModal()
    {
        NewTaskTitle = string.Empty;
        NewTaskDescription = string.Empty;
        NewTaskProject = "Công việc cá nhân";
        NewTaskDueDate = DateTime.Today;
        NewTaskDueTime = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
        IsCreateModalVisible = true;
    }

    private void ExecuteCloseCreateModal()
    {
        IsCreateModalVisible = false;
    }

    private async Task ExecuteSaveNewTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            ShowSnackbar("Vui lòng nhập tên công việc", canUndo: false);
            return;
        }

        var newTask = new TaskItem
        {
            Title = NewTaskTitle.Trim(),
            Description = NewTaskDescription?.Trim() ?? string.Empty,
            ProjectName = string.IsNullOrWhiteSpace(NewTaskProject) ? "Công việc cá nhân" : NewTaskProject.Trim(),
            DueDate = NewTaskDueDate.Date,
            DueTime = NewTaskDueTime,
            Status = TaskStatus.Pending
        };

        IsCreateModalVisible = false;

        bool success = await _taskService.AddTaskAsync(newTask);
        if (success)
        {
            ShowSnackbar("Đã tạo công việc mới thành công", canUndo: false);
            var tasks = await _taskService.GetTodayTasksAsync();
            OrganizeTasks(tasks);
        }
    }

    private void ShowSnackbar(string message, bool canUndo)
    {
        _snackbarCts?.Cancel();
        _snackbarCts = new CancellationTokenSource();

        SnackbarMessage = message;
        CanUndo = canUndo;
        IsSnackbarVisible = true;

        var token = _snackbarCts.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(4000, token);
                if (!token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (!token.IsCancellationRequested)
                        {
                            IsSnackbarVisible = false;
                        }
                    });
                }
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private void ExecuteDismissSnackbar()
    {
        _snackbarCts?.Cancel();
        IsSnackbarVisible = false;
    }

    private void OnTasksChanged(object? sender, EventArgs e)
    {
        if (_isInternalUpdate) return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var tasks = await _taskService.GetTodayTasksAsync();
            OrganizeTasks(tasks);
        });
    }
}
