using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Views;

public partial class HistoryPage : ContentPage
{
    private readonly ILocalizationService _localizationService;

    public HistoryPage(ILocalizationService localizationService)
    {
        InitializeComponent();
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;
        UpdateLocalization();
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(UpdateLocalization);
    }

    private void UpdateLocalization()
    {
        Title = _localizationService["HistoryTitle"];
        PageTitleLabel.Text = _localizationService["HistoryTitle"];
        AppSubtitleLabel.Text = _localizationService["AppSubtitle"];
        CompletedBadgeLabel.Text = _localizationService["HistoryCompletedBadge"];
        EmptyState.Title = _localizationService["HistoryEmptyMessage"];
        EmptyState.Subtitle = _localizationService["HistoryEmptySubMessage"];
    }
}
