using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace MyTasksApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        RequestedOrientation = ScreenOrientation.Portrait;
        base.OnCreate(savedInstanceState);
        ApplyLightStatusBar();
    }

    protected override void OnResume()
    {
        base.OnResume();
        ApplyLightStatusBar();
    }

    private void ApplyLightStatusBar()
    {
        if (Window == null) return;

        var insetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
        if (insetsController != null)
        {
            // true = biểu tượng và chữ hệ thống màu tối (dành cho nền sáng), tránh bị lóa/chìm
            bool isDarkTheme = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark;
            insetsController.AppearanceLightStatusBars = !isDarkTheme;
        }
    }
}
