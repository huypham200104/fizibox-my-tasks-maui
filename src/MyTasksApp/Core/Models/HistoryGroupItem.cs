using System.Collections.ObjectModel;
using MyTasksApp.ViewModels.Base;

namespace MyTasksApp.Core.Models;

/// <summary>
/// Đại diện cho một nhóm công việc đã hoàn thành theo ngày trong Màn hình Lịch sử (HistoryPage)
/// Kế thừa ObservableObject để hỗ trợ binding dữ liệu hai chiều.
/// </summary>
public class HistoryGroupItem : ObservableObject
{
    private DateTime _date;
    private string _title = string.Empty;
    private string _subtitle = string.Empty;

    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Subtitle
    {
        get => _subtitle;
        set => SetProperty(ref _subtitle, value);
    }

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    public HistoryGroupItem()
    {
    }

    public HistoryGroupItem(DateTime date, string title, string subtitle, IEnumerable<TaskItem>? tasks = null)
    {
        Date = date.Date;
        Title = title;
        Subtitle = subtitle;

        if (tasks != null)
        {
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }
    }
}
