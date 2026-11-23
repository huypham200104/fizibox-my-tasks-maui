using MyTasksApp.Core.Enums;
using MyTasksApp.ViewModels.Base;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Core.Models;

/// <summary>
/// Thực thể công việc (Task Entity theo ASSIGNMENT.md và DESIGN.md)
/// Kế thừa ObservableObject để hỗ trợ Two-Way Data Binding phản ứng tức thì khi đổi trạng thái.
/// </summary>
public class TaskItem : ObservableObject
{
    private int _id;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _projectName = string.Empty;
    private DateTime _dueDate = DateTime.Today;
    private TimeSpan _dueTime = new(9, 0, 0);
    private TaskStatus _status = TaskStatus.Pending;
    private string _note = string.Empty;
    private DateTime? _completedAt;

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string ProjectName
    {
        get => _projectName;
        set
        {
            if (SetProperty(ref _projectName, value))
            {
                OnPropertyChanged(nameof(HasProject));
            }
        }
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (SetProperty(ref _dueDate, value))
            {
                OnPropertyChanged(nameof(IsOverdue));
                OnPropertyChanged(nameof(IsDueToday));
                OnPropertyChanged(nameof(FormattedDueDate));
                OnPropertyChanged(nameof(DueBadgeText));
            }
        }
    }

    public TimeSpan DueTime
    {
        get => _dueTime;
        set
        {
            if (SetProperty(ref _dueTime, value))
            {
                OnPropertyChanged(nameof(FormattedDueTime));
                OnPropertyChanged(nameof(DueBadgeText));
            }
        }
    }

    public TaskStatus Status
    {
        get => _status;
        set
        {
            if (SetProperty(ref _status, value))
            {
                OnPropertyChanged(nameof(IsCompleted));
                OnPropertyChanged(nameof(IsInProgress));
                OnPropertyChanged(nameof(IsPending));
                OnPropertyChanged(nameof(IsOverdue));
                OnPropertyChanged(nameof(IsDueToday));
                OnPropertyChanged(nameof(DueBadgeText));
            }
        }
    }

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public DateTime? CompletedAt
    {
        get => _completedAt;
        set
        {
            if (SetProperty(ref _completedAt, value))
            {
                OnPropertyChanged(nameof(FormattedCompletedTime));
                OnPropertyChanged(nameof(DueBadgeText));
            }
        }
    }

    // =================================================================
    // COMPUTED PROPERTIES HỖ TRỢ XAML DATA BINDING
    // =================================================================

    public bool IsCompleted => Status == TaskStatus.Completed;
    public bool IsInProgress => Status == TaskStatus.InProgress;
    public bool IsPending => Status == TaskStatus.Pending;
    public bool HasProject => !string.IsNullOrWhiteSpace(ProjectName);

    public bool IsOverdue => Status != TaskStatus.Completed && DueDate.Date < DateTime.Today;
    public bool IsDueToday => Status != TaskStatus.Completed && DueDate.Date == DateTime.Today;

    public string FormattedDueTime => DueTime.ToString(@"hh\:mm");
    public string FormattedDueDate => DueDate.ToString("dd/MM/yyyy");
    public string FormattedCompletedTime => CompletedAt?.ToString("HH:mm") ?? string.Empty;

    /// <summary>
    /// Nhãn hiển thị badge thời gian trên Task Card theo DESIGN.md 3.1
    /// </summary>
    public string DueBadgeText
    {
        get
        {
            if (IsCompleted)
            {
                return CompletedAt.HasValue ? CompletedAt.Value.ToString("HH:mm") : "Đã xong";
            }

            if (IsOverdue)
            {
                int overdueDays = (DateTime.Today - DueDate.Date).Days;
                return overdueDays <= 1 ? "Quá hạn 1 ngày" : $"Quá hạn {overdueDays} ngày";
            }

            if (IsDueToday)
            {
                return $"{FormattedDueTime} hôm nay";
            }

            return $"{FormattedDueTime} • {DueDate:dd/MM}";
        }
    }
}
