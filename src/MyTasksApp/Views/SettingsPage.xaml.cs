using Microsoft.Maui.Devices;
using MyTasksApp.ViewModels;

namespace MyTasksApp.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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
