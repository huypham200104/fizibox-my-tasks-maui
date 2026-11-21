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
/// ViewModel quản lý dữ liệu và tương tác cho màn hình Tuần này (WeekPage)
/// Tuân thủ Clean Architecture, MVVM thuần C#, và các quy chuẩn trong BUSINESS.md và DESIGN.md
/// </summary>
public class WeekViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly ILocalizationService? _localizationService;
    private readonly CultureInfo _viCulture = new("vi-VN");

    private DateTime _currentWeekStart;
    private DateTime _selectedDate;
    private string _weekRangeText = string.Empty;
    private string _selectedDayTitle = string.Empty;

    private int _dayCompletedTasksCount;
    private int _dayTotalTasksCount;
    private double _dayProgressRatio;
    private string _dayProgressPercentageText = "0%";
    private string _dayProgressRatioText = "0/0 hoàn thành";
    private bool _isRefreshing;
    private bool _isInternalUpdate;
    private bool _isDataLoaded;

    // Modal tạo việc nhanh
    private bool _isCreateModalVisible;
    private string _newTaskTitle = string.Empty;
    private string _newTaskDescription = string.Empty;
    private string _newTaskProject = "Công việc cá nhân";
    private DateTime _newTaskDueDate = DateTime.Today;
    private TimeSpan _newTaskDueTime = new(9, 0, 0);

    // Floating Snackbar
    private bool _isSnackbarVisible;
    private string _snackbarMessage = string.Empty;
    private bool _canUndo;
    private CancellationTokenSource? _snackbarCts;

    public ObservableCollection<WeekDayItem> WeekDays { get; } = new();
    public ObservableCollection<TaskItem> DayTasks { get; } = new();

    public DateTime CurrentWeekStart
    {
        get => _currentWeekStart;
        private set => SetProperty(ref _currentWeekStart, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                UpdateSelectedDayTitle();
            }
        }
    }

    public string WeekRangeText
    {
        get => _weekRangeText;
        private set => SetProperty(ref _weekRangeText, value);
    }

    public string SelectedDayTitle
    {
        get => _selectedDayTitle;
        private set => SetProperty(ref _selectedDayTitle, value);
    }

    public int DayCompletedTasksCount
    {
        get => _dayCompletedTasksCount;
        private set => SetProperty(ref _dayCompletedTasksCount, value);
    }

    public int DayTotalTasksCount
    {
        get => _dayTotalTasksCount;
        private set => SetProperty(ref _dayTotalTasksCount, value);
    }

    public double DayProgressRatio
    {
        get => _dayProgressRatio;
        private set => SetProperty(ref _dayProgressRatio, value);
    }

    public string DayProgressPercentageText
    {
        get => _dayProgressPercentageText;
        private set => SetProperty(ref _dayProgressPercentageText, value);
    }

    public string DayProgressRatioText
    {
        get => _dayProgressRatioText;
        private set => SetProperty(ref _dayProgressRatioText, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool IsEmpty => !IsBusy && DayTasks.Count == 0;

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
    public AsyncRelayCommand LoadWeekDataCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand PreviousWeekCommand { get; }
    public AsyncRelayCommand NextWeekCommand { get; }
    public AsyncRelayCommand SelectDayCommand { get; }
    public AsyncRelayCommand ToggleTaskStatusCommand { get; }
    public AsyncRelayCommand DeleteTaskCommand { get; }
    public AsyncRelayCommand UndoDeleteCommand { get; }
    public AsyncRelayCommand OpenTaskDetailCommand { get; }
    public RelayCommand OpenCreateModalCommand { get; }
    public RelayCommand CloseCreateModalCommand { get; }
    public AsyncRelayCommand SaveNewTaskCommand { get; }
    public RelayCommand DismissSnackbarCommand { get; }

    public string EmptyMessage => _localizationService?["WeekEmptyMessage"] ?? "Không có công việc";
    public string EmptySubMessage => _localizationService?["WeekEmptySubMessage"] ?? "Ngày này chưa có công việc nào được lên lịch.";
    public string AppSubtitle => _localizationService?["AppSubtitle"] ?? "Việc của tôi";

    public WeekViewModel(ITaskService taskService, ILocalizationService? localizationService = null)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _localizationService = localizationService;
        Title = "Tuần này";

        // Khởi tạo tuần chứa ngày hôm nay
        DateTime today = DateTime.Today;
        _currentWeekStart = GetMondayOfWeek(today);
        _selectedDate = today;

        UpdateWeekRangeText();
        UpdateSelectedDayTitle();

        // Khởi tạo sẵn 7 ngày ngay từ constructor để UI binding lập tức, không bị trễ hay trống
        string[] dayNames = GetDayNames();
        for (int i = 0; i < 7; i++)
        {
            DateTime dayDate = _currentWeekStart.AddDays(i);
            WeekDays.Add(new WeekDayItem
            {
                Date = dayDate,
                DayOfWeekShort = dayNames[i],
                DayNumber = dayDate.Day.ToString(),
                TaskCount = 0,
                IsToday = dayDate.Date == today,
                IsSelected = dayDate.Date == _selectedDate.Date
            });
        }

        LoadWeekDataCommand = new AsyncRelayCommand(ExecuteLoadWeekDataAsync);
        RefreshCommand = new AsyncRelayCommand(ExecuteRefreshAsync);
        PreviousWeekCommand = new AsyncRelayCommand(ExecutePreviousWeekAsync);
        NextWeekCommand = new AsyncRelayCommand(ExecuteNextWeekAsync);
        SelectDayCommand = new AsyncRelayCommand(ExecuteSelectDayAsync);
        ToggleTaskStatusCommand = new AsyncRelayCommand(ExecuteToggleTaskStatusAsync);
        DeleteTaskCommand = new AsyncRelayCommand(ExecuteDeleteTaskAsync);
        UndoDeleteCommand = new AsyncRelayCommand(ExecuteUndoDeleteAsync);
        OpenTaskDetailCommand = new AsyncRelayCommand(ExecuteOpenTaskDetailAsync);
        OpenCreateModalCommand = new RelayCommand(ExecuteOpenCreateModal);
        CloseCreateModalCommand = new RelayCommand(ExecuteCloseCreateModal);
        SaveNewTaskCommand = new AsyncRelayCommand(ExecuteSaveNewTaskAsync);
        DismissSnackbarCommand = new RelayCommand(ExecuteDismissSnackbar);

        _taskService.TasksChanged += OnTasksChanged;

        if (_localizationService != null)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
        }
    }

    private string[] GetDayNames()
    {
        if (_localizationService?.CurrentLanguage == AppLanguage.English)
        {
            return new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        }
        return new[] { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateWeekRangeText();
            UpdateSelectedDayTitle();
            string[] dayNames = GetDayNames();
            for (int i = 0; i < WeekDays.Count && i < dayNames.Length; i++)
            {
                WeekDays[i].DayOfWeekShort = dayNames[i];
            }
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptySubMessage));
            OnPropertyChanged(nameof(AppSubtitle));
        });
    }

    public async Task EnsureDataLoadedAsync()
    {
        if (_isDataLoaded) return;
        await ExecuteLoadWeekDataAsync();
    }

    public static DateTime GetMondayOfWeek(DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-1 * diff);
    }

    private void UpdateWeekRangeText()
    {
        DateTime endOfWeek = _currentWeekStart.AddDays(6);
        if (_localizationService?.CurrentLanguage == AppLanguage.English)
        {
            if (_currentWeekStart.Month == endOfWeek.Month)
            {
                WeekRangeText = $"{_currentWeekStart.ToString("MMM d", CultureInfo.InvariantCulture)} – {endOfWeek.Day}, {_currentWeekStart.Year}";
            }
            else
            {
                WeekRangeText = $"{_currentWeekStart.ToString("MMM d", CultureInfo.InvariantCulture)} – {endOfWeek.ToString("MMM d", CultureInfo.InvariantCulture)}, {endOfWeek.Year}";
            }
        }
        else
        {
            if (_currentWeekStart.Month == endOfWeek.Month)
            {
                WeekRangeText = $"{_currentWeekStart.Day} – {endOfWeek.Day} Tháng {_currentWeekStart.Month}, {_currentWeekStart.Year}";
            }
            else if (_currentWeekStart.Year == endOfWeek.Year)
            {
                WeekRangeText = $"{_currentWeekStart.Day} Thg {_currentWeekStart.Month} – {endOfWeek.Day} Thg {endOfWeek.Month}, {endOfWeek.Year}";
            }
            else
            {
                WeekRangeText = $"{_currentWeekStart:dd/MM/yyyy} – {endOfWeek:dd/MM/yyyy}";
            }
        }
    }

    private void UpdateSelectedDayTitle()
    {
        if (_localizationService?.CurrentLanguage == AppLanguage.English)
        {
            SelectedDayTitle = _selectedDate.ToString("dddd, MMM d", CultureInfo.InvariantCulture);
        }
        else
        {
            string raw = _selectedDate.ToString("dddd, dd 'tháng' MM", _viCulture);
            if (!string.IsNullOrEmpty(raw))
            {
                SelectedDayTitle = char.ToUpper(raw[0], _viCulture) + raw[1..];
            }
            else
            {
                SelectedDayTitle = _selectedDate.ToString("dd/MM/yyyy");
            }
        }
    }

    private async Task ExecuteLoadWeekDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsEmpty));

            await RefreshWeekDaysAndTasksAsync();
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
            await RefreshWeekDaysAndTasksAsync();
        }
        finally
        {
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    private async Task RefreshWeekDaysAndTasksAsync()
    {
        UpdateWeekRangeText();
        UpdateSelectedDayTitle();

        DateTime today = DateTime.Today;
        string[] dayNames = GetDayNames();

        bool needInit = WeekDays.Count != 7;
        var initialList = needInit ? new List<WeekDayItem>() : null;

        for (int i = 0; i < 7; i++)
        {
            DateTime dayDate = _currentWeekStart.AddDays(i);
            var tasksForDay = await _taskService.GetTasksByDateAsync(dayDate);

            if (needInit)
            {
                initialList!.Add(new WeekDayItem
                {
                    Date = dayDate,
                    DayOfWeekShort = dayNames[i],
                    DayNumber = dayDate.Day.ToString(),
                    TaskCount = tasksForDay.Count,
                    IsToday = dayDate.Date == today,
                    IsSelected = dayDate.Date == _selectedDate.Date
                });
            }
            else
            {
                var day = WeekDays[i];
                day.Date = dayDate;
                day.DayOfWeekShort = dayNames[i];
                day.DayNumber = dayDate.Day.ToString();
                day.TaskCount = tasksForDay.Count;
                day.IsToday = dayDate.Date == today;
                day.IsSelected = dayDate.Date == _selectedDate.Date;
            }
        }

        if (needInit && initialList != null)
        {
            WeekDays.Clear();
            foreach (var item in initialList)
            {
                WeekDays.Add(item);
            }
        }

        // Tải danh sách công việc của ngày đang chọn
        await LoadSelectedDayTasksAsync();
    }

    private async Task LoadSelectedDayTasksAsync()
    {
        var tasks = await _taskService.GetTasksByDateAsync(_selectedDate);

        DayTasks.Clear();
        // Sắp xếp theo thứ tự mốc giờ DueTime tăng dần
        foreach (var task in tasks.OrderBy(t => t.DueTime))
        {
            DayTasks.Add(task);
        }

        UpdateDayProgressMetrics();
    }

    private void UpdateDayProgressMetrics()
    {
        DayTotalTasksCount = DayTasks.Count;
        DayCompletedTasksCount = DayTasks.Count(t => t.Status == TaskStatus.Completed);

        if (DayTotalTasksCount > 0)
        {
            DayProgressRatio = (double)DayCompletedTasksCount / DayTotalTasksCount;
            int percentage = (int)Math.Round(DayProgressRatio * 100);
            DayProgressPercentageText = $"{percentage}%";
            DayProgressRatioText = $"{DayTotalTasksCount} công việc • {DayCompletedTasksCount}/{DayTotalTasksCount} hoàn thành";
        }
        else
        {
            DayProgressRatio = 0.0;
            DayProgressPercentageText = "0%";
            DayProgressRatioText = "0 công việc • 0/0 hoàn thành";
        }

        OnPropertyChanged(nameof(IsEmpty));
    }

    private async Task ExecutePreviousWeekAsync()
    {
        // Lùi 7 ngày
        CurrentWeekStart = CurrentWeekStart.AddDays(-7);
        // Bảo toàn thứ tương ứng trong tuần
        SelectedDate = SelectedDate.AddDays(-7);
        await RefreshWeekDaysAndTasksAsync();
    }

    private async Task ExecuteNextWeekAsync()
    {
        // Tiến 7 ngày
        CurrentWeekStart = CurrentWeekStart.AddDays(7);
        // Bảo toàn thứ tương ứng trong tuần
        SelectedDate = SelectedDate.AddDays(7);
        await RefreshWeekDaysAndTasksAsync();
    }

    private async Task ExecuteSelectDayAsync(object? parameter)
    {
        DateTime targetDate;
        if (parameter is WeekDayItem item)
        {
            targetDate = item.Date;
        }
        else if (parameter is DateTime dt)
        {
            targetDate = dt;
        }
        else
        {
            return;
        }

        if (_selectedDate.Date == targetDate.Date) return;

        SelectedDate = targetDate;

        foreach (var day in WeekDays)
        {
            day.IsSelected = day.Date.Date == targetDate.Date;
        }

        await LoadSelectedDayTasksAsync();
    }

    private async Task ExecuteToggleTaskStatusAsync(object? parameter)
    {
        if (parameter is not TaskItem task) return;

        bool willBeCompleted = task.Status != TaskStatus.Completed;

        // Optimistic UI: Đổi trạng thái tại chỗ
        task.Status = willBeCompleted ? TaskStatus.Completed : TaskStatus.Pending;
        task.CompletedAt = willBeCompleted ? DateTime.Now : null;

        UpdateDayProgressMetrics();

        ShowSnackbar(willBeCompleted ? "Đã hoàn thành công việc" : "Đã mở lại công việc", canUndo: false);

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

        DayTasks.Remove(task);
        UpdateDayProgressMetrics();

        // Cập nhật lại số lượng badge của ngày
        var currentDay = WeekDays.FirstOrDefault(d => d.Date.Date == task.DueDate.Date);
        if (currentDay != null && currentDay.TaskCount > 0)
        {
            currentDay.TaskCount--;
        }

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
            await RefreshWeekDaysAndTasksAsync();
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
        // Mặc định ngày tạo việc là ngày đang xem
        NewTaskDueDate = SelectedDate;
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
            // Cập nhật lại tuần để badge của ngày mới tạo tăng lên
            await RefreshWeekDaysAndTasksAsync();
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
                    RunOnMainThread(() =>
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

        RunOnMainThread(async () =>
        {
            await RefreshWeekDaysAndTasksAsync();
        });
    }

    private static void RunOnMainThread(Action action)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
        catch
        {
            // Môi trường Unit Test không có UI Dispatcher, thực thi trực tiếp
            action();
        }
    }
}
