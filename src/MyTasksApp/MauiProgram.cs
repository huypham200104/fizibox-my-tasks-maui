using FluentIcons.Maui;
using Microsoft.Extensions.Logging;
using MyTasksApp.Services;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.Views;

namespace MyTasksApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseFluentIcons()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Inter-Regular.ttf", "InterRegular");
				fonts.AddFont("Inter-Medium.ttf", "InterMedium");
				fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
				fonts.AddFont("Inter-Bold.ttf", "InterBold");
			});

		// Đăng ký Services
		builder.Services.AddSingleton<ITaskService, MockTaskService>();

		// Đăng ký Views
		builder.Services.AddTransient<TodayPage>();
		builder.Services.AddTransient<WeekPage>();
		builder.Services.AddTransient<HistoryPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<TaskDetailPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
