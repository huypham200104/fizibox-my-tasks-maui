using Microsoft.Maui.Controls;
using MyTasksApp.Core.Enums;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Services;

/// <summary>
/// Quản lý cài đặt cấu hình người dùng qua MAUI Preferences
/// Cập nhật trực tiếp DynamicResource trong Application.Current.Resources giúp đổi theme và cỡ chữ tức thì không giật lag
/// </summary>
public class PreferencesSettingsService : ISettingsService
{
    private const string KeyTheme = "mytasks_app_theme";
    private const string KeyFontSize = "mytasks_app_font_size";
    private const string KeyLanguage = "mytasks_app_language";

    private readonly IPreferences? _preferences;
    private readonly ILocalizationService? _localizationService;
    private readonly Dictionary<string, object> _inMemoryFallback = new();

    private AppThemeMode _currentTheme = AppThemeMode.Light;
    private AppFontSize _currentFontSize = AppFontSize.Normal;
    private AppLanguage _currentLanguage = AppLanguage.Vietnamese;

    public AppThemeMode CurrentTheme => _currentTheme;
    public AppFontSize CurrentFontSize => _currentFontSize;
    public AppLanguage CurrentLanguage => _currentLanguage;

    public event EventHandler? SettingsChanged;

    public PreferencesSettingsService(
        IPreferences? preferences = null,
        ILocalizationService? localizationService = null)
    {
        _preferences = preferences;
        _localizationService = localizationService;

        // Khôi phục giá trị ngay lập tức khi Service được tạo bởi DI Container
        LoadPreferences();
    }

    private void LoadPreferences()
    {
        var savedTheme = GetPreference(KeyTheme, (int)AppThemeMode.Light);
        _currentTheme = Enum.IsDefined(typeof(AppThemeMode), savedTheme)
            ? (AppThemeMode)savedTheme
            : AppThemeMode.Light;

        var savedFontSize = GetPreference(KeyFontSize, (int)AppFontSize.Normal);
        _currentFontSize = Enum.IsDefined(typeof(AppFontSize), savedFontSize)
            ? (AppFontSize)savedFontSize
            : AppFontSize.Normal;

        var savedLanguage = GetPreference(KeyLanguage, (int)AppLanguage.Vietnamese);
        _currentLanguage = Enum.IsDefined(typeof(AppLanguage), savedLanguage)
            ? (AppLanguage)savedLanguage
            : AppLanguage.Vietnamese;
    }

