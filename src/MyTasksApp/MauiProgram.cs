using FluentIcons.Maui;
using Microsoft.Extensions.Logging;
using MyTasksApp.Services;
using MyTasksApp.Services.Interfaces;
using MyTasksApp.ViewModels;
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

		// Đăng ký Services (Sử dụng NoSQL LiteDB lưu trữ dữ liệu vĩnh viễn và đồng bộ realtime)
		builder.Services.AddSingleton<ITaskService, LiteDbTaskService>();

		// Đăng ký ViewModels
		builder.Services.AddSingleton<TodayViewModel>();
		builder.Services.AddSingleton<WeekViewModel>();

		// Đăng ký Views (Singleton cho các tab chính giúp chuyển tab tức thì 0ms, không lag/khựng)
		builder.Services.AddSingleton<TodayPage>();
		builder.Services.AddSingleton<WeekPage>();
		builder.Services.AddSingleton<HistoryPage>();
		builder.Services.AddSingleton<SettingsPage>();
		builder.Services.AddTransient<TaskDetailPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
