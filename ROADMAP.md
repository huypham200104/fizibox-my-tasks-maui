# LỘ TRÌNH PHÁT TRIỂN DỰ ÁN (ROADMAP.MD)

Tài liệu theo dõi tiến độ thực hiện các tính năng của ứng dụng **MyTasksApp**. Cập nhật trạng thái `[x]` khi hoàn thành từng hạng mục.

---

## 📌 Tổng Quan Tiến Độ

- [x] **Giai đoạn 0: Khởi tạo dự án & Thiết lập quy tắc**
  - [x] Khởi tạo dự án .NET MAUI `MyTasksApp` (.NET 10).
  - [x] Cấu hình Solution `MyTasksApp.slnx` và `.gitignore` chuẩn.
  - [x] Build thử nghiệm Android thành công (0 Warning, 0 Error).
  - [x] Thiết lập bộ quy tắc hoạt động tại `AGENTS.md`.
  - [x] Phỏng vấn chốt 5 điểm tự quyết và viết tài liệu `BUSINESS.md`.

---

## 🚀 Các Giai Đoạn Thực Hiện Tiếp Theo

### Giai đoạn 1: Xây dựng Nền tảng Kiến trúc (Architecture & Foundation)
- [ ] **1.1. Core Layer (Domain)**
  - [ ] Khai báo Entity: `TaskItem`, `Project`, Enum `TaskPriority`, `TaskGroupType`.
  - [ ] Khai báo Interface dịch vụ: `ITaskService`, `ISettingsService`, `ILocalizationService`.
- [ ] **1.2. MVVM Base thuần C# (Không cài NuGet ngoài)**
  - [ ] Xây dựng `ObservableObject` kế thừa `INotifyPropertyChanged`.
  - [ ] Xây dựng `RelayCommand` và `AsyncRelayCommand` kế thừa `ICommand`.
  - [ ] Xây dựng `BaseViewModel` quản lý `IsBusy`, `Title`, và các cờ trạng thái chung.
- [ ] **1.3. Infrastructure Layer (Services & Mock Data)**
  - [ ] Xây dựng `MockTaskService` (In-memory, nạp sẵn dữ liệu mẫu thực tế, hỗ trợ kích hoạt giả lập Loading/Empty/Not Found).
  - [ ] Xây dựng `PreferencesSettingsService` (Lưu Theme, Cỡ chữ, Ngôn ngữ qua `Preferences`).
  - [ ] Xây dựng `LocalizationService` (Quản lý từ điển song ngữ VI/EN, hỗ trợ cập nhật nhãn tức thì không cần restart).
- [ ] **1.4. Design System & Dynamic Resources**
  - [ ] Thiết lập bảng màu tập trung (Primary, Background, Surface, Text, Danger, Warning, Success) cho cả Light/Dark Mode.
  - [ ] Thiết lập Dynamic Resource cho cỡ chữ (Nhỏ: 13/14px, Thường: 15/16px, Lớn: 18/20px).

---

### Giai đoạn 2: Shell Navigation & Màn hình 4 (Cài đặt & Reviewer Tools)
- [ ] **2.1. Cấu hình AppShell & DI**
  - [ ] Cấu hình TabBar 3 tab: Hôm nay, Tuần này, Cài đặt.
  - [ ] Đăng ký Services và ViewModels vào DI container (`MauiProgram.cs`).
  - [ ] Đăng ký Route cho trang Chi tiết (`TaskDetailPage`).
- [ ] **2.2. Xây dựng Màn hình 4 — Cài đặt (SettingsPage & SettingsViewModel)**
  - [ ] Bộ chọn Theme: Sáng / Tối (Áp dụng ngay trên toàn app).
  - [ ] Bộ chọn Cỡ chữ: Nhỏ / Thường / Lớn (Áp dụng ngay không restart).
  - [ ] Bộ chọn Ngôn ngữ: Tiếng Việt / English (Cập nhật ngay tất cả nhãn).
  - [ ] Khu vực Reviewer Tools:
    - [ ] Switch giả lập Loading.
    - [ ] Switch giả lập Empty.
    - [ ] Nút mở thử công việc không tồn tại (Test Not Found).
    - [ ] Nút "Đặt lại dữ liệu mẫu" (Reset mock data).