    public void Initialize()
    {
        LoadPreferences();

        // Đồng bộ ngôn ngữ sang LocalizationService
        _localizationService?.SetLanguage(_currentLanguage);

        // Áp dụng lên Resources của ứng dụng
        ApplyThemeResources(_currentTheme);
        ApplyFontSizeResources(_currentFontSize);

        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetTheme(AppThemeMode theme)
    {
        _currentTheme = theme;
        SetPreference(KeyTheme, (int)theme);
        ApplyThemeResources(theme);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetFontSize(AppFontSize fontSize)
    {
        _currentFontSize = fontSize;
        SetPreference(KeyFontSize, (int)fontSize);
        ApplyFontSizeResources(fontSize);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetLanguage(AppLanguage language)
    {
        _currentLanguage = language;
        SetPreference(KeyLanguage, (int)language);
        _localizationService?.SetLanguage(language);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private T GetPreference<T>(string key, T defaultValue)
    {
        try
        {
            if (_preferences != null)
            {
                return _preferences.Get(key, defaultValue);
            }
            return Preferences.Default.Get(key, defaultValue);
        }
        catch
        {
            if (_inMemoryFallback.TryGetValue(key, out var val) && val is T typedVal)
            {
                return typedVal;
            }
            return defaultValue;
        }
    }

    private void SetPreference<T>(string key, T value)
    {
        try
        {
            if (_preferences != null)
            {
                Task.Run(() => _preferences.Set(key, value));
                return;
            }
            Task.Run(() => Preferences.Default.Set(key, value));
        }
        catch
        {
            _inMemoryFallback[key] = value!;
        }
    }

    private void ApplyThemeResources(AppThemeMode theme)
    {
        if (Application.Current == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var res = Application.Current.Resources;
            if (res == null) return;

            if (theme == AppThemeMode.Dark)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;

                // Cập nhật DynamicResource sang Dark Palette
                res["Background"] = Color.FromArgb("#0F172A");
                res["Surface"] = Color.FromArgb("#1E293B");
                res["SurfaceVariant"] = Color.FromArgb("#334155");
                res["BorderColor"] = Color.FromArgb("#334155");
                res["TextPrimary"] = Color.FromArgb("#F8FAFC");
                res["TextSecondary"] = Color.FromArgb("#94A3B8");
                res["TextMuted"] = Color.FromArgb("#64748B");
                res["Primary"] = Color.FromArgb("#6366F1");
                res["PrimaryVariant"] = Color.FromArgb("#818CF8");
                res["PrimaryLight"] = Color.FromArgb("#1E1B4B");
                res["PrimaryLightDark"] = Color.FromArgb("#1E1B4B");
                res["ProgressTrack"] = Color.FromArgb("#312E81");
                res["ProgressFill"] = Color.FromArgb("#6366F1");
                res["Danger"] = Color.FromArgb("#EF4444");
                res["DangerSurface"] = Color.FromArgb("#450A0A");
                res["Warning"] = Color.FromArgb("#F59E0B");
                res["WarningSurface"] = Color.FromArgb("#451A03");
                res["Success"] = Color.FromArgb("#22C55E");
                res["SuccessSurface"] = Color.FromArgb("#052E16");
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;

                // Cập nhật DynamicResource sang Light Palette
                res["Background"] = Color.FromArgb("#F8FAFC");
                res["Surface"] = Color.FromArgb("#FFFFFF");
                res["SurfaceVariant"] = Color.FromArgb("#F1F5F9");
                res["BorderColor"] = Color.FromArgb("#E2E8F0");
                res["TextPrimary"] = Color.FromArgb("#0F172A");
                res["TextSecondary"] = Color.FromArgb("#475569");
                res["TextMuted"] = Color.FromArgb("#94A3B8");
                res["Primary"] = Color.FromArgb("#4F46E5");
                res["PrimaryVariant"] = Color.FromArgb("#4338CA");
                res["PrimaryLight"] = Color.FromArgb("#EEF2FF");
                res["PrimaryLightDark"] = Color.FromArgb("#EEF2FF");
                res["ProgressTrack"] = Color.FromArgb("#E0E7FF");
                res["ProgressFill"] = Color.FromArgb("#4F46E5");
                res["Danger"] = Color.FromArgb("#DC2626");
                res["DangerSurface"] = Color.FromArgb("#FEF2F2");
                res["Warning"] = Color.FromArgb("#D97706");
                res["WarningSurface"] = Color.FromArgb("#FFFBEB");
                res["Success"] = Color.FromArgb("#16A34A");
                res["SuccessSurface"] = Color.FromArgb("#F0FDF4");
            }
        });
    }

    private void ApplyFontSizeResources(AppFontSize fontSize)
    {
        if (Application.Current == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            var res = Application.Current.Resources;
            if (res == null) return;

            switch (fontSize)
            {
                case AppFontSize.Small:
                    res["FontSizeDisplay"] = 22.0;
                    res["FontSizeTitle"] = 18.0;
                    res["FontSizeSubtitle"] = 14.0;
                    res["FontSizeBody"] = 13.0;
                    res["FontSizeCaption"] = 12.0;
                    res["FontSizeBadge"] = 11.0;
                    break;

                case AppFontSize.Large:
                    res["FontSizeDisplay"] = 30.0;
                    res["FontSizeTitle"] = 24.0;
                    res["FontSizeSubtitle"] = 19.0;
                    res["FontSizeBody"] = 17.0;
                    res["FontSizeCaption"] = 15.0;
                    res["FontSizeBadge"] = 13.0;
                    break;

                case AppFontSize.Normal:
                default:
                    res["FontSizeDisplay"] = 26.0;
                    res["FontSizeTitle"] = 20.0;
                    res["FontSizeSubtitle"] = 16.0;
                    res["FontSizeBody"] = 15.0;
                    res["FontSizeCaption"] = 13.0;
                    res["FontSizeBadge"] = 12.0;
                    break;
            }
        });
    }
}
