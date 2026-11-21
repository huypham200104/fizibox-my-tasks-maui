using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Controls;

public partial class FloatingSnackbarView : ContentView
{
    private ISnackbarService? _snackbarService;
    private ILocalizationService? _localizationService;
    private CancellationTokenSource? _dismissCts;
    private Func<Task>? _currentUndoAction;
    private readonly SemaphoreSlim _animationLock = new(1, 1);

    public FloatingSnackbarView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ResolveServices();
        if (_snackbarService != null)
        {
            _snackbarService.ShowRequested += OnShowRequested;
            _snackbarService.DismissRequested += OnDismissRequested;
        }

        if (_localizationService != null)
        {
            UpdateUndoButtonText();
            _localizationService.LanguageChanged += OnLanguageChanged;
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_snackbarService != null)
        {
            _snackbarService.ShowRequested -= OnShowRequested;
            _snackbarService.DismissRequested -= OnDismissRequested;
            _snackbarService = null;
        }

        if (_localizationService != null)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            _localizationService = null;
        }

        _dismissCts?.Cancel();
    }

    private void ResolveServices()
    {
        var services = Handler?.MauiContext?.Services
            ?? Application.Current?.Handler?.MauiContext?.Services
            ?? IPlatformApplication.Current?.Services;

        if (services != null)
        {
            _snackbarService = services.GetService<ISnackbarService>();
            _localizationService = services.GetService<ILocalizationService>();
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateUndoButtonText);
    }

    private void UpdateUndoButtonText()
    {
        if (_localizationService != null && !string.IsNullOrEmpty(_localizationService["SnackbarUndoButton"]))
        {
            UndoBtn.Text = _localizationService["SnackbarUndoButton"];
        }
    }

    private void OnShowRequested(object? sender, SnackbarModel model)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await ShowSnackbarAsync(model);
        });
    }

    private void OnDismissRequested(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            _dismissCts?.Cancel();
            await HideSnackbarAsync();
        });
    }

    private async Task ShowSnackbarAsync(SnackbarModel model)
    {
        _dismissCts?.Cancel();
        _dismissCts = new CancellationTokenSource();
        var token = _dismissCts.Token;

        _currentUndoAction = model.OnUndoAction;
        MessageLabel.Text = model.Message;

        // Cập nhật icon theo 4 kiểu của snack_bar.png
        IconSuccess.IsVisible = model.Type == SnackbarType.Success;
        IconDanger.IsVisible = model.Type == SnackbarType.Danger;
        IconWarning.IsVisible = model.Type == SnackbarType.Warning;
        IconUndo.IsVisible = model.Type == SnackbarType.Undo;

        // Chỉ hiển thị nút Hoàn tác khi kiểu là Undo và có callback action
        UndoBtn.IsVisible = model.Type == SnackbarType.Undo && model.OnUndoAction != null;

        await _animationLock.WaitAsync();
        try
        {
            this.IsVisible = true;
            if (!SnackbarBorder.IsVisible)
            {
                SnackbarBorder.TranslationY = 30;
                SnackbarBorder.Opacity = 0;
                SnackbarBorder.IsVisible = true;

                await Task.WhenAll(
                    SnackbarBorder.TranslateToAsync(0, 0, 250, Easing.CubicOut),
                    SnackbarBorder.FadeToAsync(1, 250, Easing.CubicOut)
                );
            }
            else
            {
                // Khi đang hiển thị mà có thông báo mới (Immediate Replacement):
                // Hiệu ứng nháy nhẹ (pulse) để báo hiệu nội dung mới cập nhật
                SnackbarBorder.TranslationY = 0;
                SnackbarBorder.Opacity = 1;
            }
        }
        finally
        {
            _animationLock.Release();
        }

        // Khởi động bộ hẹn giờ tự ẩn sau DurationMs (mặc định 5000ms)
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(model.DurationMs, token);
                if (!token.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await HideSnackbarAsync();
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // Bị hủy khi có thông báo mới hoặc người dùng đóng sớm
            }
        }, token);
    }

    private async Task HideSnackbarAsync()
    {
        await _animationLock.WaitAsync();
        try
        {
            if (SnackbarBorder.IsVisible)
            {
                await Task.WhenAll(
                    SnackbarBorder.TranslateToAsync(0, 20, 200, Easing.CubicIn),
                    SnackbarBorder.FadeToAsync(0, 200, Easing.CubicIn)
                );
                SnackbarBorder.IsVisible = false;
                this.IsVisible = false;
                _currentUndoAction = null;
            }
            else
            {
                this.IsVisible = false;
            }
        }
        finally
        {
            _animationLock.Release();
        }
    }

    private async void OnUndoClicked(object? sender, EventArgs e)
    {
        _dismissCts?.Cancel();
        var undoAction = _currentUndoAction;
        _currentUndoAction = null;

        await HideSnackbarAsync();

        if (undoAction != null)
        {
            try
            {
                await undoAction();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FloatingSnackbarView] Undo failed: {ex.Message}");
            }
        }
    }

    private async void OnDismissClicked(object? sender, EventArgs e)
    {
        _dismissCts?.Cancel();
        await HideSnackbarAsync();
        _snackbarService?.Dismiss();
    }

    private async void OnSwipeDownDismiss(object? sender, SwipedEventArgs e)
    {
        _dismissCts?.Cancel();
        await HideSnackbarAsync();
    }
}
