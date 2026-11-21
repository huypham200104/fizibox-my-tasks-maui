using MyTasksApp.Core.Enums;

namespace MyTasksApp.Services.Interfaces;

/// <summary>
/// Giao diện dịch vụ bản địa hóa ngôn ngữ (Song ngữ Tiếng Việt và Tiếng Anh)
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Ngôn ngữ hiện tại
    /// </summary>
    AppLanguage CurrentLanguage { get; }

    /// <summary>
    /// Sự kiện thông báo khi ngôn ngữ thay đổi
    /// </summary>
    event EventHandler? LanguageChanged;

    /// <summary>
    /// Lấy chuỗi bản dịch theo key
    /// </summary>
    string GetString(string key);

    /// <summary>
    /// Truy cập chuỗi bản dịch theo indexer
    /// </summary>
    string this[string key] { get; }

    /// <summary>
    /// Chuyển đổi ngôn ngữ
    /// </summary>
    void SetLanguage(AppLanguage language);
}
