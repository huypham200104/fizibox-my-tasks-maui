namespace MyTasksApp.Core.Enums;

/// <summary>
/// Các kiểu thông báo nổi SnackBar theo thiết kế ui_design/snack_bar.png
/// </summary>
public enum SnackbarType
{
    /// <summary>
    /// Thông báo thành công (Icon xanh lá CheckmarkCircle)
    /// </summary>
    Success,

    /// <summary>
    /// Thông báo lỗi (Icon đỏ ErrorCircle)
    /// </summary>
    Danger,

    /// <summary>
    /// Thông báo cảnh báo (Icon vàng Warning)
    /// </summary>
    Warning,

    /// <summary>
    /// Thông báo hoàn tác khi xóa (Icon thùng rác Delete + nút Hoàn tác)
    /// </summary>
    Undo
}
