using MyTasksApp.Core.Enums;

namespace MyTasksApp.Services.Interfaces;

/// <summary>
/// Giao diện quản lý cài đặt cá nhân hóa ứng dụng (Theme, Cỡ chữ, Ngôn ngữ)
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Chế độ giao diện hiện tại
    /// </summary>
    AppThemeMode CurrentTheme { get; }

    /// <summary>
    /// Cỡ chữ hiện tại
    /// </summary>
    AppFontSize CurrentFontSize { get; }

    /// <summary>
    /// Ngôn ngữ hiện tại
    /// </summary>
    AppLanguage CurrentLanguage { get; }

    /// <summary>
    /// Sự kiện thông báo khi cài đặt thay đổi
    /// </summary>
    event EventHandler? SettingsChanged;

    /// <summary>
    /// Áp dụng chế độ giao diện và lưu cấu hình
    /// </summary>
    void SetTheme(AppThemeMode theme);

    /// <summary>
    /// Áp dụng cỡ chữ và cập nhật ResourceDictionary toàn app
    /// </summary>
    void SetFontSize(AppFontSize fontSize);

    /// <summary>
    /// Áp dụng ngôn ngữ hiển thị
    /// </summary>
    void SetLanguage(AppLanguage language);

    /// <summary>
    /// Khởi tạo các cài đặt từ Preferences khi khởi động app
    /// </summary>
    void Initialize();
}
