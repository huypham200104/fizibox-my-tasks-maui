using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;
using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.ViewModels.Base;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.ViewModels;

/// <summary>
/// ViewModel quản lý dữ liệu và toàn bộ tương tác cho màn hình Chi tiết công việc (TaskDetailPage)
/// Tuân thủ Clean Architecture, MVVM thuần C#, và các quy tắc đặc tả trong BUSINESS.md và DESIGN.md
/// </summary>
public class TaskDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ITaskService _taskService;
    private readonly ILocalizationService _localizationService;
    private readonly ISnackbarService _snackbarService;
    private readonly CultureInfo _viCulture = new("vi-VN");

    private TaskItem? _currentTask;
    private bool _isNotFound;
    private int _taskId;

    // Trạng thái hiển thị Modals & Bottom Sheets
    private bool _isActionMenuVisible;
    private bool _isEditModalVisible;
    private bool _isStatusSheetVisible;
    private bool _isDatePickerSheetVisible;
    private bool _isTimePickerSheetVisible;
    private bool _isProjectSheetVisible;
    private bool _isReopenConfirmVisible;
    private bool _isDeleteConfirmVisible;

    // Dữ liệu tạm thời cho Modal Chỉnh sửa (Edit Modal)
    private string _editTitle = string.Empty;
    private string _editDescription = string.Empty;
    private string _editNote = string.Empty;
    private string _editProjectName = "Công việc cá nhân";

    // Dữ liệu tạm thời cho Bottom Sheet Trạng thái
    private TaskStatus _selectedTempStatus = TaskStatus.Pending;

    // Dữ liệu tạm thời cho Bottom Sheet Chọn ngày (Date Picker)
    private DateTime _pickerCalendarMonth = DateTime.Today;
    private DateTime _pickerSelectedDate = DateTime.Today;

    // Dữ liệu tạm thời cho Bottom Sheet Chọn giờ (Time Picker)
    private TimeSpan _pickerSelectedTime = new(14, 0, 0);
    private int _pickerSelectedHour = 14;
    private int _pickerSelectedMinute = 0;

    public TaskDetailViewModel(ITaskService taskService, ILocalizationService localizationService, ISnackbarService? snackbarService = null)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        _snackbarService = snackbarService ?? new SnackbarService();

        Title = _localizationService["DetailTitle"];

        // Khởi tạo các Commands
        GoBackCommand = new AsyncRelayCommand(OnGoBackAsync);
        ToggleMainStatusCommand = new AsyncRelayCommand(OnToggleMainStatusAsync);
        ConfirmReopenCommand = new AsyncRelayCommand(OnConfirmReopenAsync);
        CancelReopenCommand = new RelayCommand(() => IsReopenConfirmVisible = false);

        OpenActionMenuCommand = new RelayCommand(() => IsActionMenuVisible = true);
        CloseActionMenuCommand = new RelayCommand(() => IsActionMenuVisible = false);

        OpenEditModalCommand = new RelayCommand(OnOpenEditModal);
        CloseEditModalCommand = new RelayCommand(() => IsEditModalVisible = false);
        SaveEditCommand = new AsyncRelayCommand(OnSaveEditAsync);

        OpenStatusSheetCommand = new RelayCommand(OnOpenStatusSheet);
        CloseStatusSheetCommand = new RelayCommand(() => IsStatusSheetVisible = false);
        SelectStatusOptionCommand = new AsyncRelayCommand<TaskStatus>(OnSelectStatusOptionAsync);

        OpenDatePickerSheetCommand = new RelayCommand(OnOpenDatePickerSheet);
        CloseDatePickerSheetCommand = new RelayCommand(() => IsDatePickerSheetVisible = false);
        PrevMonthCommand = new RelayCommand(OnPrevMonth);
        NextMonthCommand = new RelayCommand(OnNextMonth);
        SelectDateDayCommand = new RelayCommand<CalendarDayItem>(OnSelectDateDay);
        SelectTodayShortcutCommand = new RelayCommand(OnSelectTodayShortcut);
        ApplyDatePickerCommand = new AsyncRelayCommand(OnApplyDatePickerAsync);

        OpenTimePickerSheetCommand = new RelayCommand(OnOpenTimePickerSheet);
        CloseTimePickerSheetCommand = new RelayCommand(() => IsTimePickerSheetVisible = false);
        SelectQuickTimePresetCommand = new RelayCommand<string>(OnSelectQuickTimePreset);
        SelectHourCommand = new RelayCommand<int>(OnSelectHour);
        SelectMinuteCommand = new RelayCommand<int>(OnSelectMinute);
        ApplyTimePickerCommand = new AsyncRelayCommand(OnApplyTimePickerAsync);

        OpenProjectSheetCommand = new RelayCommand(() => IsProjectSheetVisible = true);
        CloseProjectSheetCommand = new RelayCommand(() => IsProjectSheetVisible = false);
        SelectProjectCommand = new AsyncRelayCommand<string>(OnSelectProjectAsync);

        RequestDeleteCommand = new RelayCommand(OnRequestDelete);
        ConfirmDeleteCommand = new AsyncRelayCommand(OnConfirmDeleteAsync);
        CancelDeleteCommand = new RelayCommand(() => IsDeleteConfirmVisible = false);

        // Khởi tạo danh sách dự án
        AvailableProjects = new ObservableCollection<string>
        {
            "Công việc cá nhân",
            "Dự án Mobile App",
            "Dự án Backend",
            "MyTasks App"
        };

        // Khởi tạo danh sách giờ và phút cho bộ chọn trực quan
        AvailableHours = new ObservableCollection<int>();
        for (int i = 0; i < 24; i++) AvailableHours.Add(i);

        AvailableMinutes = new ObservableCollection<int> { 0, 15, 30, 45 };

        _localizationService.LanguageChanged += (_, _) =>
        {
            Title = _localizationService["DetailTitle"];
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(CurrentStatusText));
            OnPropertyChanged(nameof(MainCtaButtonText));
            OnPropertyChanged(nameof(FormattedCompletedAtText));
            OnPropertyChanged(nameof(SectionDescTitle));
            OnPropertyChanged(nameof(SectionInfoTitle));
            OnPropertyChanged(nameof(DueDateLabel));
            OnPropertyChanged(nameof(DueTimeLabel));
            OnPropertyChanged(nameof(ProjectLabel));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(SectionNotesTitle));
            OnPropertyChanged(nameof(SheetStatusTitle));
            OnPropertyChanged(nameof(SheetStatusSubtitle));
            OnPropertyChanged(nameof(StatusPendingTitle));
            OnPropertyChanged(nameof(StatusPendingDesc));
            OnPropertyChanged(nameof(StatusInProgressTitle));
            OnPropertyChanged(nameof(StatusInProgressDesc));
            OnPropertyChanged(nameof(StatusCompletedTitle));
            OnPropertyChanged(nameof(StatusCompletedDesc));
            OnPropertyChanged(nameof(MenuEditTitle));
            OnPropertyChanged(nameof(MenuDeleteTitle));
            OnPropertyChanged(nameof(DialogReopenTitle));
            OnPropertyChanged(nameof(DialogReopenMsg));
            OnPropertyChanged(nameof(DialogDeleteTitle));
            OnPropertyChanged(nameof(DialogDeleteMsg));
            OnPropertyChanged(nameof(DialogCancelText));
            OnPropertyChanged(nameof(DialogConfirmText));
            OnPropertyChanged(nameof(DialogDeleteConfirmText));
            OnPropertyChanged(nameof(NotFoundTitle));
            OnPropertyChanged(nameof(NotFoundDesc));
            OnPropertyChanged(nameof(NotFoundGoBackText));
            OnPropertyChanged(nameof(PickerMonthText));
            OnPropertyChanged(nameof(PickerSelectedDateFormatted));
        };
    }

    #region Observable Properties - Task & Trạng thái chính

    public TaskItem? CurrentTask
    {
        get => _currentTask;
        private set
        {
            if (SetProperty(ref _currentTask, value))
            {
                OnPropertyChanged(nameof(HasTask));
                OnPropertyChanged(nameof(IsCompletedState));
                OnPropertyChanged(nameof(CurrentStatusText));
                OnPropertyChanged(nameof(FormattedCompletedAtText));
                OnPropertyChanged(nameof(MainCtaButtonText));
                OnPropertyChanged(nameof(IsTempStatusPending));
                OnPropertyChanged(nameof(IsTempStatusInProgress));
                OnPropertyChanged(nameof(IsTempStatusCompleted));
            }
        }
    }

    public bool HasTask => _currentTask != null;
    public bool IsCompletedState => _currentTask?.IsCompleted == true;

    public bool IsNotFound
    {
        get => _isNotFound;
        set => SetProperty(ref _isNotFound, value);
    }

    public int TaskId
    {
        get => _taskId;
        private set => SetProperty(ref _taskId, value);
    }

    public string MainCtaButtonText => IsCompletedState
        ? _localizationService["DetailActionReopen"]
        : _localizationService["DetailActionComplete"];

    public string FormattedCompletedAtText
    {
        get
        {
            if (_currentTask?.CompletedAt == null) return string.Empty;
            var dt = _currentTask.CompletedAt.Value;
            return string.Format(_localizationService["DetailCompletedAt"], dt.ToString("HH:mm"), dt.ToString("dd/MM/yyyy"));
        }
    }

    public string CurrentStatusText => CurrentTask?.Status switch
    {
        TaskStatus.Completed => _localizationService["StatusCompletedTitle"],
        TaskStatus.InProgress => _localizationService["StatusInProgressTitle"],
        _ => _localizationService["StatusPendingTitle"]
    };

    // Section headers & labels
    public string SectionDescTitle => _localizationService["DetailSectionDesc"];
    public string SectionInfoTitle => _localizationService["DetailSectionInfo"];
    public string DueDateLabel => _localizationService["DetailDueDate"];
    public string DueTimeLabel => _localizationService["DetailDueTime"];
    public string ProjectLabel => _localizationService["DetailProject"];
    public string StatusLabel => _localizationService["DetailStatus"];
    public string SectionNotesTitle => _localizationService["DetailSectionNotes"];

    // Bottom Sheet Status
    public string SheetStatusTitle => _localizationService["SheetStatusTitle"];
    public string SheetStatusSubtitle => _localizationService["SheetStatusSubtitle"];
    public string StatusPendingTitle => _localizationService["StatusPendingTitle"];
    public string StatusPendingDesc => _localizationService["StatusPendingDesc"];
    public string StatusInProgressTitle => _localizationService["StatusInProgressTitle"];
    public string StatusInProgressDesc => _localizationService["StatusInProgressDesc"];
    public string StatusCompletedTitle => _localizationService["StatusCompletedTitle"];
    public string StatusCompletedDesc => _localizationService["StatusCompletedDesc"];

    // Action Menu
    public string MenuEditTitle => _localizationService["DetailMenuEdit"];
    public string MenuDeleteTitle => _localizationService["DetailMenuDelete"];

    // Dialogs & Not Found
    public string DialogReopenTitle => _localizationService["DialogReopenTitle"];
    public string DialogReopenMsg => _localizationService["DialogReopenMsg"];
    public string DialogDeleteTitle => _localizationService["DialogDeleteTitle"];
    public string DialogDeleteMsg => _localizationService["DialogDeleteMsg"];
    public string DialogCancelText => _localizationService["BtnCancel"];
    public string DialogConfirmText => _localizationService["BtnConfirm"];
    public string DialogDeleteConfirmText => _localizationService["BtnDelete"];
    public string NotFoundTitle => _localizationService["NotFoundDetailTitle"];
    public string NotFoundDesc => _localizationService["NotFoundDetailDesc"];
    public string NotFoundGoBackText => _localizationService["BtnBackToList"];

    #endregion

    #region Observable Properties - Modals & Bottom Sheets Visibility

    public bool IsActionMenuVisible
    {
        get => _isActionMenuVisible;
        set => SetProperty(ref _isActionMenuVisible, value);
    }

    public bool IsEditModalVisible
    {
        get => _isEditModalVisible;
        set => SetProperty(ref _isEditModalVisible, value);
    }

    public bool IsStatusSheetVisible
    {
        get => _isStatusSheetVisible;
        set => SetProperty(ref _isStatusSheetVisible, value);
    }

    public bool IsDatePickerSheetVisible
    {
        get => _isDatePickerSheetVisible;
        set => SetProperty(ref _isDatePickerSheetVisible, value);
    }

    public bool IsTimePickerSheetVisible
    {
        get => _isTimePickerSheetVisible;
        set => SetProperty(ref _isTimePickerSheetVisible, value);
    }

    public bool IsProjectSheetVisible
    {
        get => _isProjectSheetVisible;
        set => SetProperty(ref _isProjectSheetVisible, value);
    }

    public bool IsReopenConfirmVisible
    {
        get => _isReopenConfirmVisible;
        set => SetProperty(ref _isReopenConfirmVisible, value);
    }

    public bool IsDeleteConfirmVisible
    {
        get => _isDeleteConfirmVisible;
        set => SetProperty(ref _isDeleteConfirmVisible, value);
    }

    #endregion

    #region Observable Properties - Form Edit Modal (popup_edit_text.png)

    public string EditTitle
    {
        get => _editTitle;
        set
        {
            if (SetProperty(ref _editTitle, value))
            {
                OnPropertyChanged(nameof(EditTitleCharCountText));
            }
        }
    }

    public string EditDescription
    {
        get => _editDescription;
        set
        {
            if (SetProperty(ref _editDescription, value))
            {
                OnPropertyChanged(nameof(EditDescCharCountText));
            }
        }
    }

    public string EditNote
    {
        get => _editNote;
        set
        {
            if (SetProperty(ref _editNote, value))
            {
                OnPropertyChanged(nameof(EditNoteCharCountText));
            }
        }
    }

    public string EditProjectName
    {
        get => _editProjectName;
        set => SetProperty(ref _editProjectName, value);
    }

    public string EditTitleCharCountText => $"{_editTitle?.Length ?? 0}/100";
    public string EditDescCharCountText => $"{_editDescription?.Length ?? 0}/1000";
    public string EditNoteCharCountText => $"{_editNote?.Length ?? 0}/500";

    #endregion

    #region Observable Properties - Date Picker Sheet (bottom_sheet_date_picker.png)

    public DateTime PickerCalendarMonth
    {
        get => _pickerCalendarMonth;
        set
        {
            if (SetProperty(ref _pickerCalendarMonth, value))
            {
                OnPropertyChanged(nameof(PickerMonthText));
            }
        }
    }

    public DateTime PickerSelectedDate
    {
        get => _pickerSelectedDate;
        set
        {
            if (SetProperty(ref _pickerSelectedDate, value))
            {
                OnPropertyChanged(nameof(PickerSelectedDateFormatted));
            }
        }
    }

    public string PickerMonthText => $"Tháng {_pickerCalendarMonth.Month}, {_pickerCalendarMonth.Year}";

    public string PickerSelectedDateFormatted
    {
        get
        {
            string dayOfWeek = _pickerSelectedDate.DayOfWeek switch
            {
                DayOfWeek.Monday => "Thứ Hai",
                DayOfWeek.Tuesday => "Thứ Ba",
                DayOfWeek.Wednesday => "Thứ Tư",
                DayOfWeek.Thursday => "Thứ Năm",
                DayOfWeek.Friday => "Thứ Sáu",
                DayOfWeek.Saturday => "Thứ Bảy",
                DayOfWeek.Sunday => "Chủ Nhật",
                _ => string.Empty
            };
            return $"{dayOfWeek}, {_pickerSelectedDate:dd/MM/yyyy}";
        }
    }

    public ObservableCollection<CalendarDayItem> CalendarDays { get; } = new();

    #endregion

    #region Observable Properties - Time Picker Sheet (bottom_sheet_time_picker.png)

    public TimeSpan PickerSelectedTime
    {
        get => _pickerSelectedTime;
        set
        {
            if (SetProperty(ref _pickerSelectedTime, value))
            {
                OnPropertyChanged(nameof(PickerTimeFormatted));
            }
        }
    }

    public int PickerSelectedHour
    {
        get => _pickerSelectedHour;
        set => SetProperty(ref _pickerSelectedHour, value);
    }

    public int PickerSelectedMinute
    {
        get => _pickerSelectedMinute;
        set => SetProperty(ref _pickerSelectedMinute, value);
    }

    public string PickerTimeFormatted => _pickerSelectedTime.ToString(@"hh\:mm");

    public ObservableCollection<int> AvailableHours { get; }
    public ObservableCollection<int> AvailableMinutes { get; }

    #endregion

    #region Observable Properties - Status & Projects

    public TaskStatus SelectedTempStatus
    {
        get => _selectedTempStatus;
        set
        {
            if (SetProperty(ref _selectedTempStatus, value))
            {
                OnPropertyChanged(nameof(IsTempStatusPending));
                OnPropertyChanged(nameof(IsTempStatusInProgress));
                OnPropertyChanged(nameof(IsTempStatusCompleted));
            }
        }
    }

    public bool IsTempStatusPending => SelectedTempStatus == TaskStatus.Pending;
    public bool IsTempStatusInProgress => SelectedTempStatus == TaskStatus.InProgress;
    public bool IsTempStatusCompleted => SelectedTempStatus == TaskStatus.Completed;

    public ObservableCollection<string> AvailableProjects { get; }

    #endregion

    #region Commands Declarations

    public AsyncRelayCommand GoBackCommand { get; }
    public AsyncRelayCommand ToggleMainStatusCommand { get; }
    public AsyncRelayCommand ConfirmReopenCommand { get; }
    public RelayCommand CancelReopenCommand { get; }

    public RelayCommand OpenActionMenuCommand { get; }
    public RelayCommand CloseActionMenuCommand { get; }

    public RelayCommand OpenEditModalCommand { get; }
    public RelayCommand CloseEditModalCommand { get; }
    public AsyncRelayCommand SaveEditCommand { get; }

    public RelayCommand OpenStatusSheetCommand { get; }
    public RelayCommand CloseStatusSheetCommand { get; }
    public AsyncRelayCommand<TaskStatus> SelectStatusOptionCommand { get; }

    public RelayCommand OpenDatePickerSheetCommand { get; }
    public RelayCommand CloseDatePickerSheetCommand { get; }
    public RelayCommand PrevMonthCommand { get; }
    public RelayCommand NextMonthCommand { get; }
    public RelayCommand<CalendarDayItem> SelectDateDayCommand { get; }
    public RelayCommand SelectTodayShortcutCommand { get; }
    public AsyncRelayCommand ApplyDatePickerCommand { get; }

    public RelayCommand OpenTimePickerSheetCommand { get; }
    public RelayCommand CloseTimePickerSheetCommand { get; }
    public RelayCommand<string> SelectQuickTimePresetCommand { get; }
    public RelayCommand<int> SelectHourCommand { get; }
    public RelayCommand<int> SelectMinuteCommand { get; }
    public AsyncRelayCommand ApplyTimePickerCommand { get; }

    public RelayCommand OpenProjectSheetCommand { get; }
    public RelayCommand CloseProjectSheetCommand { get; }
    public AsyncRelayCommand<string> SelectProjectCommand { get; }

    public RelayCommand RequestDeleteCommand { get; }
    public AsyncRelayCommand ConfirmDeleteCommand { get; }
    public RelayCommand CancelDeleteCommand { get; }

    #endregion

    #region Navigation & Data Loading

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && idObj != null)
        {
            if (int.TryParse(idObj.ToString(), out int id))
            {
                TaskId = id;
                _ = LoadTaskByIdAsync(id);
            }
            else
            {
                IsNotFound = true;
            }
        }
        else
        {
            IsNotFound = true;
        }
    }

    public async Task LoadTaskByIdAsync(int id)
    {
        if (id <= 0)
        {
            IsNotFound = true;
            CurrentTask = null;
            return;
        }

        IsBusy = true;
        IsNotFound = false;

        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                IsNotFound = true;
                CurrentTask = null;
            }
            else
            {
                CurrentTask = task;
                IsNotFound = false;
            }
        }
        catch
        {
            IsNotFound = true;
            CurrentTask = null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnGoBackAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    #endregion

    #region Task Status & Reopen Handlers

    private async Task OnToggleMainStatusAsync()
    {
        if (CurrentTask == null) return;

        if (CurrentTask.IsCompleted)
        {
            // Bắt buộc mở dialog xác nhận mở lại việc (DESIGN.md 3.5 & BUSINESS.md 3.3)
            IsReopenConfirmVisible = true;
        }
        else
        {
            CurrentTask.Status = TaskStatus.Completed;
            CurrentTask.CompletedAt = DateTime.Now;
            await _taskService.UpdateTaskAsync(CurrentTask);
            OnPropertyChanged(nameof(IsCompletedState));
            OnPropertyChanged(nameof(CurrentStatusText));
            OnPropertyChanged(nameof(FormattedCompletedAtText));
            OnPropertyChanged(nameof(MainCtaButtonText));
            _snackbarService.ShowSuccess(_localizationService["SnackbarTaskCompleted"]);
        }
    }

    private async Task OnConfirmReopenAsync()
    {
        IsReopenConfirmVisible = false;
        if (CurrentTask != null)
        {
            CurrentTask.Status = TaskStatus.Pending;
            CurrentTask.CompletedAt = null;
            await _taskService.UpdateTaskAsync(CurrentTask);
            OnPropertyChanged(nameof(IsCompletedState));
            OnPropertyChanged(nameof(CurrentStatusText));
            OnPropertyChanged(nameof(FormattedCompletedAtText));
            OnPropertyChanged(nameof(MainCtaButtonText));
            _snackbarService.ShowSuccess(_localizationService["SnackbarTaskReopened"]);
        }
    }

    #endregion

    #region Edit Task Modal Handlers (popup_edit_text.png)

    private void OnOpenEditModal()
    {
        IsActionMenuVisible = false;
        if (CurrentTask != null)
        {
            EditTitle = CurrentTask.Title;
            EditDescription = CurrentTask.Description;
            EditNote = CurrentTask.Note;
            EditProjectName = CurrentTask.ProjectName;
            IsEditModalVisible = true;
        }
    }

    private async Task OnSaveEditAsync()
    {
        if (CurrentTask == null || string.IsNullOrWhiteSpace(EditTitle)) return;

        CurrentTask.Title = EditTitle.Trim();
        CurrentTask.Description = EditDescription?.Trim() ?? string.Empty;
        CurrentTask.Note = EditNote?.Trim() ?? string.Empty;
        CurrentTask.ProjectName = EditProjectName;

        await _taskService.UpdateTaskAsync(CurrentTask);
        IsEditModalVisible = false;
        _snackbarService.ShowSuccess(_localizationService["SnackbarTaskUpdated"]);
    }

    #endregion

    #region Status Sheet Handlers (bottom_sheet_change_status.png)

    private void OnOpenStatusSheet()
    {
        if (CurrentTask != null)
        {
            SelectedTempStatus = CurrentTask.Status;
            OnPropertyChanged(nameof(IsTempStatusPending));
            OnPropertyChanged(nameof(IsTempStatusInProgress));
            OnPropertyChanged(nameof(IsTempStatusCompleted));
            IsStatusSheetVisible = true;
        }
    }

    private async Task OnSelectStatusOptionAsync(TaskStatus status)
    {
        SelectedTempStatus = status;
        if (CurrentTask != null)
        {
            CurrentTask.Status = status;
            if (status == TaskStatus.Completed)
            {
                if (!CurrentTask.CompletedAt.HasValue)
                    CurrentTask.CompletedAt = DateTime.Now;
            }
            else
            {
                CurrentTask.CompletedAt = null;
            }

            await _taskService.UpdateTaskAsync(CurrentTask);
            OnPropertyChanged(nameof(IsCompletedState));
            OnPropertyChanged(nameof(CurrentStatusText));
            OnPropertyChanged(nameof(FormattedCompletedAtText));
            OnPropertyChanged(nameof(MainCtaButtonText));
        }

        IsStatusSheetVisible = false;
    }

    #endregion

    #region Date Picker Sheet Handlers (bottom_sheet_date_picker.png)

    private void OnOpenDatePickerSheet()
    {
        if (CurrentTask != null)
        {
            PickerCalendarMonth = CurrentTask.DueDate;
            PickerSelectedDate = CurrentTask.DueDate;
            GenerateCalendarGrid();
            IsDatePickerSheetVisible = true;
        }
    }

    private void OnPrevMonth()
    {
        PickerCalendarMonth = PickerCalendarMonth.AddMonths(-1);
        GenerateCalendarGrid();
    }

    private void NextMonth()
    {
        PickerCalendarMonth = PickerCalendarMonth.AddMonths(1);
        GenerateCalendarGrid();
    }

    private void OnNextMonth()
    {
        PickerCalendarMonth = PickerCalendarMonth.AddMonths(1);
        GenerateCalendarGrid();
    }

    private void OnSelectDateDay(CalendarDayItem? item)
    {
        if (item == null) return;
        PickerSelectedDate = item.Date;
        foreach (var day in CalendarDays)
        {
            day.IsSelected = day.Date.Date == item.Date.Date;
        }
        OnPropertyChanged(nameof(PickerSelectedDateFormatted));
    }

    private void OnSelectTodayShortcut()
    {
        PickerSelectedDate = DateTime.Today;
        PickerCalendarMonth = DateTime.Today;
        GenerateCalendarGrid();
    }

    private async Task OnApplyDatePickerAsync()
    {
        if (CurrentTask != null)
        {
            CurrentTask.DueDate = PickerSelectedDate;
            await _taskService.UpdateTaskAsync(CurrentTask);
        }
        IsDatePickerSheetVisible = false;
    }

    private void GenerateCalendarGrid()
    {
        CalendarDays.Clear();

        // Chuẩn ISO 8601: Tuần bắt đầu từ Thứ Hai
        var firstDayOfMonth = new DateTime(PickerCalendarMonth.Year, PickerCalendarMonth.Month, 1);
        int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Monday = 0, Sunday = 6

        var startDate = firstDayOfMonth.AddDays(-offset);

        // Tạo 42 ô ngày (6 tuần)
        for (int i = 0; i < 42; i++)
        {
            var date = startDate.AddDays(i);
            CalendarDays.Add(new CalendarDayItem
            {
                Date = date,
                DayNumber = date.Day,
                IsCurrentMonth = date.Month == PickerCalendarMonth.Month,
                IsToday = date.Date == DateTime.Today,
                IsSelected = date.Date == PickerSelectedDate.Date
            });
        }

        OnPropertyChanged(nameof(PickerMonthText));
        OnPropertyChanged(nameof(PickerSelectedDateFormatted));
    }

    #endregion

    #region Time Picker Sheet Handlers (bottom_sheet_time_picker.png)

    private void OnOpenTimePickerSheet()
    {
        if (CurrentTask != null)
        {
            PickerSelectedTime = CurrentTask.DueTime;
            PickerSelectedHour = PickerSelectedTime.Hours;
            PickerSelectedMinute = PickerSelectedTime.Minutes;
            IsTimePickerSheetVisible = true;
        }
    }

    private void OnSelectQuickTimePreset(string? preset)
    {
        if (!string.IsNullOrEmpty(preset) && TimeSpan.TryParse(preset, out var ts))
        {
            PickerSelectedTime = ts;
            PickerSelectedHour = ts.Hours;
            PickerSelectedMinute = ts.Minutes;
            OnPropertyChanged(nameof(PickerTimeFormatted));
        }
    }

    private void OnSelectHour(int hour)
    {
        PickerSelectedHour = hour;
        PickerSelectedTime = new TimeSpan(hour, PickerSelectedMinute, 0);
        OnPropertyChanged(nameof(PickerTimeFormatted));
    }

    private void OnSelectMinute(int minute)
    {
        PickerSelectedMinute = minute;
        PickerSelectedTime = new TimeSpan(PickerSelectedHour, minute, 0);
        OnPropertyChanged(nameof(PickerTimeFormatted));
    }

    private async Task OnApplyTimePickerAsync()
    {
        if (CurrentTask != null)
        {
            CurrentTask.DueTime = PickerSelectedTime;
            await _taskService.UpdateTaskAsync(CurrentTask);
        }
        IsTimePickerSheetVisible = false;
    }

    #endregion

    #region Project Sheet Handlers

    private async Task OnSelectProjectAsync(string? projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName)) return;

        if (CurrentTask != null)
        {
            CurrentTask.ProjectName = projectName;
            await _taskService.UpdateTaskAsync(CurrentTask);
        }

        EditProjectName = projectName;
        IsProjectSheetVisible = false;
    }

    #endregion

    #region Delete Handlers

    private void OnRequestDelete()
    {
        IsActionMenuVisible = false;
        IsDeleteConfirmVisible = true;
    }

    private async Task OnConfirmDeleteAsync()
    {
        IsDeleteConfirmVisible = false;
        if (CurrentTask != null)
        {
            var taskToDelete = CurrentTask;
            await _taskService.DeleteTaskAsync(taskToDelete.Id);

            _snackbarService.ShowUndo(_localizationService["SnackbarTaskDeleted"], async () =>
            {
                var restored = await _taskService.UndoDeleteAsync();
                if (restored != null)
                {
                    _snackbarService.ShowSuccess($"{_localizationService["SnackbarTaskRestored"]}{restored.Title}");
                }
            });

            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }

    #endregion
}
