using MyTasksApp.Core.Enums;

namespace MyTasksApp.Core.Models;

/// <summary>
/// Dữ liệu thông báo SnackBar truyền qua sự kiện
/// </summary>
public class SnackbarModel
{
    public string Message { get; set; } = string.Empty;
    public SnackbarType Type { get; set; } = SnackbarType.Success;
    public Func<Task>? OnUndoAction { get; set; }
    public int DurationMs { get; set; } = 5000;
}
