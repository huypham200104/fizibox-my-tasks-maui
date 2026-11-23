using FluentAssertions;
using MyTasksApp.Core.Enums;
using MyTasksApp.Services;
using MyTasksApp.Tests.Mocks;
using MyTasksApp.ViewModels;
using MyTasksApp.ViewModels.Base;
using Xunit;

namespace MyTasksApp.Tests;

public class SettingsViewModelTests
{
    private readonly MockTaskService _taskService;
    private readonly MockSettingsService _settingsService;
    private readonly LocalizationService _localizationService;
    private readonly SettingsViewModel _viewModel;

    public SettingsViewModelTests()
    {
        _taskService = new MockTaskService();
        _settingsService = new MockSettingsService();
        _localizationService = new LocalizationService();

        // Đảm bảo trạng thái ban đầu sạch
        _settingsService.SetTheme(AppThemeMode.Light);
        _settingsService.SetFontSize(AppFontSize.Normal);
        _settingsService.SetLanguage(AppLanguage.Vietnamese);
        _localizationService.SetLanguage(AppLanguage.Vietnamese);

        _viewModel = new SettingsViewModel(_settingsService, _localizationService, _taskService);
    }

    [Fact]
    public void InitialState_ShouldMatchDefaultSettings()
    {
        _viewModel.IsLightMode.Should().BeTrue();
        _viewModel.IsDarkMode.Should().BeFalse();
        _viewModel.IsNormalFont.Should().BeTrue();
        _viewModel.IsSmallFont.Should().BeFalse();
        _viewModel.IsLargeFont.Should().BeFalse();
        _viewModel.IsVietnamese.Should().BeTrue();
        _viewModel.IsEnglish.Should().BeFalse();
        _viewModel.SettingsTitle.Should().Be("Cài đặt");
        _viewModel.SettingsSubtitle.Should().Be("Cá nhân hóa trải nghiệm của bạn");
    }

    [Fact]
    public void SelectThemeCommand_Dark_ShouldUpdateStateToDark()
    {
        // Act
        _viewModel.SelectThemeCommand.Execute("Dark");

        // Assert
        _viewModel.IsDarkMode.Should().BeTrue();
        _viewModel.IsLightMode.Should().BeFalse();
        _settingsService.CurrentTheme.Should().Be(AppThemeMode.Dark);

        // Switch back to Light
        _viewModel.SelectThemeCommand.Execute("Light");
        _viewModel.IsLightMode.Should().BeTrue();
        _viewModel.IsDarkMode.Should().BeFalse();
        _settingsService.CurrentTheme.Should().Be(AppThemeMode.Light);
    }

    [Fact]
    public void SelectFontSizeCommand_ShouldUpdateFontSizeFlags()
    {
        // Act Small
        _viewModel.SelectFontSizeCommand.Execute("Small");
        _viewModel.IsSmallFont.Should().BeTrue();
        _viewModel.IsNormalFont.Should().BeFalse();
        _viewModel.IsLargeFont.Should().BeFalse();
        _settingsService.CurrentFontSize.Should().Be(AppFontSize.Small);

        // Act Large
        _viewModel.SelectFontSizeCommand.Execute("Large");
        _viewModel.IsLargeFont.Should().BeTrue();
        _viewModel.IsSmallFont.Should().BeFalse();
        _viewModel.IsNormalFont.Should().BeFalse();
        _settingsService.CurrentFontSize.Should().Be(AppFontSize.Large);

        // Act Normal
        _viewModel.SelectFontSizeCommand.Execute("Normal");
        _viewModel.IsNormalFont.Should().BeTrue();
        _settingsService.CurrentFontSize.Should().Be(AppFontSize.Normal);
    }

    [Fact]
    public void SelectLanguageCommand_English_ShouldUpdateStrings()
    {
        // Act
        _viewModel.SelectLanguageCommand.Execute("English");

        // Assert
        _viewModel.IsEnglish.Should().BeTrue();
        _viewModel.IsVietnamese.Should().BeFalse();
        _viewModel.SettingsTitle.Should().Be("Settings");
        _viewModel.SettingsSubtitle.Should().Be("Personalize your experience");
        _viewModel.LightTheme.Should().Be("Light");
        _viewModel.DarkTheme.Should().Be("Dark");
        _viewModel.ActionPreview.Should().Be("Preview");

        // Revert to Vietnamese
        _viewModel.SelectLanguageCommand.Execute("Vietnamese");
        _viewModel.IsVietnamese.Should().BeTrue();
        _viewModel.SettingsTitle.Should().Be("Cài đặt");
    }

    [Fact]
    public async Task SimulateLoadingCommand_ShouldSetSimulateLoadingFlag()
    {
        // Act
        _taskService.SimulateLoading = false;
        var command = (AsyncRelayCommand)_viewModel.SimulateLoadingCommand;
        await command.ExecuteAsync();

        // Assert
        _taskService.SimulateLoading.Should().BeTrue();
    }

    [Fact]
    public async Task SimulateEmptyCommand_ShouldSetSimulateEmptyFlag()
    {
        // Act
        _taskService.SimulateEmpty = false;
        var command = (AsyncRelayCommand)_viewModel.SimulateEmptyCommand;
        await command.ExecuteAsync();

        // Assert
        _taskService.SimulateEmpty.Should().BeTrue();
    }
}