---

### Giai đoạn 3: Màn hình 1 — Hôm nay (TodayPage)
- [ ] **3.1. TodayViewModel**
  - [ ] Lấy danh sách việc từ `ITaskService`, tự động phân loại thành 3 nhóm: Quá hạn, Hôm nay, Xong.
  - [ ] Lắng nghe sự kiện thay đổi dữ liệu từ `ITaskService` để cập nhật đồng bộ.
  - [ ] Xử lý Command hoàn thành / bỏ hoàn thành công việc.
- [ ] **3.2. TodayPage UI (XAML)**
  - [ ] Giao diện phân nhóm 3 section rõ ràng, trực quan.
  - [ ] Hỗ trợ thao tác chạm checkbox hoặc vuốt (SwipeView) để hoàn thành.
  - [ ] Giao diện trạng thái Loading.
  - [ ] Giao diện trạng thái Empty với thông điệp khích lệ.

---

### Giai đoạn 4: Màn hình 2 — Tuần này (WeekPage)
- [ ] **4.1. WeekViewModel**
  - [ ] Tính toán dải 7 ngày trong tuần (Thứ Hai đến Chủ Nhật).
  - [ ] Tính số lượng công việc cho từng ngày (hiển thị badge).
  - [ ] Quản lý ngày đang chọn và giữ nguyên khi quay lại tab.
  - [ ] Lọc danh sách công việc theo ngày được chọn.
- [ ] **4.2. WeekPage UI (XAML)**
  - [ ] Dải chọn 7 ngày dạng ngang (Horizontal Week Strip) tương tác mượt mà.
  - [ ] Highlight ngày được chọn và ngày hôm nay.
  - [ ] Danh sách việc theo ngày với trạng thái Loading và Empty.

---

### Giai đoạn 5: Màn hình 3 — Chi tiết việc (TaskDetailPage)
- [ ] **5.1. TaskDetailViewModel**
  - [ ] Nhận `TaskId` qua Navigation query parameters.
  - [ ] Tải thông tin việc từ `ITaskService`.
  - [ ] Xử lý trạng thái Not Found khi không tìm thấy việc.
  - [ ] Command chuyển trạng thái Hoàn thành.
  - [ ] Command Mở lại công việc: Kích hoạt Popup/Dialog xác nhận bắt buộc.
  - [ ] Command chỉnh sửa & lưu Ghi chú vào mock data.
- [ ] **5.2. TaskDetailPage UI (XAML)**
  - [ ] Bố cục chi tiết: Tiêu đề, Dự án, Hạn chót, Trạng thái, Mô tả, Khung ghi chú.
  - [ ] Giao diện xử lý Not Found thân thiện kèm nút quay về.

---

### Giai đoạn 6: Hoàn thiện Hồ sơ & Nghiệm thu
- [ ] **6.1. Soạn thảo `README.md` (Đầy đủ 7 nội dung bắt buộc theo ASSIGNMENT.md)**
  - [ ] 1. Phiên bản SDK, .NET MAUI, công cụ, thiết bị test.
  - [ ] 2. Các bước cài đặt, build và chạy.
  - [ ] 3. Cấu trúc project & kiến trúc Clean Architecture.
  - [ ] 4. Quyết định thiết kế & 5 giả định nghiệp vụ tự quyết.
  - [ ] 5. Hướng dẫn kích hoạt các trạng thái giả lập (Loading, Empty, Not Found).
  - [ ] 6. Ảnh chụp 4 màn hình (kèm Light/Dark mode, Cỡ chữ lớn).
  - [ ] 7. Các trường hợp đã test & phần chưa kiểm chứng.
- [ ] **6.2. Soạn thảo `AI-NOTES.md`**
  - [ ] Công cụ AI đã dùng và cách phân chia công việc.
  - [ ] 3 ví dụ tiêu biểu (Prompt -> Kết quả -> Quyết định & lý do).
  - [ ] 1 trường hợp AI tạo kết quả chưa phù hợp và cách giải quyết.
- [ ] **6.3. Kiểm thử tổng thể & Tối ưu trên Android Emulator/Thiết bị thật**
