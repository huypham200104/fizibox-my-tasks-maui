using System.Windows.Input;
using Microsoft.Maui.Graphics;
using MyTasksApp.Core.Enums;
using MyTasksApp.Services;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.ViewModels.Base;
using MyTasksApp.Views;

namespace MyTasksApp.ViewModels;

/// <summary>
/// ViewModel quản lý màn hình Cài đặt (SettingsPage)
/// Xử lý chuyển đổi Theme, Cỡ chữ, Ngôn ngữ, xem trước Live Preview và Reviewer Tools
/// Cung cấp trực tiếp các thuộc tính màu sắc, đường viền Bindable chống lỗi kẹt màu của MAUI DataTrigger
/// </summary>
public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private readonly ITaskService _taskService;
    private readonly ISnackbarService _snackbarService;
    private bool _isDataLoaded;
    public bool IsPageVisible { get; set; }

    public const int SkeletonMinTransitionDelayMs = 150;

    public async Task EnsurePageLoadedAsync()
    {
        if (_isDataLoaded) return;
        try
        {
            IsBusy = true;
            await Task.Delay(SkeletonMinTransitionDelayMs);
            _isDataLoaded = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Quản lý Snackbar thông báo phản hồi thao tác
    private bool _isSnackbarVisible;
    private string _snackbarMessage = string.Empty;
    private CancellationTokenSource? _snackbarCts;

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

    public SettingsViewModel(
        ISettingsService settingsService,
        ILocalizationService localizationService,
        ITaskService taskService,
        ISnackbarService? snackbarService = null)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;
        _taskService = taskService;
        _snackbarService = snackbarService ?? new SnackbarService();

        // Khởi tạo trạng thái ban đầu từ Services
        SyncStateFromSettings();

        // Đăng ký lắng nghe sự kiện
        _settingsService.SettingsChanged += OnSettingsChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Commands
        SelectThemeCommand = new RelayCommand<string>(OnSelectTheme);
        SelectFontSizeCommand = new RelayCommand<string>(OnSelectFontSize);
        SelectLanguageCommand = new RelayCommand<string>(OnSelectLanguage);
        SimulateLoadingCommand = new AsyncRelayCommand(OnSimulateLoadingAsync);
        SimulateEmptyCommand = new AsyncRelayCommand(OnSimulateEmptyAsync);
        SimulateNotFoundCommand = new AsyncRelayCommand(OnSimulateNotFoundAsync);
        ResetMockDataCommand = new AsyncRelayCommand(OnResetMockDataAsync);

        TestSnackbarSuccessCommand = new RelayCommand(OnTestSnackbarSuccess);
        TestSnackbarDangerCommand = new RelayCommand(OnTestSnackbarDanger);
        TestSnackbarWarningCommand = new RelayCommand(OnTestSnackbarWarning);
        TestSnackbarUndoCommand = new RelayCommand(OnTestSnackbarUndo);
    }

    #region Observable Properties - Trạng thái lựa chọn

    public bool IsLightMode => _settingsService.CurrentTheme == AppThemeMode.Light;
    public bool IsDarkMode => _settingsService.CurrentTheme == AppThemeMode.Dark;

    public bool IsSmallFont => _settingsService.CurrentFontSize == AppFontSize.Small;
    public bool IsNormalFont => _settingsService.CurrentFontSize == AppFontSize.Normal;
    public bool IsLargeFont => _settingsService.CurrentFontSize == AppFontSize.Large;

    public bool IsVietnamese => _settingsService.CurrentLanguage == AppLanguage.Vietnamese;
    public bool IsEnglish => _settingsService.CurrentLanguage == AppLanguage.English;

    #endregion

    #region Bindable Visual Properties (Chống Bug DataTrigger MAUI khi đổi Theme)

    private bool IsDark => _settingsService.CurrentTheme == AppThemeMode.Dark;

    // --- Theme Card: Sáng (Light) ---
    public Color ThemeLightCardBackground => IsLightMode
        ? (IsDark ? Color.FromArgb("#1E1B4B") : Color.FromArgb("#EEF2FF"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color ThemeLightCardBorder => IsLightMode
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double ThemeLightCardBorderThickness => IsLightMode ? 1.5 : 1.0;

    public Color ThemeLightTextColor => IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A");

    // --- Theme Card: Tối (Dark) ---
    public Color ThemeDarkCardBackground => IsDarkMode
        ? (IsDark ? Color.FromArgb("#1E1B4B") : Color.FromArgb("#EEF2FF"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color ThemeDarkCardBorder => IsDarkMode
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double ThemeDarkCardBorderThickness => IsDarkMode ? 1.5 : 1.0;

    public Color ThemeDarkTextColor => IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A");

    // --- Cỡ chữ: Nhỏ (Small) ---
    public Color FontSizeSmallBackground => IsSmallFont
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color FontSizeSmallTextColor => IsSmallFont
        ? Color.FromArgb("#FFFFFF")
        : (IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A"));

    public Color FontSizeSmallBorder => IsSmallFont
        ? Colors.Transparent
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double FontSizeSmallBorderThickness => IsSmallFont ? 0.0 : 1.0;

    // --- Cỡ chữ: Thường (Normal) ---
    public Color FontSizeNormalBackground => IsNormalFont
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color FontSizeNormalTextColor => IsNormalFont
        ? Color.FromArgb("#FFFFFF")
        : (IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A"));

    public Color FontSizeNormalBorder => IsNormalFont
        ? Colors.Transparent
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double FontSizeNormalBorderThickness => IsNormalFont ? 0.0 : 1.0;

    // --- Cỡ chữ: Lớn (Large) ---
    public Color FontSizeLargeBackground => IsLargeFont
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color FontSizeLargeTextColor => IsLargeFont
        ? Color.FromArgb("#FFFFFF")
        : (IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A"));

    public Color FontSizeLargeBorder => IsLargeFont
        ? Colors.Transparent
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double FontSizeLargeBorderThickness => IsLargeFont ? 0.0 : 1.0;

    // --- Ngôn ngữ: Tiếng Việt ---
    public Color LangVietnameseBackground => IsVietnamese
        ? (IsDark ? Color.FromArgb("#1E1B4B") : Color.FromArgb("#EEF2FF"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color LangVietnameseBorder => IsVietnamese
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double LangVietnameseBorderThickness => IsVietnamese ? 1.5 : 1.0;

    public Color LangVietnameseTextColor => IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A");

    // --- Ngôn ngữ: English ---
    public Color LangEnglishBackground => IsEnglish
        ? (IsDark ? Color.FromArgb("#1E1B4B") : Color.FromArgb("#EEF2FF"))
        : (IsDark ? Color.FromArgb("#1E293B") : Color.FromArgb("#FFFFFF"));

    public Color LangEnglishBorder => IsEnglish
        ? (IsDark ? Color.FromArgb("#6366F1") : Color.FromArgb("#4F46E5"))
        : (IsDark ? Color.FromArgb("#334155") : Color.FromArgb("#E2E8F0"));

    public double LangEnglishBorderThickness => IsEnglish ? 1.5 : 1.0;

    public Color LangEnglishTextColor => IsDark ? Color.FromArgb("#F8FAFC") : Color.FromArgb("#0F172A");

    #endregion

    #region Observable Properties - Chuỗi văn bản song ngữ

    public string SettingsTitle => _localizationService["SettingsTitle"];
    public string SettingsSubtitle => _localizationService["SettingsSubtitle"];
    public string AppSubtitle => _localizationService?["AppSubtitle"] ?? "Việc của tôi";

    public string SectionAppearance => _localizationService["SectionAppearance"];
    public string ThemeMode => _localizationService["ThemeMode"];
    public string ThemeModeDesc => _localizationService["ThemeModeDesc"];
    public string LightTheme => _localizationService["LightTheme"];
    public string DarkTheme => _localizationService["DarkTheme"];

    public string FontSize => _localizationService["FontSize"];
    public string FontSizeDesc => _localizationService["FontSizeDesc"];
    public string FontSizeSmall => _localizationService["FontSizeSmall"];
    public string FontSizeNormal => _localizationService["FontSizeNormal"];
    public string FontSizeLarge => _localizationService["FontSizeLarge"];

    public string SectionLanguage => _localizationService["SectionLanguage"];
    public string AppLanguageText => _localizationService["AppLanguage"];
    public string AppLanguageDesc => _localizationService["AppLanguageDesc"];
    public string LangVietnamese => _localizationService["LangVietnamese"];
    public string LangEnglish => _localizationService["LangEnglish"];

    public string SectionReviewerTools => _localizationService["SectionReviewerTools"];
    public string ReviewerToolsDesc => _localizationService["ReviewerToolsDesc"];
    public string StateLoading => _localizationService["StateLoading"];
    public string StateEmpty => _localizationService["StateEmpty"];
    public string StateNotFound => _localizationService["StateNotFound"];
    public string StateResetData => _localizationService["StateResetData"];
    public string ActionPreview => _localizationService["ActionPreview"];
    public string ActionReset => _localizationService["ActionReset"];

    public string SectionAppInfo => _localizationService["SectionAppInfo"];
    public string AppDescription => _localizationService["AppDescription"];
    public string AppVersion => _localizationService["AppVersion"];

    #endregion

    #region Commands

    public ICommand SelectThemeCommand { get; }
    public ICommand SelectFontSizeCommand { get; }
    public ICommand SelectLanguageCommand { get; }
    public ICommand SimulateLoadingCommand { get; }
    public ICommand SimulateEmptyCommand { get; }
    public ICommand SimulateNotFoundCommand { get; }
    public ICommand ResetMockDataCommand { get; }

    public ICommand TestSnackbarSuccessCommand { get; }
    public ICommand TestSnackbarDangerCommand { get; }
    public ICommand TestSnackbarWarningCommand { get; }
    public ICommand TestSnackbarUndoCommand { get; }

    #endregion

    #region Command Handlers

    public void ShowSnackbar(string message)
    {
        _snackbarCts?.Cancel();
        _snackbarCts = new CancellationTokenSource();
        var token = _snackbarCts.Token;

        SnackbarMessage = message;
        IsSnackbarVisible = true;
        _snackbarService.ShowSuccess(message);

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2500, token);
                if (!token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() => IsSnackbarVisible = false);
                }
            }
            catch (TaskCanceledException) { }
        });
    }

    private void OnTestSnackbarSuccess()
    {
        _snackbarService.ShowSuccess(_localizationService["SnackbarTestSuccess"]);
    }

    private void OnTestSnackbarDanger()
    {
        _snackbarService.ShowDanger(_localizationService["SnackbarTestDanger"]);
    }

    private void OnTestSnackbarWarning()
    {
        _snackbarService.ShowWarning(_localizationService["SnackbarTestWarning"]);
    }

    private void OnTestSnackbarUndo()
    {
        _snackbarService.ShowUndo(_localizationService["SnackbarTestUndo"], async () =>
        {
            await Task.CompletedTask;
            _snackbarService.ShowSuccess($"{_localizationService["SnackbarTaskRestored"]}Demo Task #1");
        });
    }

    private async void OnSelectTheme(string? theme)
    {
        var targetTheme = theme == "Dark" ? AppThemeMode.Dark : AppThemeMode.Light;
        if (_settingsService.CurrentTheme == targetTheme) return;

        // Hoãn 60ms để hiệu ứng chạm nảy OnCardTapped hoàn tất mượt mà trước khi áp dụng tài nguyên
        await Task.Delay(60);

        _settingsService.SetTheme(targetTheme);
        ShowSnackbar(targetTheme == AppThemeMode.Dark 
            ? _localizationService["FeedbackThemeDark"] 
            : _localizationService["FeedbackThemeLight"]);
    }

    private async void OnSelectFontSize(string? size)
    {
        var (targetSize, msgKey) = size switch
        {
            "Small" => (AppFontSize.Small, "FeedbackFontSmall"),
            "Large" => (AppFontSize.Large, "FeedbackFontLarge"),
            _ => (AppFontSize.Normal, "FeedbackFontNormal")
        };

        if (_settingsService.CurrentFontSize == targetSize) return;

        // Hoãn 60ms để hiệu ứng chạm nảy OnCardTapped hoàn tất mượt mà
        await Task.Delay(60);

        _settingsService.SetFontSize(targetSize);
        ShowSnackbar(_localizationService[msgKey]);
    }

    private async void OnSelectLanguage(string? lang)
    {
        var targetLang = lang == "English" ? AppLanguage.English : AppLanguage.Vietnamese;
        if (_settingsService.CurrentLanguage == targetLang) return;

        // Hoãn 60ms để hiệu ứng chạm nảy OnCardTapped hoàn tất mượt mà
        await Task.Delay(60);

        _settingsService.SetLanguage(targetLang);
        _localizationService.SetLanguage(targetLang);
        ShowSnackbar(targetLang == AppLanguage.English 
            ? _localizationService["FeedbackLangChangedEn"] 
            : _localizationService["FeedbackLangChangedVi"]);
    }

    private async Task OnSimulateLoadingAsync()
    {
        _taskService.SimulateLoading = true;
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync($"{nameof(MockStatePreviewPage)}?state=loading");
        }
    }

    private async Task OnSimulateEmptyAsync()
    {
        _taskService.SimulateEmpty = true;
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync($"{nameof(MockStatePreviewPage)}?state=empty");
        }
    }

    private async Task OnSimulateNotFoundAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync($"{nameof(MockStatePreviewPage)}?state=notfound");
        }
    }

    private async Task OnResetMockDataAsync()
    {
        var title = _localizationService["ConfirmResetTitle"];
        var message = _localizationService["ConfirmResetMessage"];
        var ok = _localizationService["ConfirmOk"];
        var cancel = _localizationService["ConfirmCancel"];

        bool confirm = false;
        if (Shell.Current != null)
        {
            confirm = await Shell.Current.DisplayAlertAsync(title, message, ok, cancel);
        }

        if (confirm)
        {
            await _taskService.ResetMockDataAsync();

            var success = _localizationService["ResetSuccess"];
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlertAsync(title, success, "OK");
            }
        }
    }

    #endregion

    #region Private Methods

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        SyncStateFromSettings();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        NotifyAllStringsChanged();
    }

    private void SyncStateFromSettings()
    {
        OnPropertyChanged(nameof(IsLightMode));
        OnPropertyChanged(nameof(IsDarkMode));
        OnPropertyChanged(nameof(IsSmallFont));
        OnPropertyChanged(nameof(IsNormalFont));
        OnPropertyChanged(nameof(IsLargeFont));
        OnPropertyChanged(nameof(IsVietnamese));
        OnPropertyChanged(nameof(IsEnglish));

        OnPropertyChanged(nameof(ThemeLightCardBackground));
        OnPropertyChanged(nameof(ThemeLightCardBorder));
        OnPropertyChanged(nameof(ThemeLightCardBorderThickness));
        OnPropertyChanged(nameof(ThemeLightTextColor));

        OnPropertyChanged(nameof(ThemeDarkCardBackground));
        OnPropertyChanged(nameof(ThemeDarkCardBorder));
        OnPropertyChanged(nameof(ThemeDarkCardBorderThickness));
        OnPropertyChanged(nameof(ThemeDarkTextColor));

        OnPropertyChanged(nameof(FontSizeSmallBackground));
        OnPropertyChanged(nameof(FontSizeSmallTextColor));
        OnPropertyChanged(nameof(FontSizeSmallBorder));
        OnPropertyChanged(nameof(FontSizeSmallBorderThickness));

        OnPropertyChanged(nameof(FontSizeNormalBackground));
        OnPropertyChanged(nameof(FontSizeNormalTextColor));
        OnPropertyChanged(nameof(FontSizeNormalBorder));
        OnPropertyChanged(nameof(FontSizeNormalBorderThickness));

        OnPropertyChanged(nameof(FontSizeLargeBackground));
        OnPropertyChanged(nameof(FontSizeLargeTextColor));
        OnPropertyChanged(nameof(FontSizeLargeBorder));
        OnPropertyChanged(nameof(FontSizeLargeBorderThickness));

        OnPropertyChanged(nameof(LangVietnameseBackground));
        OnPropertyChanged(nameof(LangVietnameseBorder));
        OnPropertyChanged(nameof(LangVietnameseBorderThickness));
        OnPropertyChanged(nameof(LangVietnameseTextColor));

        OnPropertyChanged(nameof(LangEnglishBackground));
        OnPropertyChanged(nameof(LangEnglishBorder));
        OnPropertyChanged(nameof(LangEnglishBorderThickness));
        OnPropertyChanged(nameof(LangEnglishTextColor));
    }

    private void NotifyAllStringsChanged()
    {
        OnPropertyChanged(nameof(AppSubtitle));
        OnPropertyChanged(nameof(SettingsTitle));
        OnPropertyChanged(nameof(SettingsSubtitle));
        OnPropertyChanged(nameof(SectionAppearance));
        OnPropertyChanged(nameof(ThemeMode));
        OnPropertyChanged(nameof(ThemeModeDesc));
        OnPropertyChanged(nameof(LightTheme));
        OnPropertyChanged(nameof(DarkTheme));
        OnPropertyChanged(nameof(FontSize));
        OnPropertyChanged(nameof(FontSizeDesc));
        OnPropertyChanged(nameof(FontSizeSmall));
        OnPropertyChanged(nameof(FontSizeNormal));
        OnPropertyChanged(nameof(FontSizeLarge));
        OnPropertyChanged(nameof(SectionLanguage));
        OnPropertyChanged(nameof(AppLanguageText));
        OnPropertyChanged(nameof(AppLanguageDesc));
        OnPropertyChanged(nameof(LangVietnamese));
        OnPropertyChanged(nameof(LangEnglish));
        OnPropertyChanged(nameof(SectionReviewerTools));
        OnPropertyChanged(nameof(ReviewerToolsDesc));
        OnPropertyChanged(nameof(StateLoading));
        OnPropertyChanged(nameof(StateEmpty));
        OnPropertyChanged(nameof(StateNotFound));
        OnPropertyChanged(nameof(StateResetData));
        OnPropertyChanged(nameof(ActionPreview));
        OnPropertyChanged(nameof(ActionReset));
        OnPropertyChanged(nameof(SectionAppInfo));
        OnPropertyChanged(nameof(AppDescription));
        OnPropertyChanged(nameof(AppVersion));
    }

    #endregion
}