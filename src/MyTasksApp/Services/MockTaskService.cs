using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using MyTasksApp.Services.Interfaces;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Services;

/// <summary>
/// Dịch vụ quản lý dữ liệu In-Memory nạp sẵn bộ Mock Data thực tế 16–22/11/2026
/// Hỗ trợ cờ giả lập Loading, Empty, đồng bộ state realtime và ngăn xếp Undo.
/// </summary>
public class MockTaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = new();
    private readonly Stack<TaskItem> _undoStack = new();
    private int _nextId = 1;

    public event EventHandler? TasksChanged;

    public bool SimulateLoading { get; set; }
    public bool SimulateEmpty { get; set; }

    public MockTaskService()
    {
        InitializeDefaultMockData();
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks.Select(CloneTask).ToList();
        }
    }

    public async Task<List<TaskItem>> GetTodayTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        DateTime today = DateTime.Today;
        lock (_tasks)
        {
            // Điểm tự quyết số 1:
            // 1. Quá hạn: Hạn trước hôm nay & Chưa hoàn thành
            // 2. Hôm nay: Hạn là hôm nay & Chưa hoàn thành
            // 3. Xong: Có hạn hôm nay đã hoàn thành HOẶC vừa hoàn thành trong ngày hôm nay
            return _tasks
                .Where(t =>
                    (t.DueDate.Date < today && t.Status != TaskStatus.Completed) ||
                    (t.DueDate.Date == today && t.Status != TaskStatus.Completed) ||
                    (t.Status == TaskStatus.Completed && (t.DueDate.Date == today || (t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today))))
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.DueTime)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<List<TaskItem>> GetTasksByDateAsync(DateTime date)
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks
                .Where(t => t.DueDate.Date == date.Date)
                .OrderBy(t => t.Status == TaskStatus.Completed)
                .ThenBy(t => t.DueTime)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<List<TaskItem>> GetCompletedTasksAsync()
    {
        await DelaySimulateAsync();
        if (SimulateEmpty) return new List<TaskItem>();

        lock (_tasks)
        {
            return _tasks
                .Where(t => t.Status == TaskStatus.Completed)
                .OrderByDescending(t => t.CompletedAt ?? t.DueDate)
                .Select(CloneTask)
                .ToList();
        }
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return task != null ? CloneTask(task) : null;
        }
    }

    public async Task<bool> AddTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            task.Id = _nextId++;
            _tasks.Add(CloneTask(task));
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing == null) return false;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.ProjectName = task.ProjectName;
            existing.DueDate = task.DueDate;
            existing.DueTime = task.DueTime;
            existing.Status = task.Status;
            existing.Note = task.Note;
            existing.CompletedAt = task.CompletedAt;
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> ToggleTaskStatusAsync(int taskId)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            if (task.Status == TaskStatus.Completed)
            {
                task.Status = TaskStatus.Pending;
                task.CompletedAt = null;
            }
            else
            {
                task.Status = TaskStatus.Completed;
                task.CompletedAt = DateTime.Now;
            }
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> SetTaskStatusAsync(int taskId, TaskStatus status)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            task.Status = status;
            task.CompletedAt = status == TaskStatus.Completed ? DateTime.Now : null;
        }

        OnTasksChanged();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        await DelaySimulateAsync();
        lock (_tasks)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null) return false;

            _tasks.Remove(task);
            _undoStack.Push(task);
        }

        OnTasksChanged();
        return true;
    }

    public async Task<TaskItem?> UndoDeleteAsync()
    {
        await DelaySimulateAsync();
        TaskItem? restoredTask = null;

        lock (_tasks)
        {
            if (_undoStack.Count > 0)
            {
                restoredTask = _undoStack.Pop();
                _tasks.Add(restoredTask);
            }
        }

        if (restoredTask != null)
        {
            OnTasksChanged();
        }

        return restoredTask;
    }

    public Task ResetMockDataAsync()
    {
        lock (_tasks)
        {
            _tasks.Clear();
            _undoStack.Clear();
            SimulateLoading = false;
            SimulateEmpty = false;
            InitializeDefaultMockData();
        }

        OnTasksChanged();
        return Task.CompletedTask;
    }

    private void OnTasksChanged()
    {
        TasksChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task DelaySimulateAsync()
    {
        if (SimulateLoading)
        {
            await Task.Delay(1500);
        }
        else
        {
            await Task.CompletedTask;
        }
    }

    private static TaskItem CloneTask(TaskItem source)
    {
        return new TaskItem
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            ProjectName = source.ProjectName,
            DueDate = source.DueDate,
            DueTime = source.DueTime,
            Status = source.Status,
            Note = source.Note,
            CompletedAt = source.CompletedAt
        };
    }

    private void InitializeDefaultMockData()
    {
        DateTime today = DateTime.Today; // 20/11/2026

        // Tính ngày đầu tuần (Thứ Hai) theo chuẩn ISO 8601
        int diffToMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime monday = today.AddDays(-diffToMonday);

        DateTime d16 = monday;
        DateTime d17 = monday.AddDays(1);
        DateTime d18 = monday.AddDays(2);
        DateTime d19 = monday.AddDays(3); // Hôm qua
        DateTime d20 = today;            // Hôm nay
        DateTime d21 = monday.AddDays(5); // Ngày mai
        DateTime d22 = monday.AddDays(6); // Chủ nhật

        _tasks.AddRange(new List<TaskItem>
        {
            // === THỨ HAI (16/11) ===
            new() {
                Id = 1,
                Title = "Họp khởi động kế hoạch tuần",
                Description = "Định hướng mục tiêu và phân công công việc tuần 47.",
                ProjectName = "Nội bộ",
                DueDate = d16,
                DueTime = new TimeSpan(9, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d16.Add(new TimeSpan(10, 0, 0)),
                Note = "Đã thống nhất phân bổ tài nguyên cho team Mobile."
            },
            new() {
                Id = 2,
                Title = "Rà soát yêu cầu kỹ thuật dự án E-Commerce",
                Description = "Đọc tài liệu SRS và liệt kê danh sách endpoint API cần thiết.",
                ProjectName = "E-Commerce",
                DueDate = d16,
                DueTime = new TimeSpan(14, 30, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d16.Add(new TimeSpan(16, 0, 0)),
                Note = "Cần backend bổ sung API filter danh mục."
            },

            // === THỨ BA (17/11) ===
            new() {
                Id = 3,
                Title = "Thiết kế wireframe màn hình Checkout",
                Description = "Vẽ layout các bước nhập địa chỉ giao hàng và phương thức thanh toán.",
                ProjectName = "UI/UX",
                DueDate = d17,
                DueTime = new TimeSpan(11, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d17.Add(new TimeSpan(11, 30, 0)),
                Note = "Đã gửi Figma link cho PO review."
            },
            new() {
                Id = 4,
                Title = "Tối ưu hóa query database SQL",
                Description = "Đánh index cho các bảng đơn hàng lớn để giảm tải latency.",
                ProjectName = "Backend",
                DueDate = d17,
                DueTime = new TimeSpan(16, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d17.Add(new TimeSpan(17, 15, 0)),
                Note = "Thời gian query giảm từ 1.2s xuống 80ms."
            },

            // === THỨ TƯ (18/11) ===
            new() {
                Id = 5,
                Title = "Phỏng vấn ứng viên Fresher Mobile",
                Description = "Đánh giá kiến thức OOP, C# cơ bản và tư duy logic.",
                ProjectName = "Tuyển dụng",
                DueDate = d18,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d18.Add(new TimeSpan(11, 0, 0)),
                Note = "Ứng viên có tiềm năng, nắm chắc OOP."
            },

            // === THỨ NĂM (19/11) - CÓ 1 VIỆC QUÁ HẠN ===
            new() {
                Id = 6,
                Title = "Soạn thảo tài liệu phân tích hệ thống",
                Description = "Tổng hợp kiến trúc Clean Architecture, sơ đồ luồng dữ liệu và danh mục Design Tokens.",
                ProjectName = "Core Platform",
                DueDate = d19,
                DueTime = new TimeSpan(17, 0, 0),
                Status = TaskStatus.Pending, // QUÁ HẠN
                Note = "Cần chốt danh mục Token trước khi bàn giao cho team phát triển giao diện."
            },
            new() {
                Id = 7,
                Title = "Setup CI/CD pipeline build tự động",
                Description = "Tạo GitHub Action workflow tự động build và chạy test khi push code.",
                ProjectName = "DevOps",
                DueDate = d19,
                DueTime = new TimeSpan(15, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d19.Add(new TimeSpan(16, 30, 0)),
                Note = "Workflow đã chạy ổn định trên GitHub Actions."
            },

            // === THỨ SÁU (20/11 - HÔM NAY) ===
            new() {
                Id = 8,
                Title = "Họp đồng bộ tiến độ tuần (Weekly Sync)",
                Description = "Báo cáo kết quả tuần và xử lý các vấn đề nghẽn tài nguyên.",
                ProjectName = "Nội bộ",
                DueDate = d20,
                DueTime = new TimeSpan(9, 30, 0),
                Status = TaskStatus.Pending, // HÔM NAY
                Note = "Chuẩn bị slide tóm tắt tiến độ 4 màn hình."
            },
            new() {
                Id = 9,
                Title = "Cập nhật tài liệu thiết kế UI",
                Description = "Đồng bộ mã màu HEX, khoảng cách padding và typography tokens vào DESIGN.md.",
                ProjectName = "MyTasksApp",
                DueDate = d20,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Completed, // XONG HÔM NAY
                CompletedAt = d20.Add(new TimeSpan(10, 30, 0)),
                Note = "Đã hoàn thành chuẩn hóa toàn bộ token theo bộ thiết kế ui_design/."
            },
            new() {
                Id = 10,
                Title = "Kiểm thử API thanh toán trên Staging",
                Description = "Thực hiện test các trường hợp giao dịch thành công, thất bại và hủy đơn hàng.",
                ProjectName = "E-Commerce",
                DueDate = d20,
                DueTime = new TimeSpan(11, 30, 0),
                Status = TaskStatus.Completed, // XONG HÔM NAY
                CompletedAt = d20.Add(new TimeSpan(11, 45, 0)),
                Note = "Tất cả test cases thanh toán thẻ nội địa đều pass."
            },
            new() {
                Id = 11,
                Title = "Code review Pull Request tính năng giỏ hàng",
                Description = "Kiểm tra code quality, quản lý state và xử lý ngoại lệ khi giỏ hàng trống.",
                ProjectName = "E-Commerce",
                DueDate = d20,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.InProgress, // ĐANG LÀM HÔM NAY
                Note = "Cần nhắc dev thêm try-catch cho luồng lưu LocalStorage."
            },
            new() {
                Id = 12,
                Title = "Viết báo cáo đánh giá năng suất cá nhân",
                Description = "Tổng kết số lượng story points đã hoàn thành và đề xuất cải tiến quy trình sprint tới.",
                ProjectName = "Báo cáo",
                DueDate = d20,
                DueTime = new TimeSpan(17, 30, 0),
                Status = TaskStatus.Pending, // HÔM NAY
                Note = "Báo cáo sẽ gửi cho trưởng nhóm trước 18h."
            },

            // === THỨ BẢY (21/11) ===
            new() {
                Id = 13,
                Title = "Kiểm tra toàn diện ứng dụng trước demo",
                Description = "Smoke test trên Android Emulator ở cả Light/Dark Mode và Cỡ chữ lớn.",
                ProjectName = "QA",
                DueDate = d21,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Kiểm tra kỹ tương phản WCAG AA và vùng chạm 44x44dp."
            },

            // === CHỦ NHẬT (22/11) ===
            new() {
                Id = 14,
                Title = "Lên kế hoạch công việc tuần mới",
                Description = "Sắp xếp thứ tự ưu tiên các công việc tồn đọng và chuẩn bị cho Sprint tiếp theo.",
                ProjectName = "Cá nhân",
                DueDate = d22,
                DueTime = new TimeSpan(20, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Dành 30 phút buổi tối để sắp xếp lịch."
            }
        });

        _nextId = 15;
    }
}
