using MyTasksApp.ViewModels.Base;

namespace MyTasksApp.Core.Models;

/// <summary>
/// Đại diện cho 1 ô ngày trên lưới lịch tháng (Calendar Grid) của DatePicker Sheet
/// </summary>
public class CalendarDayItem : ObservableObject
{
    private DateTime _date;
    private int _dayNumber;
    private bool _isCurrentMonth = true;
    private bool _isToday;
    private bool _isSelected;

    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public int DayNumber
    {
        get => _dayNumber;
        set
        {
            if (SetProperty(ref _dayNumber, value))
            {
                OnPropertyChanged(nameof(DayText));
            }
        }
    }

    public string DayText => _dayNumber > 0 ? _dayNumber.ToString() : string.Empty;

    public bool IsCurrentMonth
    {
        get => _isCurrentMonth;
        set => SetProperty(ref _isCurrentMonth, value);
    }

    public bool IsToday
    {
        get => _isToday;
        set => SetProperty(ref _isToday, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
