using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyTasksApp.ViewModels.Base;

/// <summary>
/// Lớp cơ sở cung cấp cơ chế thông báo thay đổi dữ liệu thuần C# (INotifyPropertyChanged)
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Cập nhật giá trị và kích hoạt thông báo nếu giá trị mới khác giá trị cũ
    /// </summary>
    protected bool SetProperty<T>(
        ref T backingStore,
        T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Kích hoạt sự kiện PropertyChanged cho một hoặc nhiều thuộc tính
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
