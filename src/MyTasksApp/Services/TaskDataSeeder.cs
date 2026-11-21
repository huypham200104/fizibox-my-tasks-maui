using MyTasksApp.Core.Enums;
using MyTasksApp.Core.Models;
using TaskStatus = MyTasksApp.Core.Enums.TaskStatus;

namespace MyTasksApp.Services;

/// <summary>
/// Bộ tạo dữ liệu mẫu (Seeder) cho ứng dụng MyTasksApp
/// Cung cấp 74 công việc thực tế, phân bổ từ 4 đến 12 công việc mỗi ngày trong dải ngày 19/11/2026 - 28/11/2026.
/// </summary>
public static class TaskDataSeeder
{
    public static List<TaskItem> GenerateDefaultTasks()
    {
        DateTime d19 = new(2026, 11, 19); // Thứ Năm - 5 tasks (1 Quá hạn, 4 Đã hoàn thành)
        DateTime d20 = new(2026, 11, 20); // Thứ Sáu - 8 tasks (3 Đã xong, 1 Đang làm, 4 Chưa xong)
        DateTime d21 = new(2026, 11, 21); // Thứ Bảy - 6 tasks (2 Đã xong, 1 Đang làm, 3 Chưa xong)
        DateTime d22 = new(2026, 11, 22); // Chủ Nhật - 4 tasks (1 Đã xong, 3 Chưa xong)
        DateTime d23 = new(2026, 11, 23); // Thứ Hai - 10 tasks (2 Đang làm, 8 Chưa xong)
        DateTime d24 = new(2026, 11, 24); // Thứ Ba - 8 tasks (1 Đang làm, 7 Chưa xong)
        DateTime d25 = new(2026, 11, 25); // Thứ Tư - 12 tasks (Ngày cao điểm - 2 Đang làm, 10 Chưa xong)
        DateTime d26 = new(2026, 11, 26); // Thứ Năm - 9 tasks (1 Đang làm, 8 Chưa xong)
        DateTime d27 = new(2026, 11, 27); // Thứ Sáu - 7 tasks (1 Đang làm, 6 Chưa xong)
        DateTime d28 = new(2026, 11, 28); // Thứ Bảy - 5 tasks (5 Chưa xong)

        var tasks = new List<TaskItem>
        {
            // =================================================================
            // NGÀY 19/11/2026 (THỨ NĂM) - 5 TASKS
            // =================================================================
            new() {
                Id = 1,
                Title = "Soạn thảo tài liệu phân tích hệ thống",
                Description = "Tổng hợp kiến trúc Clean Architecture, sơ đồ luồng dữ liệu và danh mục Design Tokens.",
                ProjectName = "Core Platform",
                DueDate = d19,
                DueTime = new TimeSpan(17, 0, 0),
                Status = TaskStatus.Pending, // Việc Quá hạn mẫu
                Note = "Cần chốt danh mục Token trước khi bàn giao cho team phát triển giao diện."
            },
            new() {
                Id = 2,
                Title = "Setup CI/CD pipeline build tự động",
                Description = "Tạo GitHub Action workflow tự động build và chạy test khi push code.",
                ProjectName = "DevOps",
                DueDate = d19,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d19.Add(new TimeSpan(11, 15, 0)),
                Note = "Workflow đã chạy ổn định trên GitHub Actions."
            },
            new() {
                Id = 3,
                Title = "Kiểm tra tiến độ dự án",
                Description = "Rà soát lại tiến độ các đầu việc tuần 47 của team Mobile và Backend.",
                ProjectName = "MyTasks App",
                DueDate = d19,
                DueTime = new TimeSpan(11, 30, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d19.Add(new TimeSpan(11, 45, 0)),
                Note = "Team bám sát kế hoạch, không có blocker lớn."
            },
            new() {
                Id = 4,
                Title = "Review Pull Request module xác thực",
                Description = "Kiểm tra kỹ mã nguồn JWT refresh token và bảo mật mật khẩu người dùng.",
                ProjectName = "Dự án Backend",
                DueDate = d19,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d19.Add(new TimeSpan(14, 50, 0)),
                Note = "Đã approve sau khi tác giả fix các lỗi convention."
            },
            new() {
                Id = 5,
                Title = "Đồng bộ mockup Figma sang Design System",
                Description = "Xuất các component nút bấm, thẻ công việc và icon sang thư mục assets.",
                ProjectName = "UI/UX Design",
                DueDate = d19,
                DueTime = new TimeSpan(16, 30, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d19.Add(new TimeSpan(16, 45, 0)),
                Note = "Đã cập nhật đúng chuẩn màu Primary Indigo và Slate."
            },

            // =================================================================
            // NGÀY 20/11/2026 (THỨ SÁU) - 8 TASKS
            // =================================================================
            new() {
                Id = 6,
                Title = "Họp đồng bộ tiến độ tuần (Weekly Sync)",
                Description = "Báo cáo kết quả tuần và xử lý các vấn đề nghẽn tài nguyên.",
                ProjectName = "Nội bộ",
                DueDate = d20,
                DueTime = new TimeSpan(9, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Chuẩn bị slide tóm tắt tiến độ 4 màn hình."
            },
            new() {
                Id = 7,
                Title = "Cập nhật tài liệu thiết kế UI",
                Description = "Viết bổ sung đặc tả màu sắc cho chế độ Dark Mode đạt chuẩn tương phản WCAG AA.",
                ProjectName = "UI/UX Design",
                DueDate = d20,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đặc biệt lưu ý độ tương phản của TextSecondary trên nền Slate-800."
            },
            new() {
                Id = 8,
                Title = "Chuẩn bị dữ liệu demo cho Reviewer",
                Description = "Xây dựng kịch bản kiểm thử tính năng chuyển đổi ngôn ngữ và đổi theme tức thì.",
                ProjectName = "MyTasks App",
                DueDate = d20,
                DueTime = new TimeSpan(16, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Kịch bản cần bao phủ đầy đủ các màn hình và popup modal."
            },
            new() {
                Id = 9,
                Title = "Kiểm thử API nạp danh sách công việc",
                Description = "Viết test case kiểm tra thời gian phản hồi khi danh sách có hơn 100 tasks.",
                ProjectName = "Dự án Backend",
                DueDate = d20,
                DueTime = new TimeSpan(17, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo pagination và caching hoạt động mượt mà."
            },
            new() {
                Id = 10,
                Title = "Triển khai giao diện Lịch sử công việc",
                Description = "Xây dựng HistoryPage với bộ lọc thời gian và thanh tìm kiếm realtime.",
                ProjectName = "Dự án Mobile App",
                DueDate = d20,
                DueTime = new TimeSpan(15, 0, 0),
                Status = TaskStatus.InProgress,
                Note = "Đang hoàn thiện phần gom nhóm công việc theo ngày."
            },
            new() {
                Id = 11,
                Title = "Hoàn thiện thiết kế Home Screen",
                Description = "Căn chỉnh các khoảng cách thẻ Task Card theo Spacing Grid bội số 4.",
                ProjectName = "UI/UX Design",
                DueDate = d20,
                DueTime = new TimeSpan(9, 15, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d20.Add(new TimeSpan(9, 20, 0)),
                Note = "Giao diện responsive tốt trên các kích thước 360dp - 412dp."
            },
            new() {
                Id = 12,
                Title = "Đăng ký dịch vụ DI trong MauiProgram",
                Description = "Đăng ký các Singleton ViewModel và Service để hỗ trợ chuyển tab tức thì 0ms.",
                ProjectName = "Core Platform",
                DueDate = d20,
                DueTime = new TimeSpan(10, 30, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d20.Add(new TimeSpan(10, 35, 0)),
                Note = "Chuyển tab không còn hiện tượng giật lag hay khựng giao diện."
            },
            new() {
                Id = 13,
                Title = "Cấu hình font chữ Inter chuẩn",
                Description = "Nhúng các biến thể Inter Regular, Medium, SemiBold và Bold vào ứng dụng.",
                ProjectName = "MyTasks App",
                DueDate = d20,
                DueTime = new TimeSpan(11, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d20.Add(new TimeSpan(11, 10, 0)),
                Note = "Font hiển thị sắc nét trên cả màn hình mật độ điểm ảnh cao."
            },

            // =================================================================
            // NGÀY 21/11/2026 (THỨ BẢY) - 6 TASKS
            // =================================================================
            new() {
                Id = 14,
                Title = "Đọc tài liệu .NET MAUI 10 Performance",
                Description = "Tìm hiểu các cải tiến XAML Source Generation và Native AOT trong .NET 10.",
                ProjectName = "Công việc cá nhân",
                DueDate = d21,
                DueTime = new TimeSpan(9, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d21.Add(new TimeSpan(10, 15, 0)),
                Note = "Source Generator giúp khởi tạo view nhanh hơn 35%."
            },
            new() {
                Id = 15,
                Title = "Dọn dẹp mã nguồn và refactor ViewModel",
                Description = "Tách các phương thức phụ trợ thành các helper riêng biệt, tăng tính đóng gói.",
                ProjectName = "MyTasks App",
                DueDate = d21,
                DueTime = new TimeSpan(10, 30, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d21.Add(new TimeSpan(11, 20, 0)),
                Note = "Giảm đáng kể sự trùng lặp code giữa các ViewModel."
            },
            new() {
                Id = 16,
                Title = "Kiểm tra tương thích trên Android Emulator",
                Description = "Chạy thử nghiệm trên máy ảo Pixel 8 Pro và Nexus One màn hình nhỏ 320dp.",
                ProjectName = "DevOps",
                DueDate = d21,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.InProgress,
                Note = "Layout không bị vỡ trên màn hình siêu hẹp."
            },
            new() {
                Id = 17,
                Title = "Lên danh sách tính năng cho Sprint tiếp theo",
                Description = "Liệt kê các mục tiêu cần đạt: Đồng bộ đám mây và Thông báo nhắc việc cục bộ.",
                ProjectName = "Core Platform",
                DueDate = d21,
                DueTime = new TimeSpan(15, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Cần ưu tiên tính năng nhắc việc theo thời gian thực."
            },
            new() {
                Id = 18,
                Title = "Mua sắm thiết bị kiểm thử di động",
                Description = "Đặt mua thêm cáp kết nối và giá đỡ điện thoại để test tương tác trực tiếp.",
                ProjectName = "Công việc cá nhân",
                DueDate = d21,
                DueTime = new TimeSpan(17, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Chọn loại cáp Type-C có hỗ trợ truyền dữ liệu tốc độ cao."
            },
            new() {
                Id = 19,
                Title = "Viết ghi chú tóm tắt bài học kinh nghiệm",
                Description = "Ghi lại những lưu ý khi xử lý bàn phím ảo và Safe Area trên iOS và Android.",
                ProjectName = "Công việc cá nhân",
                DueDate = d21,
                DueTime = new TimeSpan(18, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Lưu vào kho tài liệu nội bộ team."
            },

            // =================================================================
            // NGÀY 22/11/2026 (CHỦ NHẬT) - 4 TASKS
            // =================================================================
            new() {
                Id = 20,
                Title = "Lập kế hoạch làm việc cho tuần mới",
                Description = "Xác định 3 ưu tiên lớn nhất cần hoàn thành trong tuần từ 23/11 đến 28/11.",
                ProjectName = "Công việc cá nhân",
                DueDate = d22,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Completed,
                CompletedAt = d22.Add(new TimeSpan(10, 45, 0)),
                Note = "3 ưu tiên: Tối ưu UI, Viết Unit Test và Hoàn thiện tài liệu."
            },
            new() {
                Id = 21,
                Title = "Chạy bộ rèn luyện sức khỏe cuối tuần",
                Description = "Hoàn thành cự ly 5km tại công viên để nạp năng lượng cho tuần mới.",
                ProjectName = "Công việc cá nhân",
                DueDate = d22,
                DueTime = new TimeSpan(16, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đạt mục tiêu pace dưới 6:30."
            },
            new() {
                Id = 22,
                Title = "Sao lưu dữ liệu dự án lên đám mây",
                Description = "Tạo bản snapshot backup toàn bộ database và các branch Git quan trọng.",
                ProjectName = "DevOps",
                DueDate = d22,
                DueTime = new TimeSpan(20, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đã mã hóa dữ liệu trước khi đẩy lên bộ nhớ an toàn."
            },
            new() {
                Id = 23,
                Title = "Đọc sách Clean Code chương 9: Unit Tests",
                Description = "Ôn lại các quy tắc F.I.R.S.T và kỹ thuật viết test case ngắn gọn, dễ bảo trì.",
                ProjectName = "Công việc cá nhân",
                DueDate = d22,
                DueTime = new TimeSpan(21, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Áp dụng ngay cho bộ test của MyTasksApp."
            },

            // =================================================================
            // NGÀY 23/11/2026 (THỨ HAI) - 10 TASKS
            // =================================================================
            new() {
                Id = 24,
                Title = "Họp khởi động Sprint mới (Sprint Planning)",
                Description = "Thống nhất backlog và phân chia công việc cho từng thành viên trong team.",
                ProjectName = "Nội bộ",
                DueDate = d23,
                DueTime = new TimeSpan(8, 30, 0),
                Status = TaskStatus.InProgress,
                Note = "Sprint kéo dài 2 tuần, tập trung vào hoàn thiện toàn diện ứng dụng."
            },
            new() {
                Id = 25,
                Title = "Phân tích yêu cầu module Bottom Sheet",
                Description = "Thiết kế các modal tương tác chọn trạng thái, chọn ngày và chọn giờ.",
                ProjectName = "UI/UX Design",
                DueDate = d23,
                DueTime = new TimeSpan(9, 45, 0),
                Status = TaskStatus.InProgress,
                Note = "Bám sát kích thước và các tùy chọn trong DESIGN.md."
            },
            new() {
                Id = 26,
                Title = "Tạo Base Component cho Bottom Sheet",
                Description = "Xây dựng khung kéo thả (Drag Handle), hiệu ứng mờ nền và bo góc trên 24dp.",
                ProjectName = "Core Platform",
                DueDate = d23,
                DueTime = new TimeSpan(11, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo đóng modal mượt mà khi nhấn ra ngoài nền mờ."
            },
            new() {
                Id = 27,
                Title = "Tích hợp Bottom Sheet chọn trạng thái",
                Description = "Cho phép người dùng chọn nhanh 1 trong 3 mức: Chưa hoàn thành, Đang làm, Đã xong.",
                ProjectName = "Dự án Mobile App",
                DueDate = d23,
                DueTime = new TimeSpan(13, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Hiển thị dấu tích và viền tím nổi bật quanh lựa chọn đang được chọn."
            },
            new() {
                Id = 28,
                Title = "Viết Unit Test cho chuyển đổi trạng thái",
                Description = "Kiểm tra sự kiện TasksChanged phát ra đúng lúc khi chuyển đổi trạng thái.",
                ProjectName = "MyTasks App",
                DueDate = d23,
                DueTime = new TimeSpan(14, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Bao phủ các trường hợp Pending -> Completed và ngược lại."
            },
            new() {
                Id = 29,
                Title = "Review bảo mật API phân quyền",
                Description = "Kiểm tra lỗ hổng IDOR khi người dùng truy cập công việc của tài khoản khác.",
                ProjectName = "Dự án Backend",
                DueDate = d23,
                DueTime = new TimeSpan(15, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Mọi request đều phải kiểm tra quyền sở hữu TenantId."
            },
            new() {
                Id = 30,
                Title = "Thiết kế giao diện bộ chọn ngày",
                Description = "Vẽ lưới lịch tháng trực quan, có nút quay về tháng hiện tại và nút Hôm nay.",
                ProjectName = "UI/UX Design",
                DueDate = d23,
                DueTime = new TimeSpan(16, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Đánh dấu ngày hôm nay bằng viền nổi bật."
            },
            new() {
                Id = 31,
                Title = "Tối ưu hóa hình ảnh tài nguyên",
                Description = "Nén các file PNG logo và icon bằng TinyPNG để giảm dung lượng file APK.",
                ProjectName = "DevOps",
                DueDate = d23,
                DueTime = new TimeSpan(17, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Giảm tối thiểu 40% dung lượng mà không làm suy giảm chất lượng hiển thị."
            },
            new() {
                Id = 32,
                Title = "Cập nhật tài liệu API Swagger",
                Description = "Mô tả chi tiết các tham số đầu vào và mã lỗi trả về của từng endpoint.",
                ProjectName = "Dự án Backend",
                DueDate = d23,
                DueTime = new TimeSpan(17, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Xuất file OpenAPI v3.0 để team Mobile tiện tích hợp."
            },
            new() {
                Id = 33,
                Title = "Kiểm tra tin nhắn và phản hồi email đối tác",
                Description = "Phản hồi các thắc mắc về tiến độ bàn giao bản dùng thử v1.0.",
                ProjectName = "Công việc cá nhân",
                DueDate = d23,
                DueTime = new TimeSpan(18, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Xác nhận lịch demo dự kiến vào cuối tuần này."
            },

            // =================================================================
            // NGÀY 24/11/2026 (THỨ BA) - 8 TASKS
            // =================================================================
            new() {
                Id = 34,
                Title = "Xây dựng Custom Date Picker Sheet",
                Description = "Lập trình component lịch tháng hỗ trợ chọn ngày đến hạn với tương tác mượt mà.",
                ProjectName = "Dự án Mobile App",
                DueDate = d24,
                DueTime = new TimeSpan(9, 0, 0),
                Status = TaskStatus.InProgress,
                Note = "Hỗ trợ 2 nút chân trang: Hủy và Xác nhận."
            },
            new() {
                Id = 35,
                Title = "Lập trình Time Picker với các nút chọn nhanh",
                Description = "Cung cấp các preset chọn giờ phổ biến: 09:00, 12:00, 14:00, 17:00.",
                ProjectName = "Dự án Mobile App",
                DueDate = d24,
                DueTime = new TimeSpan(10, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Hiển thị khung số giờ lớn màu Primary nổi bật."
            },
            new() {
                Id = 36,
                Title = "Tối ưu hóa hiệu năng render danh sách Task",
                Description = "Sử dụng BindableLayout thay cho ListView cũ để loại bỏ hiện tượng khựng khung hình.",
                ProjectName = "Core Platform",
                DueDate = d24,
                DueTime = new TimeSpan(13, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo cuộn danh sách đạt tốc độ ổn định 60-120fps."
            },
            new() {
                Id = 37,
                Title = "Kiểm thử hồi quy màn hình Hôm nay",
                Description = "Đảm bảo thanh tiến độ và số lượng việc quá hạn cập nhật chính xác.",
                ProjectName = "MyTasks App",
                DueDate = d24,
                DueTime = new TimeSpan(14, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Test trường hợp hoàn thành việc quá hạn và mở lại việc hôm nay."
            },
            new() {
                Id = 38,
                Title = "Tích hợp tính năng rung phản hồi (Haptic)",
                Description = "Kích hoạt haptic feedback nhẹ khi người dùng tích hoàn thành công việc.",
                ProjectName = "Dự án Mobile App",
                DueDate = d24,
                DueTime = new TimeSpan(15, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Sử dụng HapticFeedback.Perform(HapticFeedbackType.Click)."
            },
            new() {
                Id = 39,
                Title = "Đánh giá hiệu năng truy vấn LiteDB",
                Description = "Kiểm tra chỉ mục Index trên các trường DueDate và Status trong database.",
                ProjectName = "Dự án Backend",
                DueDate = d24,
                DueTime = new TimeSpan(16, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Chỉ mục giúp tăng tốc độ tìm kiếm gấp 5 lần khi dữ liệu lớn."
            },
            new() {
                Id = 40,
                Title = "Thảo luận giải pháp Offline-first",
                Description = "Lên phương án đồng bộ dữ liệu hai chiều khi thiết bị có kết nối mạng trở lại.",
                ProjectName = "Core Platform",
                DueDate = d24,
                DueTime = new TimeSpan(17, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Sử dụng cơ chế Conflict Resolution dựa trên Timestamp."
            },
            new() {
                Id = 41,
                Title = "Đặt lịch khám sức khỏe định kỳ",
                Description = "Chọn khung giờ khám tổng quát vào sáng thứ Bảy tuần tới.",
                ProjectName = "Công việc cá nhân",
                DueDate = d24,
                DueTime = new TimeSpan(17, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Đã nhận mã xác nhận lịch hẹn qua tin nhắn."
            },

            // =================================================================
            // NGÀY 25/11/2026 (THỨ TƯ - NGÀY CAO ĐIỂM) - 12 TASKS
            // =================================================================
            new() {
                Id = 42,
                Title = "Xây dựng Popup Modal chỉnh sửa chi tiết",
                Description = "Hỗ trợ sửa Tiêu đề (max 100 ký tự), Mô tả (max 1000) và Ghi chú (max 500).",
                ProjectName = "Dự án Mobile App",
                DueDate = d25,
                DueTime = new TimeSpan(8, 30, 0),
                Status = TaskStatus.InProgress,
                Note = "Tích hợp bộ đếm ký tự trực quan theo đúng mockup popup_edit_text.png."
            },
            new() {
                Id = 43,
                Title = "Hạn chế nhập liệu vượt quá giới hạn ký tự",
                Description = "Thêm bộ kiểm tra validation và cảnh báo viền đỏ khi người dùng nhập quá dài.",
                ProjectName = "Core Platform",
                DueDate = d25,
                DueTime = new TimeSpan(9, 15, 0),
                Status = TaskStatus.InProgress,
                Note = "Ngăn chặn việc lưu dữ liệu rác vào database."
            },
            new() {
                Id = 44,
                Title = "Cấu hình Floating SnackBar nổi đa năng",
                Description = "Hỗ trợ 4 phong cách: Thành công (xanh), Lỗi (đỏ), Cảnh báo (vàng) và Hoàn tác (tím).",
                ProjectName = "UI/UX Design",
                DueDate = d25,
                DueTime = new TimeSpan(10, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Tự động ẩn sau 5 giây hoặc khi bấm nút đóng X."
            },
            new() {
                Id = 45,
                Title = "Tích hợp cơ chế Hoàn tác xóa (Undo Delete)",
                Description = "Lưu công việc vừa xóa vào ngăn xếp Stack và khôi phục ngay khi bấm Hoàn tác.",
                ProjectName = "MyTasks App",
                DueDate = d25,
                DueTime = new TimeSpan(11, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo công việc được khôi phục về đúng vị trí và trạng thái ban đầu."
            },
            new() {
                Id = 46,
                Title = "Họp kỹ thuật giữa team Mobile và Backend",
                Description = "Chốt định dạng JSON phản hồi của các API báo cáo và thống kê theo tháng.",
                ProjectName = "Nội bộ",
                DueDate = d25,
                DueTime = new TimeSpan(13, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Thống nhất chuẩn ISO 8601 cho tất cả các trường thời gian."
            },
            new() {
                Id = 47,
                Title = "Thực hiện Code Review chéo trong team",
                Description = "Rà soát toàn bộ các thay đổi trong nhánh phát triển tuần này.",
                ProjectName = "DevOps",
                DueDate = d25,
                DueTime = new TimeSpan(14, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo không còn hardcoded color hay magic string trong XAML."
            },
            new() {
                Id = 48,
                Title = "Kiểm tra bộ từ điển song ngữ VI/EN",
                Description = "Rà soát lại tất cả các chuỗi thông báo, tiêu đề dialog và placeholder ô nhập.",
                ProjectName = "MyTasks App",
                DueDate = d25,
                DueTime = new TimeSpan(15, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo từ ngữ tự nhiên, chuẩn văn phong tiếng Việt và tiếng Anh."
            },
            new() {
                Id = 49,
                Title = "Kiểm thử tính năng vuốt thẻ việc (SwipeView)",
                Description = "Vuốt từ phải sang trái để hoàn thành, vuốt từ trái sang phải để xóa nhanh.",
                ProjectName = "Dự án Mobile App",
                DueDate = d25,
                DueTime = new TimeSpan(16, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Test độ nhạy cảm ứng trên cả thiết bị cấu hình yếu."
            },
            new() {
                Id = 50,
                Title = "Tối ưu hóa mức tiêu hao pin khi chạy ngầm",
                Description = "Giảm thiểu số lượng timer chạy nền và hủy các đăng ký sự kiện khi unbind.",
                ProjectName = "Core Platform",
                DueDate = d25,
                DueTime = new TimeSpan(16, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Tránh rò rỉ bộ nhớ (Memory Leak) khi chuyển qua lại giữa các trang."
            },
            new() {
                Id = 51,
                Title = "Cập nhật tài liệu hướng dẫn kiểm thử",
                Description = "Soạn thảo hướng dẫn Reviewer kích hoạt các cờ Loading, Empty và Not Found.",
                ProjectName = "Core Platform",
                DueDate = d25,
                DueTime = new TimeSpan(17, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Bổ sung ảnh chụp minh họa các bước thao tác trên màn hình Cài đặt."
            },
            new() {
                Id = 52,
                Title = "Thanh toán hóa đơn internet văn phòng",
                Description = "Thanh toán cước internet cáp quang hàng tháng qua ứng dụng ngân hàng.",
                ProjectName = "Công việc cá nhân",
                DueDate = d25,
                DueTime = new TimeSpan(17, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Lưu lại biên lai thanh toán vào thư mục tài chính."
            },
            new() {
                Id = 53,
                Title = "Tổng hợp nhật ký công việc ngày cao điểm",
                Description = "Ghi chép lại các vấn đề đã xử lý và các hạng mục còn tồn đọng sang ngày mai.",
                ProjectName = "Công việc cá nhân",
                DueDate = d25,
                DueTime = new TimeSpan(18, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Chia sẻ tóm tắt vào kênh chat chung của team."
            },

            // =================================================================
            // NGÀY 26/11/2026 (THỨ NĂM) - 9 TASKS
            // =================================================================
            new() {
                Id = 54,
                Title = "Triển khai màn hình Chi tiết công việc",
                Description = "Hiển thị đầy đủ thông tin: Tiêu đề lớn, Dự án, Trạng thái, Mô tả, Hạn chót và Ghi chú.",
                ProjectName = "Dự án Mobile App",
                DueDate = d26,
                DueTime = new TimeSpan(9, 0, 0),
                Status = TaskStatus.InProgress,
                Note = "Tuân thủ nghiêm ngặt bố cục trong detail_task.png."
            },
            new() {
                Id = 55,
                Title = "Xử lý trạng thái Không tìm thấy công việc (Not Found)",
                Description = "Hiển thị giao diện thân thiện với icon cảnh báo và nút quay về khi nhận ID không hợp lệ.",
                ProjectName = "Dự án Mobile App",
                DueDate = d26,
                DueTime = new TimeSpan(10, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Kiểm tra với ID rác (id = -1) từ Reviewer Tools."
            },
            new() {
                Id = 56,
                Title = "Xây dựng Hộp thoại xác nhận mở lại việc",
                Description = "Hiển thị dialog xác nhận bắt buộc trước khi chuyển việc đã hoàn thành về Chưa hoàn thành.",
                ProjectName = "UI/UX Design",
                DueDate = d26,
                DueTime = new TimeSpan(11, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Tiêu đề: 'Mở lại công việc?' kèm 2 nút Hủy và Xác nhận."
            },
            new() {
                Id = 57,
                Title = "Tối ưu hóa kích thước vùng chạm tương tác",
                Description = "Rà soát mọi nút bấm và icon đảm bảo đạt kích thước tối thiểu chuẩn 44x44dp.",
                ProjectName = "Core Platform",
                DueDate = d26,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Giúp người dùng không bị bấm trượt khi thao tác bằng ngón tay cái."
            },
            new() {
                Id = 58,
                Title = "Kiểm tra tương thích Dark Theme toàn diện",
                Description = "Xác nhận màu nền, màu chữ và đường viền đạt độ tương phản chuẩn WCAG AA.",
                ProjectName = "UI/UX Design",
                DueDate = d26,
                DueTime = new TimeSpan(15, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Thử nghiệm chuyển đổi qua lại giữa Light và Dark mode 10 lần liên tục."
            },
            new() {
                Id = 59,
                Title = "Viết Unit Test cho TaskDetailViewModel",
                Description = "Kiểm thử các luồng điều hướng, cập nhật thông tin và xử lý xóa công việc.",
                ProjectName = "MyTasks App",
                DueDate = d26,
                DueTime = new TimeSpan(16, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đảm bảo độ bao phủ mã nguồn (Code Coverage) đạt trên 85%."
            },
            new() {
                Id = 60,
                Title = "Cấu hình build bản phát hành Release",
                Description = "Bật tối ưu hóa mã nguồn R8, gỡ bỏ log debug và cấu hình ký số Keystore.",
                ProjectName = "DevOps",
                DueDate = d26,
                DueTime = new TimeSpan(16, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Tạo file AAB sẵn sàng tải lên Google Play Console."
            },
            new() {
                Id = 61,
                Title = "Kiểm tra kích thước file đóng gói ứng dụng",
                Description = "Đảm bảo file cài đặt APK sau nén có dung lượng dưới mức 25MB.",
                ProjectName = "DevOps",
                DueDate = d26,
                DueTime = new TimeSpan(17, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Loại bỏ các kiến trúc vi xử lý không cần thiết."
            },
            new() {
                Id = 62,
                Title = "Chuẩn bị trang phục cho buổi thuyết trình demo",
                Description = "Ủi áo sơ mi và chuẩn bị phong thái tự tin cho buổi báo cáo sản phẩm.",
                ProjectName = "Công việc cá nhân",
                DueDate = d26,
                DueTime = new TimeSpan(18, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đến sớm 15 phút để test kết nối máy chiếu."
            },

            // =================================================================
            // NGÀY 27/11/2026 (THỨ SÁU) - 7 TASKS
            // =================================================================
            new() {
                Id = 63,
                Title = "Buổi báo cáo Demo sản phẩm v1.0",
                Description = "Trình diễn trực tiếp 4 màn hình chính, tính năng đổi theme, cỡ chữ và ngôn ngữ tức thì.",
                ProjectName = "MyTasks App",
                DueDate = d27,
                DueTime = new TimeSpan(9, 30, 0),
                Status = TaskStatus.InProgress,
                Note = "Trình chiếu cả trên thiết bị thật Android và Windows desktop."
            },
            new() {
                Id = 64,
                Title = "Ghi nhận phản hồi và đánh giá từ Reviewer",
                Description = "Tổng hợp danh sách các điểm khen ngợi và các đề xuất cải tiến cho phiên bản sau.",
                ProjectName = "MyTasks App",
                DueDate = d27,
                DueTime = new TimeSpan(11, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Phần lớn đánh giá cao tính mượt mà và giao diện token-driven chuẩn chỉ."
            },
            new() {
                Id = 65,
                Title = "Hoàn thiện tài liệu README.md nghiệm thu",
                Description = "Soạn thảo đầy đủ 7 nội dung bắt buộc: SDK, cách build, kiến trúc, 5 quyết định tự quyết.",
                ProjectName = "Core Platform",
                DueDate = d27,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Đính kèm ảnh chụp 4 màn hình chính, chế độ Dark Mode và Cỡ chữ lớn."
            },
            new() {
                Id = 66,
                Title = "Soạn thảo tài liệu ghi chú sử dụng AI (AI-NOTES.md)",
                Description = "Mô tả các công cụ AI đã dùng, 3 ví dụ tiêu biểu và 1 trường hợp AI tạo kết quả chưa phù hợp.",
                ProjectName = "Core Platform",
                DueDate = d27,
                DueTime = new TimeSpan(15, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Trình bày trung thực, nêu rõ quyết định và lý do xử lý của lập trình viên."
            },
            new() {
                Id = 67,
                Title = "Kiểm tra toàn bộ test suite lần cuối",
                Description = "Chạy lệnh dotnet test đảm bảo 100% các unit test đều vượt qua thành công.",
                ProjectName = "DevOps",
                DueDate = d27,
                DueTime = new TimeSpan(16, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Kết quả đạt 0 lỗi, 0 cảnh báo."
            },
            new() {
                Id = 68,
                Title = "Gắn tag phiên bản Release v1.0.0 trên Git",
                Description = "Tạo Git tag đánh dấu mốc hoàn thành toàn diện dự án Việc của tôi.",
                ProjectName = "DevOps",
                DueDate = d27,
                DueTime = new TimeSpan(17, 15, 0),
                Status = TaskStatus.Pending,
                Note = "Đính kèm Release notes tóm tắt các tính năng nổi bật."
            },
            new() {
                Id = 69,
                Title = "Tiệc liên hoan chúc mừng hoàn thành dự án",
                Description = "Cùng team đi ăn tối và nâng ly chúc mừng sản phẩm ra mắt thành công tốt đẹp.",
                ProjectName = "Công việc cá nhân",
                DueDate = d27,
                DueTime = new TimeSpan(18, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Địa điểm: Nhà hàng gần công ty."
            },

            // =================================================================
            // NGÀY 28/11/2026 (THỨ BẢY) - 5 TASKS
            // =================================================================
            new() {
                Id = 70,
                Title = "Nghỉ ngơi và thư giãn sau kỳ chạy nước rút",
                Description = "Tận hưởng ngày cuối tuần thảnh thơi, nghe nhạc và đọc sách yêu thích.",
                ProjectName = "Công việc cá nhân",
                DueDate = d28,
                DueTime = new TimeSpan(9, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Không mở máy tính công việc trong buổi sáng."
            },
            new() {
                Id = 71,
                Title = "Xem lại nhật ký phát triển dự án",
                Description = "Đúc rút các kinh nghiệm quý báu về kiến trúc MVVM và quy chuẩn thiết kế Responsive.",
                ProjectName = "Công việc cá nhân",
                DueDate = d28,
                DueTime = new TimeSpan(14, 0, 0),
                Status = TaskStatus.Pending,
                Note = "Lưu lại bài viết lên blog kỹ thuật cá nhân."
            },
            new() {
                Id = 72,
                Title = "Khám phá các thư viện mã nguồn mở mới",
                Description = "Tìm hiểu các thư viện animation và biểu đồ hỗ trợ .NET MAUI mới phát hành.",
                ProjectName = "Core Platform",
                DueDate = d28,
                DueTime = new TimeSpan(15, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Cân nhắc áp dụng cho phiên bản cập nhật v1.1."
            },
            new() {
                Id = 73,
                Title = "Dọn dẹp góc làm việc và sắp xếp sách vở",
                Description = "Lau chùi màn hình, bàn phím và sắp xếp lại các thiết bị kiểm thử ngăn nắp.",
                ProjectName = "Công việc cá nhân",
                DueDate = d28,
                DueTime = new TimeSpan(16, 45, 0),
                Status = TaskStatus.Pending,
                Note = "Tạo không gian làm việc sạch sẽ, thoáng đãng."
            },
            new() {
                Id = 74,
                Title = "Lên kế hoạch học tập công nghệ mới cho tháng 12",
                Description = "Đặt mục tiêu nghiên cứu sâu hơn về trí tuệ nhân tạo (AI Agents) và Cloud Architecture.",
                ProjectName = "Công việc cá nhân",
                DueDate = d28,
                DueTime = new TimeSpan(17, 30, 0),
                Status = TaskStatus.Pending,
                Note = "Sẵn sàng cho các thử thách kỹ thuật lớn tiếp theo."
            }
        };

        return tasks;
    }
}
