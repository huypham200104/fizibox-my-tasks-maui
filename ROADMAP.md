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
  - [x] Cập nhật toàn diện `BUSINESS.md` và `DESIGN.md` bám sát bộ thiết kế `ui_design/`.

---

## 🚀 Các Giai Đoạn Thực Hiện Tiếp Theo

### Giai đoạn 1: Xây dựng Nền tảng Kiến trúc (Architecture & Foundation)
- [x] **1.1. Core Layer (Domain)**
  - [x] Khai báo Entity: `TaskItem`, `Project`, Enum `TaskStatus` (`Pending`, `InProgress`, `Completed`), `TaskGroupType`, `HistoryFilterType`.
  - [x] Khai báo Interface dịch vụ: `ITaskService`, `ISettingsService`, `ILocalizationService`, `ISnackbarService`.
- [x] **1.2. MVVM Base thuần C# (Không cài NuGet ngoài)**
  - [x] Xây dựng `ObservableObject` kế thừa `INotifyPropertyChanged`.
  - [x] Xây dựng `RelayCommand` và `AsyncRelayCommand` kế thừa `ICommand`.
  - [x] Xây dựng `BaseViewModel` quản lý `IsBusy`, `Title`, `Strings`, và cơ chế lắng nghe sự kiện chung.
- [x] **1.3. Infrastructure Layer (Services & Mock Data)**
  - [x] Xây dựng `MockTaskService` (In-memory, nạp sẵn bộ dữ liệu mẫu thực tế 16–22/11/2026, hỗ trợ kích hoạt giả lập Loading/Empty/Not Found, ngăn xếp Undo xóa việc).
  - [x] Xây dựng `PreferencesSettingsService` (Lưu Theme, Cỡ chữ, Ngôn ngữ qua `Preferences`).
  - [x] Xây dựng `LocalizationService` (Quản lý từ điển song ngữ VI/EN, cập nhật nhãn tức thì không cần restart).
  - [x] Xây dựng `SnackbarService` (Quản lý hiển thị thông báo nổi 4 trạng thái: Thành công, Lỗi, Cảnh báo, Hoàn tác).
- [x] **1.4. Design System & Dynamic Resources**
  - [x] Thiết lập bảng màu tập trung (Primary Indigo, Background, Surface, Text, Danger, Warning, Success) cho cả Light/Dark Mode.
  - [x] Thiết lập Dynamic Resource cho cỡ chữ (Nhỏ: 13/14px, Thường: 15/16px, Lớn: 18/20px).
  - [x] Thiết lập Tokens khoảng cách (`SpacingXs` đến `SpacingXxl`), bo góc (`RadiusSm` đến `RadiusFull`).

---

### Giai đoạn 2: Shell Navigation 4 Tab & Màn hình Cài đặt (SettingsPage)
- [x] **2.1. Cấu hình AppShell & DI Container**
  - [x] Cấu hình TabBar 4 tab: Hôm nay (`TodayPage`), Tuần này (`WeekPage`), Đã xong (`HistoryPage`), Cài đặt (`SettingsPage`).
  - [x] Đăng ký Services và ViewModels vào DI container (`MauiProgram.cs`).
  - [x] Đăng ký Route cho trang Chi tiết (`TaskDetailPage`).
- [x] **2.2. Xây dựng Màn hình Cài đặt (SettingsPage & SettingsViewModel)**
  - [x] Bộ chọn Theme dạng 2 Card trực quan Sáng / Tối (Áp dụng ngay trên toàn app).
  - [x] Bộ điều khiển Cỡ chữ Segmented: Nhỏ / Thường / Lớn (Áp dụng ngay không restart).
  - [x] Bộ chọn Ngôn ngữ có cờ quốc gia: Tiếng Việt 🇻🇳 / English 🇬🇧 (Cập nhật tức thì tất cả nhãn).
  - [x] Khối "Xem trước" (Live Preview Card) cập nhật real-time giao diện thẻ việc mẫu.
  - [x] Khu vực Reviewer Tools:
    - [x] Nút "Xem thử" Trạng thái đang tải.
    - [x] Nút "Xem thử" Danh sách trống.
    - [x] Nút "Xem thử" Không tìm thấy công việc (Điều hướng `id=-1`).
    - [x] Nút "Đặt lại dữ liệu mẫu" (Reset mock data).
  - [x] Khối Thông tin ứng dụng (App Icon, MyTasks v1.0.0).

