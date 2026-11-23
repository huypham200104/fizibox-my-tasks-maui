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
		builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
		builder.Services.AddSingleton<ITaskService, LiteDbTaskService>();
		builder.Services.AddSingleton<ISettingsService, PreferencesSettingsService>();
		builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
		builder.Services.AddSingleton<ISnackbarService, SnackbarService>();

		// Đăng ký ViewModels
		builder.Services.AddSingleton<TodayViewModel>();
		builder.Services.AddSingleton<WeekViewModel>();
		builder.Services.AddSingleton<HistoryViewModel>();
		builder.Services.AddSingleton<SettingsViewModel>();
		builder.Services.AddTransient<TaskDetailViewModel>();

		// Đăng ký AppShell & Views (Singleton cho các tab chính giúp chuyển tab tức thì 0ms, không lag/khựng)
		builder.Services.AddSingleton<AppShell>();
		builder.Services.AddSingleton<TodayPage>();
		builder.Services.AddSingleton<WeekPage>();
		builder.Services.AddSingleton<HistoryPage>();
		builder.Services.AddSingleton<SettingsPage>();
		builder.Services.AddTransient<TaskDetailPage>();
		builder.Services.AddTransient<MockStatePreviewPage>();

		// Vô hiệu hóa phóng to kép (FontAutoScalingEnabled = false) trên toàn bộ hệ thống
		// Toàn quyền điều khiển cỡ chữ thông qua 3 mức trong Cài đặt (Nhỏ / Thường / Lớn)
		Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is Label label) label.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is Button button) button.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is Entry entry) entry.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is Editor editor) editor.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is SearchBar searchBar) searchBar.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is DatePicker datePicker) datePicker.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is TimePicker timePicker) timePicker.FontAutoScalingEnabled = false;
		});
		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("DisableFontAutoScaling", (h, v) =>
		{
			if (v is Picker picker) picker.FontAutoScalingEnabled = false;
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
