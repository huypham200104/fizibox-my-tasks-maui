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
/// ViewModel quản lý dữ liệu và tương tác cho màn hình Lịch sử / Đã xong (HistoryPage)
/// Tuân thủ Clean Architecture, MVVM thuần C#, và các quy chuẩn trong BUSINESS.md và DESIGN.md
/// </summary>
public class HistoryViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly ILocalizationService? _localizationService;
    private readonly CultureInfo _viCulture = new("vi-VN");

    private string _searchText = string.Empty;
    private HistoryFilterType _selectedFilter = HistoryFilterType.All;
    private bool _isSortDescending = true;
    private int _completedTasksCount;
    private string _filterPeriodText = string.Empty;
    private bool _isRefreshing;
    private bool _isDataLoaded;
    private bool _isInternalUpdate;

    private readonly List<TaskItem> _rawCompletedTasks = new();

    public ObservableCollection<HistoryGroupItem> GroupedTasks { get; } = new();

    // =================================================================
    // PROPERTIES BINDING
    // =================================================================

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                OnPropertyChanged(nameof(HasSearchText));
                ApplyFilterAndGrouping();
            }
        }
    }

    public bool HasSearchText => !string.IsNullOrWhiteSpace(_searchText);

    public HistoryFilterType SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
            {
                NotifyFilterProperties();
                UpdateFilterPeriodText();
                ApplyFilterAndGrouping();
            }
        }
    }

    public bool IsFilterAll => _selectedFilter == HistoryFilterType.All;
    public bool IsFilterToday => _selectedFilter == HistoryFilterType.Today;
    public bool IsFilterWeek => _selectedFilter == HistoryFilterType.ThisWeek;
    public bool IsFilterMonth => _selectedFilter == HistoryFilterType.ThisMonth;

    public bool IsSortDescending
    {
        get => _isSortDescending;
        set
        {
            if (SetProperty(ref _isSortDescending, value))
            {
                ApplyFilterAndGrouping();
            }
        }
    }

    public int CompletedTasksCount
    {
        get => _completedTasksCount;
        private set => SetProperty(ref _completedTasksCount, value);
    }

    public string FilterPeriodText
    {
        get => _filterPeriodText;
        private set => SetProperty(ref _filterPeriodText, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool HasCompletedTasks => GroupedTasks.Count > 0;
    public bool IsEmpty => !IsBusy && _rawCompletedTasks.Count == 0 && string.IsNullOrWhiteSpace(_searchText);
    public bool IsSearchEmpty => !IsBusy && _rawCompletedTasks.Count > 0 && GroupedTasks.Count == 0 && !string.IsNullOrWhiteSpace(_searchText);

    // =================================================================
    // LOCALIZATION WRAPPERS FOR UI BINDING
    // =================================================================

    public string PageTitle => _localizationService?["HistoryTitle"] ?? "Lịch sử";
    public string PageSubtitle => _localizationService?["HistorySubtitle"] ?? "Những công việc bạn đã hoàn thành";
    public string AppSubtitle => _localizationService?["AppSubtitle"] ?? "Việc của tôi";
    public string CompletedBadge => _localizationService?["HistoryCompletedBadge"] ?? "Đã hoàn thành";
    public string CompletedSectionTitle => _localizationService?["HistoryCompletedSection"] ?? "Công việc đã hoàn thành";
    public string AchievementTitle => _localizationService?["HistoryAchievementTitle"] ?? "Công việc đã hoàn thành";
    public string SearchPlaceholder => _localizationService?["HistorySearchPlaceholder"] ?? "Tìm kiếm công việc đã hoàn thành...";
    public string EmptyMessage => _localizationService?["HistoryEmptyMessage"] ?? "Chưa có công việc hoàn thành";
    public string EmptySubMessage => _localizationService?["HistoryEmptySubMessage"] ?? "Những công việc bạn hoàn thành sẽ xuất hiện tại đây.";
    public string SearchEmptyMessage => _localizationService?["HistorySearchEmptyMessage"] ?? "Không tìm thấy kết quả";
    public string SearchEmptySubMessage => _localizationService?["HistorySearchEmptySubMessage"] ?? "Thử tìm kiếm với từ khóa khác hoặc xóa bộ lọc.";
    public string ClearSearchText => _localizationService?["HistoryClearSearch"] ?? "Xóa tìm kiếm";
    public string BackToTodayText => _localizationService?["HistoryBackToToday"] ?? "Quay lại hôm nay";
    public string FilterAllText => _localizationService?["HistoryFilterAll"] ?? "Tất cả";
    public string FilterTodayText => _localizationService?["HistoryFilterToday"] ?? "Hôm nay";
    public string FilterWeekText => _localizationService?["HistoryFilterWeek"] ?? "Tuần này";
    public string FilterMonthText => _localizationService?["HistoryFilterMonth"] ?? "Tháng này";

    // =================================================================
    // COMMANDS
    // =================================================================

    public AsyncRelayCommand LoadHistoryCommand { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public RelayCommand<object> SelectFilterCommand { get; }
    public RelayCommand ToggleSortOrderCommand { get; }
    public RelayCommand ClearSearchCommand { get; }
    public AsyncRelayCommand NavigateToTodayCommand { get; }
    public AsyncRelayCommand<TaskItem> OpenTaskDetailCommand { get; }
    public AsyncRelayCommand<TaskItem> ReopenTaskCommand { get; }
    public AsyncRelayCommand<TaskItem> DeleteTaskCommand { get; }

    // =================================================================
    // CONSTRUCTOR
    // =================================================================

    public HistoryViewModel(ITaskService taskService, ILocalizationService? localizationService = null)
    {
        _taskService = taskService;
        _localizationService = localizationService;
        Title = PageTitle;

        LoadHistoryCommand = new AsyncRelayCommand(ExecuteLoadHistoryAsync);
        RefreshCommand = new AsyncRelayCommand(ExecuteRefreshAsync);
        SelectFilterCommand = new RelayCommand<object>(ExecuteSelectFilter);
        ToggleSortOrderCommand = new RelayCommand(ExecuteToggleSortOrder);
        ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
        NavigateToTodayCommand = new AsyncRelayCommand(ExecuteNavigateToTodayAsync);
        OpenTaskDetailCommand = new AsyncRelayCommand<TaskItem>(ExecuteOpenTaskDetailAsync);
        ReopenTaskCommand = new AsyncRelayCommand<TaskItem>(ExecuteReopenTaskAsync);
        DeleteTaskCommand = new AsyncRelayCommand<TaskItem>(ExecuteDeleteTaskAsync);

        _taskService.TasksChanged += OnTasksChanged;
        if (_localizationService != null)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        UpdateFilterPeriodText();
    }

    public async Task EnsureDataLoadedAsync()
    {
        if (_isDataLoaded) return;
        await ExecuteLoadHistoryAsync();
    }

    private async Task ExecuteLoadHistoryAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var tasks = await _taskService.GetCompletedTasksAsync();

            _rawCompletedTasks.Clear();
            _rawCompletedTasks.AddRange(tasks);

            UpdateFilterPeriodText();
            ApplyFilterAndGrouping();
            _isDataLoaded = true;
        }
        finally
        {
            IsBusy = false;
            NotifyStateProperties();
        }
    }

    private async Task ExecuteRefreshAsync()
    {
        try
        {
            IsRefreshing = true;
            var tasks = await _taskService.GetCompletedTasksAsync();

            _rawCompletedTasks.Clear();
            _rawCompletedTasks.AddRange(tasks);

            UpdateFilterPeriodText();
            ApplyFilterAndGrouping();
        }
        finally
        {
            IsRefreshing = false;
            NotifyStateProperties();
        }
    }

    private void ExecuteSelectFilter(object? filterParam)
    {
        if (filterParam is HistoryFilterType filterType)
        {
            SelectedFilter = filterType;
        }
        else if (filterParam is string filterStr && Enum.TryParse<HistoryFilterType>(filterStr, true, out var parsed))
        {
            SelectedFilter = parsed;
        }
    }

    private void ExecuteToggleSortOrder()
    {
        IsSortDescending = !IsSortDescending;
    }

    private void ExecuteClearSearch()
    {
        SearchText = string.Empty;
    }

    private async Task ExecuteNavigateToTodayAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//TodayPage");
        }
    }

    private async Task ExecuteOpenTaskDetailAsync(TaskItem? task)
    {
        if (task == null || Shell.Current == null) return;
        await Shell.Current.GoToAsync($"{nameof(TaskDetailPage)}?id={task.Id}");
    }

    private async Task ExecuteReopenTaskAsync(TaskItem? task)
    {
        if (task == null) return;

        bool confirm = true;
        if (Shell.Current != null)
        {
            string title = _localizationService?["HistoryReopenTitle"] ?? "Mở lại công việc?";
            string message = _localizationService?["HistoryReopenMessage"] ?? "Bạn có chắc chắn muốn mở lại công việc này không? Trạng thái sẽ được chuyển về Chưa hoàn thành.";
            string confirmText = _localizationService?["HistoryReopenConfirm"] ?? "Xác nhận";
            string cancelText = _localizationService?["HistoryReopenCancel"] ?? "Hủy";

            confirm = await Shell.Current.DisplayAlertAsync(title, message, confirmText, cancelText);
        }

        if (confirm)
        {
            _isInternalUpdate = true;
            try
            {
                await _taskService.SetTaskStatusAsync(task.Id, TaskStatus.Pending);
                _rawCompletedTasks.RemoveAll(t => t.Id == task.Id);
                ApplyFilterAndGrouping();
            }
            finally
            {
                _isInternalUpdate = false;
            }
        }
    }

    private async Task ExecuteDeleteTaskAsync(TaskItem? task)
    {
        if (task == null) return;

        _isInternalUpdate = true;
        try
        {
            await _taskService.DeleteTaskAsync(task.Id);
            _rawCompletedTasks.RemoveAll(t => t.Id == task.Id);
            ApplyFilterAndGrouping();
        }
        finally
        {
            _isInternalUpdate = false;
        }
    }

    private void OnTasksChanged(object? sender, EventArgs e)
    {
        if (_isInternalUpdate) return;

        RunOnMainThread(async () =>
        {
            var tasks = await _taskService.GetCompletedTasksAsync();
            _rawCompletedTasks.Clear();
            _rawCompletedTasks.AddRange(tasks);
            UpdateFilterPeriodText();
            ApplyFilterAndGrouping();
        });
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RunOnMainThread(() =>
        {
            Title = PageTitle;
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(PageSubtitle));
            OnPropertyChanged(nameof(AppSubtitle));
            OnPropertyChanged(nameof(CompletedBadge));
            OnPropertyChanged(nameof(CompletedSectionTitle));
            OnPropertyChanged(nameof(AchievementTitle));
            OnPropertyChanged(nameof(SearchPlaceholder));
            OnPropertyChanged(nameof(EmptyMessage));
            OnPropertyChanged(nameof(EmptySubMessage));
            OnPropertyChanged(nameof(SearchEmptyMessage));
            OnPropertyChanged(nameof(SearchEmptySubMessage));
            OnPropertyChanged(nameof(ClearSearchText));
            OnPropertyChanged(nameof(BackToTodayText));
            OnPropertyChanged(nameof(FilterAllText));
            OnPropertyChanged(nameof(FilterTodayText));
            OnPropertyChanged(nameof(FilterWeekText));
            OnPropertyChanged(nameof(FilterMonthText));

            UpdateFilterPeriodText();
            ApplyFilterAndGrouping();
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

    public void ApplyFilterAndGrouping()
    {
        DateTime today = DateTime.Today;
        DateTime mondayOfWeek = WeekViewModel.GetMondayOfWeek(today);
        DateTime sundayOfWeek = mondayOfWeek.AddDays(6);

        // 1. Lọc theo thời gian (Filter by Period)
        IEnumerable<TaskItem> filtered = _rawCompletedTasks.Where(t =>
        {
            DateTime itemDate = (t.CompletedAt ?? t.DueDate).Date;
            return _selectedFilter switch
            {
                HistoryFilterType.Today => itemDate == today,
                HistoryFilterType.ThisWeek => itemDate >= mondayOfWeek && itemDate <= sundayOfWeek,
                HistoryFilterType.ThisMonth => itemDate.Year == today.Year && itemDate.Month == today.Month,
                _ => true
            };
        });

        // 2. Lọc theo từ khóa tìm kiếm (Filter by Search Keyword)
        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            string keyword = _searchText.Trim();
            filtered = filtered.Where(t =>
                (!string.IsNullOrEmpty(t.Title) && t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(t.ProjectName) && t.ProjectName.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
        }

        var filteredList = filtered.ToList();
        CompletedTasksCount = filteredList.Count;

        // 3. Sắp xếp theo ngày & giờ hoàn thành (Sort order)
        filteredList = _isSortDescending
            ? filteredList.OrderByDescending(t => t.CompletedAt ?? t.DueDate).ThenByDescending(t => t.DueTime).ToList()
            : filteredList.OrderBy(t => t.CompletedAt ?? t.DueDate).ThenBy(t => t.DueTime).ToList();

        // 4. Gom nhóm theo ngày (Grouping by Date)
        var groups = filteredList
            .GroupBy(t => (t.CompletedAt ?? t.DueDate).Date);

        groups = _isSortDescending
            ? groups.OrderByDescending(g => g.Key)
            : groups.OrderBy(g => g.Key);

        GroupedTasks.Clear();
        foreach (var group in groups)
        {
            DateTime date = group.Key;
            string title;
            string subtitle;

            bool isEn = _localizationService?.CurrentLanguage == AppLanguage.English;

            if (date == today)
            {
                title = isEn ? "Today" : "Hôm nay";
                subtitle = isEn ? date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture) : date.ToString("dd 'tháng' MM, yyyy", _viCulture);
            }
            else if (date == today.AddDays(-1))
            {
                title = isEn ? "Yesterday" : "Hôm qua";
                subtitle = isEn ? date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture) : date.ToString("dd 'tháng' MM, yyyy", _viCulture);
            }
            else
            {
                if (isEn)
                {
                    title = date.ToString("dddd, MMM dd", CultureInfo.InvariantCulture);
                    subtitle = date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    title = FormatVietnameseDateTitle(date);
                    subtitle = date.ToString("dd 'tháng' MM, yyyy", _viCulture);
                }
            }

            GroupedTasks.Add(new HistoryGroupItem(date, title, subtitle, group));
        }

        NotifyStateProperties();
    }

    private string FormatVietnameseDateTitle(DateTime date)
    {
        string dayOfWeek = date.DayOfWeek switch
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

        return $"{dayOfWeek}, {date.Day} tháng {date.Month}";
    }

    private void UpdateFilterPeriodText()
    {
        bool isEn = _localizationService?.CurrentLanguage == AppLanguage.English;
        DateTime today = DateTime.Today;

        FilterPeriodText = _selectedFilter switch
        {
            HistoryFilterType.Today => isEn ? "Today" : "Trong ngày hôm nay",
            HistoryFilterType.ThisWeek => isEn ? "This week" : "Trong tuần này",
            HistoryFilterType.ThisMonth => isEn
                ? $"In {today:MMMM, yyyy}"
                : $"Trong tháng {today.Month}, {today.Year}",
            _ => isEn ? "All time" : "Tất cả thời gian"
        };
    }

    private void NotifyFilterProperties()
    {
        OnPropertyChanged(nameof(IsFilterAll));
        OnPropertyChanged(nameof(IsFilterToday));
        OnPropertyChanged(nameof(IsFilterWeek));
        OnPropertyChanged(nameof(IsFilterMonth));
    }

    private void NotifyStateProperties()
    {
        OnPropertyChanged(nameof(HasCompletedTasks));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(IsSearchEmpty));
    }
}
