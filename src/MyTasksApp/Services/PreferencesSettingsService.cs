using Microsoft.Maui.Controls;
using MyTasksApp.Core.DesignTokens;
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

    private static ResourceDictionary? _activeThemeDictionary;

    public void SetTheme(AppThemeMode theme)
    {
        if (_currentTheme == theme) return;

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

            var targetTheme = theme == AppThemeMode.Dark ? AppTheme.Dark : AppTheme.Light;
            if (Application.Current.UserAppTheme != targetTheme)
            {
                Application.Current.UserAppTheme = targetTheme;
            }

            var newDict = new ResourceDictionary();
            if (theme == AppThemeMode.Dark)
            {
                // Cập nhật DynamicResource sang Dark Palette từ nguồn AppColors (Single Source of Truth)
                newDict["Background"] = AppColors.BackgroundDark;
                newDict["Surface"] = AppColors.SurfaceDark;
                newDict["SurfaceVariant"] = AppColors.SurfaceVariantDark;
                newDict["BorderColor"] = AppColors.BorderColorDark;
                newDict["TextPrimary"] = AppColors.TextPrimaryDark;
                newDict["TextSecondary"] = AppColors.TextSecondaryDark;
                newDict["TextMuted"] = AppColors.TextMutedDark;
                newDict["Primary"] = AppColors.PrimaryDark;
                newDict["PrimaryVariant"] = AppColors.PrimaryVariantDark;
                newDict["PrimaryLight"] = AppColors.PrimaryLightDark;
                newDict["PrimaryLightDark"] = AppColors.PrimaryLightDark;
                newDict["ProgressTrack"] = AppColors.ProgressTrackDark;
                newDict["ProgressFill"] = AppColors.ProgressFillDark;
                newDict["Danger"] = AppColors.DangerDark;
                newDict["DangerSurface"] = AppColors.DangerSurfaceDark;
                newDict["Warning"] = AppColors.WarningDark;
                newDict["WarningSurface"] = AppColors.WarningSurfaceDark;
                newDict["Success"] = AppColors.SuccessDark;
                newDict["SuccessSurface"] = AppColors.SuccessSurfaceDark;
                newDict["SnackbarBg"] = AppColors.SnackbarBgDark;
                newDict["SnackbarUndoBg"] = AppColors.SnackbarUndoBgDark;
                newDict["SnackbarUndoText"] = AppColors.SnackbarUndoTextDark;
            }
            else
            {
                // Cập nhật DynamicResource sang Light Palette từ nguồn AppColors (Single Source of Truth)
                newDict["Background"] = AppColors.Background;
                newDict["Surface"] = AppColors.Surface;
                newDict["SurfaceVariant"] = AppColors.SurfaceVariant;
                newDict["BorderColor"] = AppColors.BorderColor;
                newDict["TextPrimary"] = AppColors.TextPrimary;
                newDict["TextSecondary"] = AppColors.TextSecondary;
                newDict["TextMuted"] = AppColors.TextMuted;
                newDict["Primary"] = AppColors.Primary;
                newDict["PrimaryVariant"] = AppColors.PrimaryVariant;
                newDict["PrimaryLight"] = AppColors.PrimaryLight;
                newDict["PrimaryLightDark"] = AppColors.PrimaryLight;
                newDict["ProgressTrack"] = AppColors.ProgressTrack;
                newDict["ProgressFill"] = AppColors.ProgressFill;
                newDict["Danger"] = AppColors.Danger;
                newDict["DangerSurface"] = AppColors.DangerSurface;
                newDict["Warning"] = AppColors.Warning;
                newDict["WarningSurface"] = AppColors.WarningSurface;
                newDict["Success"] = AppColors.Success;
                newDict["SuccessSurface"] = AppColors.SuccessSurface;
                newDict["SnackbarBg"] = AppColors.SnackbarBg;
                newDict["SnackbarUndoBg"] = AppColors.SnackbarUndoBg;
                newDict["SnackbarUndoText"] = AppColors.SnackbarUndoText;
            }

            // Gộp tất cả token vào ResourceDictionary riêng biệt và hoán đổi trong MergedDictionaries 
            // để kích hoạt làm mới giao diện trong 1 lượt duy nhất thay vì 22 lần riêng lẻ
            if (_activeThemeDictionary != null)
            {
                res.MergedDictionaries.Remove(_activeThemeDictionary);
            }
            _activeThemeDictionary = newDict;
            res.MergedDictionaries.Add(_activeThemeDictionary);

            // Xóa các key bị lưu đè ở root dictionary nếu có từ trước
            foreach (var key in newDict.Keys)
            {
                if (key is string strKey && res.ContainsKey(strKey))
                {
                    try { res.Remove(strKey); } catch { }
                }
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
