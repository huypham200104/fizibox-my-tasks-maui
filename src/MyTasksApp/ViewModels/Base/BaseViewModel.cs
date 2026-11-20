namespace MyTasksApp.ViewModels.Base;

/// <summary>
/// ViewModel cơ sở cho toàn bộ các màn hình
/// </summary>
public abstract class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string _title = string.Empty;

    /// <summary>
    /// Trạng thái bận / đang tải dữ liệu
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }

    /// <summary>
    /// Trạng thái rảnh (ngược lại của IsBusy)
    /// </summary>
    public bool IsNotBusy => !_isBusy;

    /// <summary>
    /// Tiêu đề trang / màn hình
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
