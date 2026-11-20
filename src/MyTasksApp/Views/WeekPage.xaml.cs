using FluentIcons.Common;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class WeekPage : ContentPage
{
    private readonly WeekViewModel _viewModel;

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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Bảo toàn ngày chọn và chỉ tải dữ liệu nếu chưa load (Điểm tự quyết 3)
        await _viewModel.EnsureDataLoadedAsync();
    }
}
