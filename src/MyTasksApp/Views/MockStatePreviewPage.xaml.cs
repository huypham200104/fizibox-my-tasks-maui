using Microsoft.Extensions.DependencyInjection;
using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Views;

[QueryProperty(nameof(State), "state")]
public partial class MockStatePreviewPage : ContentPage
{
    private readonly ITaskService? _taskService;
    private readonly ILocalizationService? _localizationService;
    private string _state = "loading";

    public string State
    {
        get => _state;
        set
        {
            _state = value;
            UpdateViewForState(value);
        }
    }

    public MockStatePreviewPage() : this(null, null)
    {
    }

    public MockStatePreviewPage(ITaskService? taskService, ILocalizationService? localizationService = null)
    {
        InitializeComponent();
        _taskService = taskService;
        _localizationService = localizationService ?? Application.Current?.Handler?.MauiContext?.Services.GetService<ILocalizationService>();
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        if (_localizationService == null) return;
        if (PageSubtitleLabel != null)
            PageSubtitleLabel.Text = _localizationService["MockTestingSubtitle"];
        if (NotFoundTitleLabel != null)
            NotFoundTitleLabel.Text = _localizationService["NotFoundDetailTitle"];
        if (NotFoundDescLabel != null)
            NotFoundDescLabel.Text = _localizationService["NotFoundDetailDesc"];
        if (BackToSettingsButton != null)
            BackToSettingsButton.Text = _localizationService["BtnBackToSettings"];
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ResetSimulationFlags();
    }

    private void UpdateViewForState(string state)
    {
        LoadingView.IsVisible = false;
        EmptyView.IsVisible = false;
        NotFoundView.IsVisible = false;

        switch (state.ToLowerInvariant())
        {
            case "empty":
                EmptyView.IsVisible = true;
                NormalHeaderTitleGroup.IsVisible = true;
                NormalHeaderBadge.IsVisible = true;
                SkeletonHeaderTitleGroup.IsVisible = false;
                SkeletonHeaderBadge.IsVisible = false;
                PageTitleLabel.Text = _localizationService?["MockStateEmptyTitle"] ?? "Kiểm thử: Danh sách trống";
                break;

            case "notfound":
                NotFoundView.IsVisible = true;
                NormalHeaderTitleGroup.IsVisible = true;
                NormalHeaderBadge.IsVisible = true;
                SkeletonHeaderTitleGroup.IsVisible = false;
                SkeletonHeaderBadge.IsVisible = false;
                PageTitleLabel.Text = _localizationService?["MockStateNotFoundTitle"] ?? "Kiểm thử: Không tìm thấy việc";
                break;

            case "loading":
            default:
                LoadingView.IsVisible = true;
                NormalHeaderTitleGroup.IsVisible = false;
                NormalHeaderBadge.IsVisible = false;
                SkeletonHeaderTitleGroup.IsVisible = true;
                SkeletonHeaderBadge.IsVisible = true;
                PageTitleLabel.Text = _localizationService?["MockStateLoadingTitle"] ?? "Kiểm thử: Đang tải";
                break;
        }
    }

    private async void OnBackButtonClicked(object? sender, EventArgs e)
    {
        ResetSimulationFlags();
        await Shell.Current.GoToAsync("..");
    }

    private void ResetSimulationFlags()
    {
        if (_taskService != null)
        {
            _taskService.SimulateEmpty = false;
            _taskService.SimulateLoading = false;
        }
    }
}
