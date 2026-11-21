using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Services;

/// <summary>
/// Dịch vụ quản lý hiển thị Floating SnackBar tập trung toàn ứng dụng
/// Hỗ trợ 4 kiểu thông báo, thay thế tức thì (Immediate Replacement) và callback Hoàn tác
/// </summary>
public class SnackbarService : ISnackbarService
{
    public event EventHandler<SnackbarModel>? ShowRequested;
    public event EventHandler? DismissRequested;

    public void Show(string message, SnackbarType type = SnackbarType.Success, Func<Task>? onUndo = null, int durationMs = 5000)
    {
        var model = new SnackbarModel
        {
            Message = message,
            Type = type,
            OnUndoAction = onUndo,
            DurationMs = durationMs
        };

        RunOnMainThread(() => ShowRequested?.Invoke(this, model));
    }

    public void ShowSuccess(string message, int durationMs = 5000)
    {
        Show(message, SnackbarType.Success, null, durationMs);
    }

    public void ShowDanger(string message, int durationMs = 5000)
    {
        Show(message, SnackbarType.Danger, null, durationMs);
    }

    public void ShowWarning(string message, int durationMs = 5000)
    {
        Show(message, SnackbarType.Warning, null, durationMs);
    }

    public void ShowUndo(string message, Func<Task> onUndo, int durationMs = 5000)
    {
        Show(message, SnackbarType.Undo, onUndo, durationMs);
    }

    public void Dismiss()
    {
        RunOnMainThread(() => DismissRequested?.Invoke(this, EventArgs.Empty));
    }

    private static void RunOnMainThread(Action action)
    {
        try
        {
            if (MainThread.IsMainThread)
            {
                action();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(action);
            }
        }
        catch
        {
            // Trong môi trường Unit Tests hoặc background thread không có UI dispatcher
            action();
        }
    }
}
