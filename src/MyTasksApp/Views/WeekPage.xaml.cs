using FluentIcons.Common;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class WeekPage : ContentPage
{
    private readonly WeekViewModel _viewModel;
    private CancellationTokenSource? _pulseAnimationCts;

    public WeekPage(WeekViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeekViewModel.IsCreateModalVisible))
        {
            if (_viewModel.IsCreateModalVisible)
            {
                // Ẩn thanh TabBar để không che modal tạo việc mới
                Shell.SetTabBarIsVisible(this, false);

                // Hiệu ứng Bottom Sheet trượt êm từ đáy màn hình lên
                CreateModalSheet.TranslationY = 320;
                CreateModalBackdrop.Opacity = 0;
                await Task.WhenAll(
                    CreateModalBackdrop.FadeToAsync(1, 220, Easing.CubicOut),
                    CreateModalSheet.TranslateToAsync(0, 0, 250, Easing.CubicOut)
                );
            }
            else
            {
                // Hiệu ứng trượt xuống và hiện lại TabBar
                await Task.WhenAll(
                    CreateModalBackdrop.FadeToAsync(0, 150, Easing.CubicIn),
                    CreateModalSheet.TranslateToAsync(0, 320, 150, Easing.CubicIn)
                );
                Shell.SetTabBarIsVisible(this, true);
            }
        }
        else if (e.PropertyName == nameof(WeekViewModel.IsBusy))
        {
            if (_viewModel.IsBusy)
            {
                StartSkeletonPulseAnimation();
            }
            else
            {
                StopSkeletonPulseAnimation();
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.IsPageVisible = true;

        if (_viewModel.IsBusy)
        {
            StartSkeletonPulseAnimation();
        }

        // Bảo toàn ngày chọn và chỉ tải dữ liệu nếu chưa load (Điểm tự quyết 3)
        await _viewModel.EnsureDataLoadedAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.IsPageVisible = false;
        StopSkeletonPulseAnimation();
    }

    private void StartSkeletonPulseAnimation()
    {
        StopSkeletonPulseAnimation();
        _pulseAnimationCts = new CancellationTokenSource();
        var token = _pulseAnimationCts.Token;

        _ = AnimateSkeletonPulseAsync(token);
    }

    private async Task AnimateSkeletonPulseAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && _viewModel.IsBusy && SkeletonContainer != null)
            {
                await SkeletonContainer.FadeToAsync(0.5, 600, Easing.SinInOut);
                if (token.IsCancellationRequested || !_viewModel.IsBusy) break;
                await SkeletonContainer.FadeToAsync(1.0, 600, Easing.SinInOut);
            }
        }
        catch
        {
            // Bỏ qua ngoại lệ hủy animation
        }
        finally
        {
            if (SkeletonContainer != null)
            {
                SkeletonContainer.Opacity = 1.0;
            }
        }
    }

    private void StopSkeletonPulseAnimation()
    {
        _pulseAnimationCts?.Cancel();
        _pulseAnimationCts?.Dispose();
        _pulseAnimationCts = null;

        if (SkeletonContainer != null)
        {
            SkeletonContainer.CancelAnimations();
            SkeletonContainer.Opacity = 1.0;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.IsCreateModalVisible)
        {
            _viewModel.CloseCreateModalCommand.Execute(null);
            return true;
        }
        return base.OnBackButtonPressed();
    }

    private async void OnTitleEntryCompleted(object? sender, EventArgs e)
    {
        NewTaskDescEditor.Focus();
        await Task.Delay(50);
        await CreateModalScrollView.ScrollToAsync(NewTaskDescEditor, ScrollToPosition.Center, animated: true);
    }

    private async void OnDescEditorCompleted(object? sender, EventArgs e)
    {
        NewTaskDescEditor.Unfocus();
        await Task.Delay(50);
        await CreateModalScrollView.ScrollToAsync(0, CreateModalScrollView.ContentSize.Height, animated: true);
    }

    private async void OnDescEditorTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue != null && e.NewTextValue.EndsWith('\n'))
        {
            NewTaskDescEditor.Text = e.NewTextValue.TrimEnd('\r', '\n');
            NewTaskDescEditor.Unfocus();
            await Task.Delay(50);
            await CreateModalScrollView.ScrollToAsync(0, CreateModalScrollView.ContentSize.Height, animated: true);
        }
    }

    private async void OnFieldFocused(object? sender, FocusEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await Task.Delay(150);
            await CreateModalScrollView.ScrollToAsync(element, ScrollToPosition.Center, animated: true);
        }
    }

    private async void OnTaskCardTapped(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement view)
        {
            await view.ScaleToAsync(0.97, 60, Easing.CubicOut);
            _ = view.ScaleToAsync(1.0, 100, Easing.CubicIn);
        }
    }
}
