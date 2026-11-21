using MyTasksApp.Services.Interfaces;

namespace MyTasksApp.Views;

[QueryProperty(nameof(State), "state")]
public partial class MockStatePreviewPage : ContentPage
{
    private readonly ITaskService? _taskService;
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

    public MockStatePreviewPage() : this(null)
    {
    }

    public MockStatePreviewPage(ITaskService? taskService)
    {
        InitializeComponent();
        _taskService = taskService;
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
                PageTitleLabel.Text = "Kiểm thử: Danh sách trống";
                break;

            case "notfound":
                NotFoundView.IsVisible = true;
                NormalHeaderTitleGroup.IsVisible = true;
                NormalHeaderBadge.IsVisible = true;
                SkeletonHeaderTitleGroup.IsVisible = false;
                SkeletonHeaderBadge.IsVisible = false;
                PageTitleLabel.Text = "Kiểm thử: Không tìm thấy việc";
                break;

            case "loading":
            default:
                LoadingView.IsVisible = true;
                NormalHeaderTitleGroup.IsVisible = false;
                NormalHeaderBadge.IsVisible = false;
                SkeletonHeaderTitleGroup.IsVisible = true;
                SkeletonHeaderBadge.IsVisible = true;
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
