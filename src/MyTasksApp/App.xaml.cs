using Microsoft.Extensions.DependencyInjection;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp;

public partial class App : Application
{
	private readonly AppShell _appShell;

	public App(ISettingsService settingsService, IServiceProvider serviceProvider)
	{
		InitializeComponent();
		settingsService.Initialize();
		_appShell = serviceProvider.GetRequiredService<AppShell>();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(_appShell);
	}
}
