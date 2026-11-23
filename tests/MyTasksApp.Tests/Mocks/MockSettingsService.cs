using MyTasksApp.Core.Enums;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Tests.Mocks;

/// <summary>
/// Mock ISettingsService lưu trạng thái In-memory hoàn toàn tách biệt khỏi MAUI Preferences.
/// </summary>
public class MockSettingsService : ISettingsService
{
    public AppThemeMode CurrentTheme { get; private set; } = AppThemeMode.Light;
    public AppFontSize CurrentFontSize { get; private set; } = AppFontSize.Normal;
    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.Vietnamese;

    public event EventHandler? SettingsChanged;

    public void SetTheme(AppThemeMode theme)
    {
        if (CurrentTheme == theme) return;
        CurrentTheme = theme;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetFontSize(AppFontSize fontSize)
    {
        if (CurrentFontSize == fontSize) return;
        CurrentFontSize = fontSize;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetLanguage(AppLanguage language)
    {
        if (CurrentLanguage == language) return;
        CurrentLanguage = language;
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Initialize()
    {
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
}