---

### Giai đoạn 3: Màn hình 1 — Hôm nay (TodayPage)
- [x] **3.1. TodayViewModel**
  - [x] Lấy danh sách việc từ `ITaskService`, tự động phân loại thành 3 nhóm: Quá hạn, Hôm nay, Xong.
  - [x] Tự động tính toán tỷ lệ % hoàn thành và tỷ số việc cho Thẻ tiến độ hôm nay.
  - [x] Lắng nghe sự kiện `TaskChanged` để cập nhật đồng bộ.
  - [x] Xử lý Command toggle hoàn thành / mở lại công việc.
  - [x] Xử lý Command tạo nhanh công việc mới từ nút FAB (+).
- [x] **3.2. TodayPage UI (XAML)**
  - [x] Header ngày tháng và câu chào truyền cảm hứng.
  - [x] Thẻ Tiến độ hôm nay dạng thanh ngang (Linear Progress Bar).
  - [x] Danh sách phân nhóm 3 section: QUÁ HẠN (đỏ), HÔM NAY (cam), XONG (xanh).
  - [x] Thẻ công việc chuẩn thiết kế (Checkbox tròn, Tag dự án, Badge hạn chót/giờ, Chevron).
  - [x] Vuốt SwipeView để hoàn thành hoặc xóa nhanh.
  - [x] Nút tròn nổi FAB (+) góc dưới phải.
  - [x] Giao diện trạng thái Shimmer Loading và Empty State.

---

### Giai đoạn 4: Màn hình 2 — Tuần này (WeekPage)
- [x] **4.1. WeekViewModel**
  - [x] Tính toán dải 7 ngày trong tuần từ Thứ Hai đến Chủ Nhật (chuẩn ISO 8601).
  - [x] Tính số lượng công việc cho từng ngày (hiển thị badge chấm tròn `• n`).
  - [x] Quản lý ngày đang chọn và giữ nguyên khi chuyển tab quay lại.
  - [x] Lọc danh sách công việc theo ngày được chọn và tính tỷ lệ hoàn thành của ngày đó.
- [x] **4.2. WeekPage UI (XAML)**
  - [x] Thanh điều hướng tuần `< dải ngày >`.
  - [x] Dải chọn 7 ngày ngang (Week Strip) tương tác mượt mà, highlight ngày hôm nay và ngày đang chọn.
  - [x] Thẻ Tóm tắt tiến độ ngày (Day Summary Card) với thanh tiến độ ngang %.
  - [x] Danh sách công việc theo ngày được chọn.
  - [x] Nút FAB (+) tạo công việc với ngày mặc định là ngày đang chọn.
  - [x] Giao diện trạng thái Shimmer Loading và Empty State.

---

### Giai đoạn 5: Màn hình 3 — Đã xong / Lịch sử (HistoryPage)
- [x] **5.1. HistoryViewModel**
  - [x] Lấy toàn bộ danh sách công việc đã hoàn thành từ `ITaskService`.
  - [x] Tìm kiếm realtime theo tiêu đề hoặc tên dự án.
  - [x] Bộ lọc thời gian: Tất cả, Hôm nay, Tuần này, Tháng này.
  - [x] Thẻ thống kê thành tích tự động cập nhật số lượng theo bộ lọc.
  - [x] Gom nhóm danh sách theo ngày hoàn thành: Hôm nay, Hôm qua, Ngày trước đó.
- [x] **5.2. HistoryPage UI (XAML)**
  - [x] Thẻ Thống kê thành tích nổi bật với icon tích xanh và viền nhấn xanh lá.
  - [x] Thanh tìm kiếm có icon kính lúp.
  - [x] Dải Chip lọc thời gian (Tất cả, Hôm nay, Tuần này, Tháng này).
  - [x] Danh sách công việc đã xong gom nhóm rõ ràng, chữ gạch ngang, hiển thị giờ hoàn thành.
  - [x] Chạm vào việc mở màn hình Chi tiết.
  - [x] Giao diện Empty State khi không có việc hoàn thành hoặc không có kết quả tìm kiếm.

---

