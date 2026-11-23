using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Services.Interfaces;

/// <summary>
/// Giao diện quản lý dịch vụ công việc theo Clean Architecture
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Sự kiện thông báo thay đổi dữ liệu giúp đồng bộ state realtime giữa Hôm nay, Tuần này, Chi tiết và Lịch sử
    /// </summary>
    event EventHandler? TasksChanged;

    /// <summary>
    /// Cờ giả lập trạng thái đang tải (Loading Skeleton Shimmer) cho Reviewer
    /// </summary>
    bool SimulateLoading { get; set; }

    /// <summary>
    /// Cờ giả lập trạng thái danh sách rỗng (Empty State) cho Reviewer
    /// </summary>
    bool SimulateEmpty { get; set; }

    /// <summary>
    /// Lấy toàn bộ danh sách công việc
    /// </summary>
    Task<List<TaskItem>> GetAllTasksAsync();

    /// <summary>
    /// Lấy danh sách công việc liên quan đến hôm nay (Quá hạn + Hạn hôm nay + Vừa xong hôm nay)
    /// </summary>
    Task<List<TaskItem>> GetTodayTasksAsync();

    /// <summary>
    /// Lấy danh sách công việc theo ngày cụ thể (Dùng cho Màn hình Tuần này)
    /// </summary>
    Task<List<TaskItem>> GetTasksByDateAsync(DateTime date);

    /// <summary>
    /// Lấy danh sách toàn bộ công việc đã hoàn thành (Dùng cho Màn hình Đã xong)
    /// </summary>
    Task<List<TaskItem>> GetCompletedTasksAsync();

    /// <summary>
    /// Lấy chi tiết 1 công việc theo Id
    /// </summary>
    Task<TaskItem?> GetTaskByIdAsync(int id);

    /// <summary>
    /// Thêm công việc mới
    /// </summary>
    Task<bool> AddTaskAsync(TaskItem task);

    /// <summary>
    /// Cập nhật thông tin công việc
    /// </summary>
    Task<bool> UpdateTaskAsync(TaskItem task);

    /// <summary>
    /// Chuyển đổi nhanh trạng thái công việc (Pending sang Completed và ngược lại)
    /// </summary>
    Task<bool> ToggleTaskStatusAsync(int taskId);

    /// <summary>
    /// Đặt trạng thái cụ thể (Pending / InProgress / Completed)
    /// </summary>
    Task<bool> SetTaskStatusAsync(int taskId, TaskStatus status);

    /// <summary>
    /// Xóa công việc và đẩy vào ngăn xếp Hoàn tác (Undo Stack)
    /// </summary>
    Task<bool> DeleteTaskAsync(int taskId);

    /// <summary>
    /// Hoàn tác việc vừa xóa
    /// </summary>
    Task<TaskItem?> UndoDeleteAsync();

    /// <summary>
    /// Đặt lại dữ liệu mẫu in-memory chuẩn 16–22/11/2026 cho Reviewer
    /// </summary>
    Task ResetMockDataAsync();

    /// <summary>
    /// Sự kiện thông báo thay đổi danh sách dự án
    /// </summary>
    event EventHandler? ProjectsChanged;

    /// <summary>
    /// Lấy danh sách toàn bộ dự án
    /// </summary>
    Task<List<Project>> GetAllProjectsAsync();

    /// <summary>
    /// Thêm dự án mới
    /// </summary>
    Task<bool> AddProjectAsync(Project project);

    /// <summary>
    /// Cập nhật thông tin dự án
    /// </summary>
    Task<bool> UpdateProjectAsync(Project project);

    /// <summary>
    /// Xóa dự án theo Id
    /// </summary>
    Task<bool> DeleteProjectAsync(string projectId);
}
