using FluentIcons.Common;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class TodayPage : ContentPage
{
    private readonly TodayViewModel _viewModel;
    private bool _isAtBottomOrScrolled;

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
    }

    private void OnMainScrollViewScrolled(object? sender, ScrolledEventArgs e)
    {
        // Nhận biết vị trí cuộn: Nếu cuộn quá 150px thì đổi sang mũi tên lên, ngược lại là mũi tên xuống
        if (e.ScrollY > 150)
        {
            if (!_isAtBottomOrScrolled)
            {
                _isAtBottomOrScrolled = true;
                ScrollIcon.Symbol = Symbol.ArrowUp;
            }
        }
        else
        {
            if (_isAtBottomOrScrolled)
            {
                _isAtBottomOrScrolled = false;
                ScrollIcon.Symbol = Symbol.ArrowDown;
            }
        }
    }

    private async void OnQuickScrollTapped(object? sender, TappedEventArgs e)
    {
        if (_isAtBottomOrScrolled)
        {
            // Cuộn mượt về đầu trang
            await MainScrollView.ScrollToAsync(0, 0, animated: true);
        }
        else
        {
            // Cuộn mượt xuống cuối danh sách
            await MainScrollView.ScrollToAsync(0, MainScrollView.ContentSize.Height, animated: true);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Chỉ tải dữ liệu ban đầu 1 lần, chuyển tab không bị load lại gây lag/khựng nhịp (Điểm 6)
        await _viewModel.EnsureDataLoadedAsync();
    }
}
