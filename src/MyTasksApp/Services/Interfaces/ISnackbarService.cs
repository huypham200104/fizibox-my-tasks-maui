using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;

namespace MyTasksApp.Services.Interfaces;

/// <summary>
/// Giao diện dịch vụ thông báo nổi Floating SnackBar tập trung toàn ứng dụng
/// </summary>
public interface ISnackbarService
{
    /// <summary>
    /// Sự kiện yêu cầu hiển thị SnackBar mới
    /// </summary>
    event EventHandler<SnackbarModel>? ShowRequested;

    /// <summary>
    /// Sự kiện yêu cầu đóng SnackBar hiện tại
    /// </summary>
    event EventHandler? DismissRequested;

    /// <summary>
    /// Hiển thị thông báo SnackBar tùy biến
    /// </summary>
    void Show(string message, SnackbarType type = SnackbarType.Success, Func<Task>? onUndo = null, int durationMs = 5000);

    /// <summary>
    /// Hiển thị thông báo Thành công (xanh)
    /// </summary>
    void ShowSuccess(string message, int durationMs = 5000);

    /// <summary>
    /// Hiển thị thông báo Lỗi / Nguy hiểm (đỏ)
    /// </summary>
    void ShowDanger(string message, int durationMs = 5000);

    /// <summary>
    /// Hiển thị thông báo Cảnh báo (vàng)
    /// </summary>
    void ShowWarning(string message, int durationMs = 5000);

    /// <summary>
    /// Hiển thị thông báo Hoàn tác khi xóa việc (icon thùng rác + nút Hoàn tác)
    /// </summary>
    void ShowUndo(string message, Func<Task> onUndo, int durationMs = 5000);

    /// <summary>
    /// Đóng thanh thông báo ngay lập tức
    /// </summary>
    void Dismiss();
}