### Giai đoạn 6: Trang Chi tiết & Hệ Thống Bottom Sheets / Modals
- [x] **6.1. TaskDetailViewModel**
  - [x] Nhận `TaskId` qua Navigation query parameters.
  - [x] Tải thông tin việc từ `ITaskService`.
  - [x] Xử lý trạng thái Not Found khi không tìm thấy việc (ID không hợp lệ hoặc `id=-1`).
  - [x] Command chuyển trạng thái (Hoàn thành / Mở lại).
  - [x] Command Mở lại công việc: Kích hoạt Popup/Dialog xác nhận bắt buộc.
  - [x] Command Chỉnh sửa thông tin (Tiêu đề, Mô tả, Ghi chú, Hạn ngày, Hạn giờ, Trạng thái).
  - [x] Command Xóa công việc: Kích hoạt xóa và điều hướng về kèm Snackbar Hoàn tác.
- [x] **6.2. TaskDetailPage UI (XAML) & Components**
  - [x] Bố cục chi tiết: Tiêu đề lớn, Tag Dự án, Chip Trạng thái, Khung Mô tả, Bảng thông tin, Khung Ghi chú.
  - [x] Menu 3 chấm `⋮` chứa tùy chọn *"Chỉnh sửa thông tin"* và *"Xóa công việc"*.
  - [x] Nút CTA chân trang: Solid Primary *"Hoàn thành công việc"* hoặc Outlined *"Mở lại công việc"*.
  - [x] Dialog Xác nhận mở lại việc (*"Mở lại công việc?"* - Hủy / Xác nhận).
  - [x] Giao diện Not Found thân thiện với icon cảnh báo và nút quay về.
- [x] **6.3. Bottom Sheets & Modals tương tác**
  - [x] Bottom Sheet "Chọn trạng thái" (`bottom_sheet_change_status`): 3 mức Chưa hoàn thành, Đang thực hiện, Đã hoàn thành.
  - [x] Bottom Sheet "Chọn ngày đến hạn" (`bottom_sheet_date_picker`): Lịch tháng, nút chọn "Hôm nay", Hủy / Xác nhận.
  - [x] Bottom Sheet "Chọn giờ đến hạn" (`bottom_sheet_time_picker`): Đồng hồ số lớn, các chip chọn giờ nhanh, Hủy / Xác nhận.
  - [x] Modal "Chỉnh sửa công việc" (`popup_edit_text`): Nhập tên (max 100), mô tả (max 1000), ghi chú (max 500) kèm bộ đếm ký tự.

---

### Giai đoạn 7: Hệ Thống Floating SnackBar & Hoàn Tác (Undo)
- [x] **7.1. Xây dựng Custom Floating SnackBar**
  - [x] Component hiển thị nổi trên TabBar.
  - [x] Hỗ trợ 4 kiểu: Thành công (xanh), Lỗi (đỏ), Cảnh báo (vàng), Hoàn tác xóa (tím/xám).
  - [x] Bộ đếm thời gian tự ẩn (`5000ms`) và nút đóng `✕`.
  - [x] Nút bấm *"Hoàn tác"* phục hồi ngay công việc vừa xóa.

---

### Giai đoạn 8: Hoàn thiện Hồ sơ & Nghiệm thu
- [x] **8.1. Soạn thảo `README.md` (Đầy đủ 7 nội dung bắt buộc theo ASSIGNMENT.md)**
  - [x] 1. Phiên bản SDK (.NET 10, MAUI), công cụ, thiết bị test.
  - [x] 2. Các bước cài đặt, build và chạy.
  - [x] 3. Cấu trúc project & kiến trúc Clean Architecture.
  - [x] 4. Quyết định thiết kế & 5 giả định nghiệp vụ tự quyết.
  - [x] 5. Hướng dẫn kích hoạt các trạng thái giả lập (Loading, Empty, Not Found, Reset mock data).
  - [x] 6. Ảnh chụp 4 màn hình chính, ảnh Dark Mode và Cỡ chữ lớn.
  - [x] 7. Các trường hợp đã test & phần chưa kiểm chứng.
- [x] **8.2. Soạn thảo `AI-NOTES.md`**
  - [x] Công cụ AI đã dùng và cách phân chia công việc.
  - [x] 3 ví dụ tiêu biểu (Prompt -> Kết quả -> Quyết định & lý do).
  - [x] 1 trường hợp AI tạo kết quả chưa phù hợp và cách giải quyết.
- [ ] **8.3. Kiểm thử tổng thể & Tối ưu trên Android Emulator**
  - [ ] Chạy mượt mà, không giật lag, 0 crash.
  - [ ] Kiểm tra tương thích Light/Dark Mode, Responsive Cỡ chữ Lớn, Song ngữ VI/EN.
