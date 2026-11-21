using Microsoft.Maui.Devices;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;
    private CancellationTokenSource? _pulseAnimationCts;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsViewModel.IsBusy))
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

        await _viewModel.EnsurePageLoadedAsync();
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

    private async void OnCardTapped(object? sender, EventArgs e)
    {
        if (sender is VisualElement visual)
        {
            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
            catch
            {
                // Haptic feedback có thể không hỗ trợ trên simulator hoặc một số thiết bị
            }

            // Hiệu ứng chạm nảy nhẹ (Micro-interaction bounce)
            await visual.ScaleToAsync(0.95, 50, Easing.CubicOut);
            await visual.ScaleToAsync(1.0, 50, Easing.CubicIn);
        }
    }
}
