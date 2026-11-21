using System.Collections.ObjectModel;
using System.Globalization;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
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
    private readonly ISnackbarService _snackbarService;
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
    private bool _isDataDirty;
    public bool IsPageVisible { get; set; }

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
    private CancellationTokenSource? _loadCts;

    /// <summary>
    /// Thời gian trễ tối thiểu để hiển thị Skeleton chuyển cảnh mượt mà (mặc định 250ms).
    /// </summary>
    public int TransitionDelayMs { get; set; } = 250;

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

    public string ModalCreateTitle => _localizationService?["ModalCreateTitle"] ?? "Tạo công việc mới";
    public string ModalCreateSubtitle => _localizationService?["ModalCreateSubtitle"] ?? "Nhập thông tin công việc cần hoàn thành hôm nay";
    public string FieldTaskTitle => _localizationService?["FieldTaskTitle"] ?? "Tên công việc";
    public string FieldTaskTitlePlaceholder => _localizationService?["FieldTaskTitlePlaceholder"] ?? "Ví dụ: Thiết kế giao diện Home";
    public string FieldTaskProject => _localizationService?["FieldTaskProject"] ?? "Dự án";
    public string FieldTaskProjectPlaceholder => _localizationService?["FieldTaskProjectPlaceholder"] ?? "Tên dự án (Ví dụ: MyTasks App)";
    public string FieldTaskDesc => _localizationService?["FieldTaskDesc"] ?? "Mô tả chi tiết";
    public string FieldTaskDescPlaceholder => _localizationService?["FieldTaskDescPlaceholder"] ?? "Nhập ghi chú thêm cho công việc...";
    public string BtnCancel => _localizationService?["BtnCancel"] ?? "Hủy";
    public string BtnSaveTask => _localizationService?["BtnSaveTask"] ?? "Lưu công việc";
    public string DueDateLabel => _localizationService?["DetailDueDate"] ?? "Hạn ngày";
    public string DueTimeLabel => _localizationService?["DetailDueTime"] ?? "Hạn giờ";

    public WeekViewModel(ITaskService taskService, ILocalizationService? localizationService = null, ISnackbarService? snackbarService = null)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _localizationService = localizationService;
        _snackbarService = snackbarService ?? new SnackbarService();
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
            OnPropertyChanged(nameof(ModalCreateTitle));
            OnPropertyChanged(nameof(ModalCreateSubtitle));
            OnPropertyChanged(nameof(FieldTaskTitle));
            OnPropertyChanged(nameof(FieldTaskTitlePlaceholder));
            OnPropertyChanged(nameof(FieldTaskProject));
            OnPropertyChanged(nameof(FieldTaskProjectPlaceholder));
            OnPropertyChanged(nameof(FieldTaskDesc));
            OnPropertyChanged(nameof(FieldTaskDescPlaceholder));
            OnPropertyChanged(nameof(BtnCancel));
            OnPropertyChanged(nameof(BtnSaveTask));
            OnPropertyChanged(nameof(DueDateLabel));
            OnPropertyChanged(nameof(DueTimeLabel));
        });
    }

    public async Task EnsureDataLoadedAsync()
    {
        if (_isDataLoaded)
        {
            if (_isDataDirty)
            {
                _isDataDirty = false;
                await RefreshWeekDaysAndTasksAsync();
            }
            return;
        }
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

            if (TransitionDelayMs > 0)
            {
                await Task.Delay(TransitionDelayMs);
            }

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

        // Tối ưu hóa: Truy vấn toàn bộ công việc 1 lần duy nhất thay vì đọc đĩa tuần tự 8 lần
        var allTasks = await _taskService.GetAllTasksAsync();

        for (int i = 0; i < 7; i++)
        {
            DateTime dayDate = _currentWeekStart.AddDays(i);
            int tasksForDayCount = allTasks.Count(t => t.DueDate.Date == dayDate.Date);

            if (needInit)
            {
                initialList!.Add(new WeekDayItem
                {
                    Date = dayDate,
                    DayOfWeekShort = dayNames[i],
                    DayNumber = dayDate.Day.ToString(),
                    TaskCount = tasksForDayCount,
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
                day.TaskCount = tasksForDayCount;
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

        // Cập nhật danh sách công việc của ngày đang chọn trực tiếp từ danh sách đã lấy
        var selectedDayTasks = allTasks
            .Where(t => t.DueDate.Date == _selectedDate.Date)
            .OrderBy(t => t.Status == TaskStatus.Completed)
            .ThenBy(t => t.DueTime)
            .ToList();

        DayTasks.Clear();
        foreach (var task in selectedDayTasks)
        {
            DayTasks.Add(task);
        }

        UpdateDayProgressMetrics();
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
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        try
        {
            IsBusy = true;
            DayTasks.Clear();

            // Lùi 7 ngày
            CurrentWeekStart = CurrentWeekStart.AddDays(-7);
            // Bảo toàn thứ tương ứng trong tuần
            SelectedDate = SelectedDate.AddDays(-7);

            if (TransitionDelayMs > 0)
            {
                await Task.Delay(TransitionDelayMs, token);
            }

            await RefreshWeekDaysAndTasksAsync();
        }
        catch (OperationCanceledException)
        {
            // Bỏ qua nếu bị hủy bởi thao tác chuyển tuần/ngày tiếp theo
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
    }

    private async Task ExecuteNextWeekAsync()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        try
        {
            IsBusy = true;
            DayTasks.Clear();

            // Tiến 7 ngày
            CurrentWeekStart = CurrentWeekStart.AddDays(7);
            // Bảo toàn thứ tương ứng trong tuần
            SelectedDate = SelectedDate.AddDays(7);

            if (TransitionDelayMs > 0)
            {
                await Task.Delay(TransitionDelayMs, token);
            }

            await RefreshWeekDaysAndTasksAsync();
        }
        catch (OperationCanceledException)
        {
            // Bỏ qua nếu bị hủy bởi thao tác chuyển tuần/ngày tiếp theo
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
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

        // Cập nhật ngay ngày chọn và highlight dải 7 ngày để UI phản hồi tức thì (0ms)
        SelectedDate = targetDate;
        foreach (var day in WeekDays)
        {
            day.IsSelected = day.Date.Date == targetDate.Date;
        }

        // Hủy lượt nạp cũ nếu người dùng chạm liên tục giữa nhiều ngày (Last-Request-Wins)
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        var token = _loadCts.Token;

        try
        {
            IsBusy = true;
            DayTasks.Clear();

            // Micro-transition delay 250ms hiển thị Skeleton mượt mà, tránh flicker
            if (TransitionDelayMs > 0)
            {
                await Task.Delay(TransitionDelayMs, token);
            }

            var tasks = await _taskService.GetTasksByDateAsync(_selectedDate);
            if (token.IsCancellationRequested) return;

            foreach (var task in tasks.OrderBy(t => t.DueTime))
            {
                DayTasks.Add(task);
            }

            UpdateDayProgressMetrics();
        }
        catch (OperationCanceledException)
        {
            // Bỏ qua ngoại lệ khi người dùng chạm ngày khác
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
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
            string emptyWarning = _localizationService?["SnackbarEnterTitle"] ?? "Vui lòng nhập tên công việc";
            ShowSnackbar(emptyWarning, canUndo: false, SnackbarType.Warning);
            return;
        }

        string defaultProject = _localizationService?["DefaultProjectPersonal"] ?? "Công việc cá nhân";
        var newTask = new TaskItem
        {
            Title = NewTaskTitle.Trim(),
            Description = NewTaskDescription?.Trim() ?? string.Empty,
            ProjectName = string.IsNullOrWhiteSpace(NewTaskProject) ? defaultProject : NewTaskProject.Trim(),
            DueDate = NewTaskDueDate.Date,
            DueTime = NewTaskDueTime,
            Status = TaskStatus.Pending
        };

        // Đóng modal tạo việc
        IsCreateModalVisible = false;

        // Optimistic UI cho WeekPage: Thêm trực tiếp thẻ công việc 0ms
        _isInternalUpdate = true;
        try
        {
            if (newTask.DueDate.Date == SelectedDate.Date)
            {
                DayTasks.Add(newTask);
                UpdateDayProgressMetrics();
            }

            var dayItem = WeekDays.FirstOrDefault(d => d.Date.Date == newTask.DueDate.Date);
            if (dayItem != null)
            {
                dayItem.TaskCount++;
            }

            string createSuccess = _localizationService?["SnackbarTaskCreated"] ?? "Đã tạo công việc mới thành công";
            ShowSnackbar(createSuccess, canUndo: false);

            bool success = await _taskService.AddTaskAsync(newTask);
            if (!success)
            {
                if (newTask.DueDate.Date == SelectedDate.Date)
                {
                    DayTasks.Remove(newTask);
                    UpdateDayProgressMetrics();
                }
                if (dayItem != null && dayItem.TaskCount > 0)
                {
                    dayItem.TaskCount--;
                }
            }
        }
        finally
        {
            _isInternalUpdate = false;
        }
    }

    private void ShowSnackbar(string message, bool canUndo, SnackbarType type = SnackbarType.Success)
    {
        _snackbarCts?.Cancel();
        _snackbarCts = new CancellationTokenSource();

        SnackbarMessage = message;
        CanUndo = canUndo;
        IsSnackbarVisible = true;

        if (canUndo)
        {
            _snackbarService.ShowUndo(message, async () => await ExecuteUndoDeleteAsync());
        }
        else if (type == SnackbarType.Warning)
        {
            _snackbarService.ShowWarning(message);
        }
        else if (type == SnackbarType.Danger)
        {
            _snackbarService.ShowDanger(message);
        }
        else
        {
            _snackbarService.ShowSuccess(message);
        }

        var token = _snackbarCts.Token;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(5000, token);
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
        _snackbarService.Dismiss();
    }

    private void OnTasksChanged(object? sender, EventArgs e)
    {
        if (_isInternalUpdate) return;

        if (!IsPageVisible)
        {
            _isDataDirty = true;
            return;
        }

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
