using FluentIcons.Common;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class TodayPage : ContentPage
{
    private readonly TodayViewModel _viewModel;
    private bool _isUpButtonVisible;
    private bool _isDownButtonVisible;

    public TodayPage(TodayViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodayViewModel.IsCreateModalVisible))
        {
            if (_viewModel.IsCreateModalVisible)
            {
                // Ẩn thanh TabBar để không che/vướng modal tạo việc mới (Điểm 5)
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
        else if (e.PropertyName == nameof(TodayViewModel.IsEmpty) ||
                 e.PropertyName == nameof(TodayViewModel.IsBusy) ||
                 e.PropertyName == nameof(TodayViewModel.TotalTasksCount) ||
                 e.PropertyName == nameof(TodayViewModel.HasOverdueTasks) ||
                 e.PropertyName == nameof(TodayViewModel.HasTodayTasks) ||
                 e.PropertyName == nameof(TodayViewModel.HasCompletedTasks))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(60);
                UpdateScrollButtonsVisibility();
            });
        }
    }

    private void OnMainScrollViewScrolled(object? sender, ScrolledEventArgs e)
    {
        UpdateScrollButtonsVisibility(e.ScrollY);
    }

    private void OnMainScrollViewSizeChanged(object? sender, EventArgs e)
    {
        UpdateScrollButtonsVisibility();
    }

    private void UpdateScrollButtonsVisibility(double? scrollY = null)
    {
        double currentY = Math.Max(0, scrollY ?? MainScrollView.ScrollY);
        double viewportHeight = MainScrollView.Height;
        double contentHeight = MainScrollView.ContentSize.Height;
        double maxScroll = Math.Max(0, contentHeight - viewportHeight);

        // 1. Nếu danh sách task trống HOẶC đang loading HOẶC nội dung ngắn nằm vừa trọn màn hình (không thể cuộn)
        if (_viewModel.IsEmpty || _viewModel.IsBusy || viewportHeight <= 0 || maxScroll < 50)
        {
            SetButtonVisibility(ScrollUpButton, false, isUpButton: true);
            SetButtonVisibility(ScrollDownButton, false, isUpButton: false);
            return;
        }

        const double threshold = 40.0;
        bool isAtTop = currentY <= threshold;
        bool isAtBottom = currentY >= maxScroll - threshold;

        if (isAtTop)
        {
            // Đang ở đầu trang: chỉ hiển thị nút Xuống
            SetButtonVisibility(ScrollUpButton, false, isUpButton: true);
            SetButtonVisibility(ScrollDownButton, true, isUpButton: false);
        }
        else if (isAtBottom)
        {
            // Đang ở cuối trang: chỉ hiển thị nút Lên
            SetButtonVisibility(ScrollUpButton, true, isUpButton: true);
            SetButtonVisibility(ScrollDownButton, false, isUpButton: false);
        }
        else
        {
            // Ở giữa: hiển thị cả 2 nút (Lên và Xuống)
            SetButtonVisibility(ScrollUpButton, true, isUpButton: true);
            SetButtonVisibility(ScrollDownButton, true, isUpButton: false);
        }
    }

    private void SetButtonVisibility(VisualElement button, bool targetVisible, bool isUpButton)
    {
        bool currentState = isUpButton ? _isUpButtonVisible : _isDownButtonVisible;
        if (currentState == targetVisible)
            return;

        if (isUpButton)
            _isUpButtonVisible = targetVisible;
        else
            _isDownButtonVisible = targetVisible;

        button.CancelAnimations();

        if (targetVisible)
        {
            button.Opacity = 0;
            button.IsVisible = true;
            _ = button.FadeToAsync(1, 150, Easing.CubicOut);
        }
        else
        {
            _ = AnimateHideAsync(button, isUpButton);
        }
    }

    private async Task AnimateHideAsync(VisualElement button, bool isUpButton)
    {
        await button.FadeToAsync(0, 150, Easing.CubicIn);
        bool currentState = isUpButton ? _isUpButtonVisible : _isDownButtonVisible;
        if (!currentState)
        {
            button.IsVisible = false;
        }
    }

    private async void OnScrollUpTapped(object? sender, TappedEventArgs e)
    {
        await MainScrollView.ScrollToAsync(0, 0, animated: true);
    }

    private async void OnScrollDownTapped(object? sender, TappedEventArgs e)
    {
        await MainScrollView.ScrollToAsync(0, MainScrollView.ContentSize.Height, animated: true);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Chỉ tải dữ liệu ban đầu 1 lần, chuyển tab không bị load lại gây lag/khựng nhịp (Điểm 6)
        await _viewModel.EnsureDataLoadedAsync();
        await Task.Delay(100);
        UpdateScrollButtonsVisibility();
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
        NewTaskProjectEntry.Focus();
        await Task.Delay(50);
        await CreateModalScrollView.ScrollToAsync(NewTaskProjectEntry, ScrollToPosition.Center, animated: true);
    }

    private async void OnProjectEntryCompleted(object? sender, EventArgs e)
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
}
