namespace MyTasksApp.Core.Enums;

/// <summary>
/// 3 nhóm phân loại công việc tại Màn hình 1 - Hôm nay (Section Groups)
/// </summary>
public enum TaskGroupType
{
    /// <summary>
    /// Việc chưa xong có hạn trước hôm nay (Màu đỏ Danger)
    /// </summary>
    Overdue = 0,

    /// <summary>
    /// Việc chưa xong có hạn là hôm nay (Màu cam Warning)
    /// </summary>
    DueToday = 1,

    /// <summary>
    /// Việc có hạn hôm nay đã hoàn thành hoặc vừa xong trong ngày (Màu xanh Success)
    /// </summary>
    Completed = 2
}
