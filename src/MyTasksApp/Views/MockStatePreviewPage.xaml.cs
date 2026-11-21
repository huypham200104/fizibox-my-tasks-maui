namespace MyTasksApp.Views;

[QueryProperty(nameof(State), "state")]
public partial class MockStatePreviewPage : ContentPage
{
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

    public MockStatePreviewPage()
    {
        InitializeComponent();
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
                PageTitleLabel.Text = "Kiểm thử: Danh sách trống";
                break;

            case "notfound":
                NotFoundView.IsVisible = true;
                PageTitleLabel.Text = "Kiểm thử: Không tìm thấy việc";
                break;

            case "loading":
            default:
                LoadingView.IsVisible = true;
                PageTitleLabel.Text = "Kiểm thử: Trạng thái đang tải";
                break;
        }
    }

    private async void OnBackButtonClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
