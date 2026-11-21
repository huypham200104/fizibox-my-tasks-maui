using MyTasksApp.ViewModels.Base;

namespace MyTasksApp.Core.Models;

/// <summary>
/// Đại diện cho 1 ngày trong thanh dải 7 ngày của Tuần (Week Strip)
/// </summary>
public class WeekDayItem : ObservableObject
{
    private DateTime _date;
    private string _dayOfWeekShort = string.Empty;
    private string _dayNumber = string.Empty;
    private int _taskCount;
    private bool _isToday;
    private bool _isSelected;

    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public string DayOfWeekShort
    {
        get => _dayOfWeekShort;
        set => SetProperty(ref _dayOfWeekShort, value);
    }

    public string DayNumber
    {
        get => _dayNumber;
        set => SetProperty(ref _dayNumber, value);
    }

    public int TaskCount
    {
        get => _taskCount;
        set
        {
            if (SetProperty(ref _taskCount, value))
            {
                OnPropertyChanged(nameof(TaskCountText));
            }
        }
    }

    public string TaskCountText => $"• {_taskCount}";

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
