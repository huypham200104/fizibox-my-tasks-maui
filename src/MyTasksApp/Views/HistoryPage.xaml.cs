using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;
    private CancellationTokenSource? _pulseAnimationCts;

    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HistoryViewModel.IsBusy))
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

    private async void OnTaskCardTapped(object? sender, TappedEventArgs e)
    {
        if (sender is VisualElement view)
        {
            await view.ScaleToAsync(0.97, 60, Easing.CubicOut);
            _ = view.ScaleToAsync(1.0, 100, Easing.CubicIn);
        }
    }
}
