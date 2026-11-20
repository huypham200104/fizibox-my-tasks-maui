namespace MyTasksApp.Core.Enums;

/// <summary>
/// Trạng thái công việc (Task Status Workflow theo BUSINESS.md và bottom_sheet_change_status.png)
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Chưa hoàn thành (Vòng tròn rỗng viền xám)
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đang thực hiện (Chấm tròn màu cam)
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Đã hoàn thành (Vòng tròn xanh lá kèm dấu tích trắng)
    /// </summary>
    Completed = 2
}
