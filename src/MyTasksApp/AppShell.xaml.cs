using Microsoft.Maui.Controls;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.Views;

namespace MyTasksApp;

public partial class AppShell : Shell
{
    private readonly ILocalizationService _localizationService;

    public AppShell(IServiceProvider serviceProvider, ILocalizationService localizationService)
    {
        InitializeComponent();
        _localizationService = localizationService;

        // Đăng ký route điều hướng trang Chi tiết công việc và trang xem thử kiểm thử
        Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage));
        Routing.RegisterRoute(nameof(MockStatePreviewPage), typeof(MockStatePreviewPage));

        // Cập nhật tiêu đề tab theo ngôn ngữ hiện tại
        UpdateTabTitles();
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Eager Loading: Gán trực tiếp các Singleton Page vào ShellContent để chuyển tab tức thì 0ms
        TabTodayContent.Content = serviceProvider.GetRequiredService<TodayPage>();
        TabWeekContent.Content = serviceProvider.GetRequiredService<WeekPage>();
        TabHistoryContent.Content = serviceProvider.GetRequiredService<HistoryPage>();
        TabSettingsContent.Content = serviceProvider.GetRequiredService<SettingsPage>();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateTabTitles);
    }

    private void UpdateTabTitles()
    {
        TabTodayContent.Title = _localizationService["TabToday"];
        TabWeekContent.Title = _localizationService["TabWeek"];
        TabHistoryContent.Title = _localizationService["TabHistory"];
        TabSettingsContent.Title = _localizationService["TabSettings"];
    }
}
