# TÀI LIỆU ĐẶC TẢ THIẾT KẾ MÀN HÌNH CHI TIẾT CÔNG VIỆC (TASK DETAIL SCREEN SPECIFICATION)

> **Dự án:** Việc của tôi (MyTasksApp) — .NET MAUI Mobile App (.NET 10)  
> **Màn hình:** Màn hình Chi tiết công việc (`TaskDetailPage`) & Hệ thống Bottom Sheets / Modals  
> **Tài liệu tham chiếu:** `BUSINESS.md`, `DESIGN.md`, `ROADMAP.md`, `ui_design/detail_task.png`, `ui_design/bottom_sheet_change_status.png`, `ui_design/bottom_sheet_date_picker.png`, `ui_design/bottom_sheet_time_picker.png`, `ui_design/popup_edit_text.png`, `ui_design/snack_bar.png`  
> **Nguyên tắc:** Clean Architecture, Pure MVVM (Zero 3rd-party MVVM packages), 100% Token-Driven, Responsive 320dp–412dp, Touch Target ≥ 44x44dp.

---

## 1. Mục Tiêu & Phạm Vi (Goals & Scope)

Màn hình **Chi tiết công việc (TaskDetailPage)** là màn hình hiển thị toàn bộ thông tin chi tiết và trung tâm thao tác cho một công việc cụ thể:
1. Nhận tham số điều hướng `id={taskId}` qua Shell Query Parameter (hỗ trợ cả trường hợp `id=-1` hoặc ID không tồn tại để hiển thị giao diện **Not Found State** chuẩn mực).
2. Hiển thị thông tin trực quan theo thiết kế `detail_task.png`:
   - Thanh Top Bar với nút quay lại `< Chi tiết công việc` và nút menu 3 chấm `⋮`.
   - Tiêu đề công việc lớn, Tag Dự án, Chip Trạng thái hiện tại.
   - Khối Mô tả công việc (Description Box).
   - Khối Thông tin công việc (Info Card): Hạn hoàn thành, Thời gian, Dự án, Trạng thái (hỗ trợ chạm vào từng dòng để mở Bottom Sheet tương ứng).
   - Khối Ghi chú (Notes Box) với icon sổ tay màu cam.
   - Nút hành động chính (CTA Button) ở chân trang:
     - Khi việc chưa xong: Nút Solid Primary *"Hoàn thành công việc"*.
     - Khi việc đã hoàn thành: Nút Outlined *"Mở lại công việc"*.
3. Hộp thoại xác nhận bắt buộc (Confirmation Dialogs):
   - **Mở lại công việc:** Hộp thoại xác nhận trước khi chuyển trạng thái về Chưa hoàn thành.
   - **Xóa công việc:** Hộp thoại xác nhận trước khi xóa, sau đó điều hướng về trang trước và hiển thị Floating Snackbar kèm nút Hoàn tác (Undo).
4. Hệ thống Bottom Sheets & Modals tương tác:
   - **Action Menu (Menu 3 chấm):** Mở Bottom Sheet tùy biến gồm 2 tùy chọn: *Chỉnh sửa thông tin* và *Xóa công việc*.
   - **Modal Chỉnh sửa công việc (`popup_edit_text.png`):** Sửa Tên (max 100 ký tự), Mô tả (max 1000 ký tự), Ghi chú (max 500 ký tự) có bộ đếm ký tự.
   - **Bottom Sheet Chọn trạng thái (`bottom_sheet_change_status.png`):** Chọn 1 trong 3 trạng thái: Chưa hoàn thành, Đang thực hiện, Đã hoàn thành.
   - **Bottom Sheet Chọn ngày đến hạn (`bottom_sheet_date_picker.png`):** Điều hướng tháng, lưới lịch chuẩn ISO (T2-CN), phím tắt "Hôm nay", Hủy / Xác nhận.
   - **Bottom Sheet Chọn giờ đến hạn (`bottom_sheet_time_picker.png`):** Giờ số lớn `14:00`, các chip chọn nhanh (`09:00`, `12:00`, `14:00`, `17:00`), bộ chọn giờ/phút thuần MAUI, Hủy / Xác nhận.
   - **Bottom Sheet Chọn dự án:** Cho phép chuyển đổi dự án cho công việc từ danh sách dự án có sẵn.

---

## 2. Các Quyết Định Đã Thống Nhất (/grill-me)

1. **Phạm vi triển khai:** Triển khai trọn vẹn toàn bộ Giai đoạn 6 của Roadmap, bao gồm giao diện Chi tiết công việc, ViewModel, 5 Bottom Sheets/Modals tùy biến chuẩn thiết kế và 2 Dialog xác nhận.
2. **Menu 3 chấm (⋮):** Sử dụng Custom Bottom Sheet Action Menu đồng bộ thiết kế với 2 mục: "Chỉnh sửa thông tin" và "Xóa công việc" (màu đỏ cảnh báo).
3. **Bộ chọn Giờ (Time Picker):** Sử dụng Custom Time Selector thuần MAUI: Hiển thị số giờ lớn (14:00), hàng chip chọn nhanh (`09:00`, `12:00`, `14:00`, `17:00`) và lưới chọn Giờ/Phút trực quan không dùng NuGet ngoài.
4. **Trường Dự án (Project):** Cho phép chọn Dự án qua Bottom Sheet danh sách dự án khi chạm vào dòng 'Dự án' trong bảng Thông tin công việc, và cũng có thể chọn trong Modal Chỉnh sửa.
5. **Luồng Xóa việc:** Hiển thị Hộp thoại xác nhận trước khi xóa việc. Khi bấm Xác nhận -> xóa việc trong `ITaskService`, tự động quay về trang trước và kích hoạt Floating Snackbar có nút Hoàn tác (Undo).

---

## 3. Kiến Trúc & Thiết Kế Thành Phần (Architecture & Components)

### 3.1. `TaskDetailViewModel : BaseViewModel, IQueryAttributable`
Nằm tại: `src/MyTasksApp/ViewModels/TaskDetailViewModel.cs`
* **Dependencies:**
  * `ITaskService` (Singleton)
  * `ILocalizationService` (Singleton)
* **Kế thừa & Giao diện:**
  * Kế thừa `BaseViewModel` để có `IsBusy`, `Title`, `Strings`, `SetProperty`.
  * Thực thi `IQueryAttributable` để nhận `id` từ `Shell.Current.GoToAsync("TaskDetailPage?id=...")`.
* **Thuộc tính Observable (Observable Properties):**
  * `CurrentTask`: `TaskItem?` (Dữ liệu công việc đang xem).
  * `IsNotFound`: `bool` (Bật `true` khi `id` không hợp lệ, `id <= 0`, hoặc không tìm thấy việc trong `ITaskService`).
  * `NotFoundMessage`: `string`.
  * Trạng thái hiển thị Modals / Bottom Sheets:
    * `IsActionMenuVisible`: `bool` (Menu 3 chấm: Sửa / Xóa).
    * `IsEditModalVisible`: `bool` (Modal popup_edit_text).
    * `IsStatusSheetVisible`: `bool` (Bottom sheet chọn 3 trạng thái).
    * `IsDatePickerSheetVisible`: `bool` (Bottom sheet lịch ngày).
    * `IsTimePickerSheetVisible`: `bool` (Bottom sheet chọn giờ).
    * `IsProjectSheetVisible`: `bool` (Bottom sheet chọn dự án).
    * `IsReopenConfirmVisible`: `bool` (Dialog xác nhận mở lại việc).
    * `IsDeleteConfirmVisible`: `bool` (Dialog xác nhận xóa việc).
  * Thuộc tính tạm thời cho Modal Chỉnh sửa (Form Edit State):
    * `EditTitle`: `string` + `EditTitleCharCountText` (`21/100`).
    * `EditDescription`: `string` + `EditDescCharCountText` (`58/1000`).
    * `EditNote`: `string` + `EditNoteCharCountText` (`37/500`).
    * `EditProjectName`: `string`.
  * Thuộc tính tạm thời cho Date Picker Sheet:
    * `PickerCalendarMonth`: `DateTime` (Tháng đang xem trên lịch).
    * `PickerMonthText`: `string` (ví dụ: *"Tháng 11, 2026"*).
    * `PickerSelectedDate`: `DateTime`.
    * `PickerSelectedDateFormatted`: `string` (ví dụ: *"Thứ Sáu, 20 tháng 11, 2026"*).
    * `CalendarDays`: `ObservableCollection<CalendarDayItem>` (42 ô ngày trên lưới 7 cột).
  * Thuộc tính tạm thời cho Time Picker Sheet:
    * `PickerSelectedTime`: `TimeSpan`.
    * `PickerTimeFormatted`: `string` (ví dụ: `"14:00"`).
    * `PickerSelectedHour`: `int` (0–23).
    * `PickerSelectedMinute`: `int` (0–59).
  * Thuộc tính tạm thời cho Status Sheet:
    * `SelectedTempStatus`: `TaskStatus`.
  * Danh sách dự án sẵn có cho Project Sheet:
    * `AvailableProjects`: `ObservableCollection<string>`.
* **Commands:**
  * `LoadTaskCommand`: Tải dữ liệu công việc từ `ITaskService.GetTaskByIdAsync(id)`.
  * `GoBackCommand`: Quay lại trang trước (`Shell.Current.GoToAsync("..")`).
  * `OpenActionMenuCommand` / `CloseActionMenuCommand`.
  * `OpenEditModalCommand` / `CloseEditModalCommand` / `SaveEditCommand`.
  * `OpenStatusSheetCommand` / `CloseStatusSheetCommand` / `SelectStatusOptionCommand` / `ApplyStatusSheetCommand`.
  * `OpenDatePickerSheetCommand` / `CloseDatePickerSheetCommand` / `PrevMonthCommand` / `NextMonthCommand` / `SelectDateDayCommand` / `SelectTodayShortcutCommand` / `ApplyDatePickerCommand`.
  * `OpenTimePickerSheetCommand` / `CloseTimePickerSheetCommand` / `SelectQuickTimePresetCommand` / `SelectHourCommand` / `SelectMinuteCommand` / `ApplyTimePickerCommand`.
  * `OpenProjectSheetCommand` / `CloseProjectSheetCommand` / `SelectProjectCommand`.
  * `ToggleMainStatusCommand`:
    * Nếu chưa xong: Chuyển sang `Completed`, cập nhật `CompletedAt = DateTime.Now`.
    * Nếu đã xong: Mở `IsReopenConfirmVisible = true`.
  * `ConfirmReopenCommand`: Chuyển trạng thái sang `Pending`, xóa `CompletedAt`, đóng dialog.
  * `CancelReopenCommand`: Đóng dialog mở lại.
  * `RequestDeleteCommand`: Mở `IsDeleteConfirmVisible = true` (từ menu 3 chấm).
  * `ConfirmDeleteCommand`: Gọi `ITaskService.DeleteTaskAsync(taskId)`, quay về trang trước, kích hoạt Snackbar Hoàn tác.
  * `CancelDeleteCommand`: Đóng dialog xóa.

---

## 4. Đặc Tả Giao Diện Người Dùng (UI Layout Specification)

### 4.1. Bố Cục `TaskDetailPage.xaml`
* **Nền tảng:** `Grid` chính với 2 lớp:
  - **Lớp 1 (Nội dung chính):**
    - `Top Bar`: `Border` mỏng phía trên có `Shell.NavBarIsVisible="False"`. Nút back `<` (`44x44dp`), Tiêu đề `"Chi tiết công việc"`, Nút menu `⋮` (`44x44dp`).
    - `ScrollView`: Chứa toàn bộ nội dung cuộn mượt mà:
      1. Tiêu đề lớn (`FontSizeDisplay`, `InterBold`, `TextPrimary`).
      2. Hàng Tag Dự án (`PrimaryLight`, bo góc `12dp`) + Chip Trạng thái (`RoundRectangle 999`, viền theo trạng thái).
      3. Section `Mô tả công việc`: Icon văn bản + Khung `Surface` bo góc `16dp`, padding `16dp`, văn bản mô tả `TextPrimary`.
      4. Section `Thông tin công việc`: Khung `Surface` bo góc `16dp` chứa 4 hàng (Hạn hoàn thành, Thời gian, Dự án, Trạng thái) với icon, nhãn, giá trị và mũi tên `ChevronRight` mở bộ chọn.
      5. Section `Ghi chú`: Icon sổ tay cam + Khung `Surface` bo góc `16dp`, vệt màu vàng cam bên trái (`Warning`), nội dung ghi chú.
      6. Khoảng đệm `Padding` đáy lớn để nút CTA không che khuất nội dung.
    - `Bottom Fixed Action Bar`: Khung chân trang chứa nút bấm CTA:
      - Khi chưa xong: Nút Solid `Primary` nền tím indigo, icon `CheckmarkCircle`, chữ trắng *"Hoàn thành công việc"*.
      - Khi đã xong: Nút Outlined viền `Primary`, icon `ArrowSync`, chữ `Primary` *"Mở lại công việc"*.
  - **Lớp 2 (Hệ thống Overlay Modals & Dialogs):**
    - **A. Custom Action Sheet Menu (Menu 3 chấm):** Nổi ở góc trên phải hoặc Bottom Sheet tinh gọn với 2 tùy chọn "Chỉnh sửa thông tin" và "Xóa công việc".
    - **B. Modal Chỉnh sửa công việc (`popup_edit_text.png`):** Bottom Sheet bo góc trên `24dp`, có Drag handle, nút đóng `✕`, 3 ô nhập kèm bộ đếm ký tự thực, 2 nút Hủy và Lưu thay đổi.
    - **C. Bottom Sheet Chọn trạng thái (`bottom_sheet_change_status.png`):** 3 Card độc lập (Chưa hoàn thành, Đang thực hiện, Đã hoàn thành) có icon, mô tả và dấu check tím bên phải mục đang chọn.
    - **D. Bottom Sheet Chọn ngày (`bottom_sheet_date_picker.png`):** Header ngày chọn, điều hướng tháng, lưới lịch 7 cột, phím tắt "Hôm nay", 2 nút chân trang Hủy / Xác nhận.
    - **E. Bottom Sheet Chọn giờ (`bottom_sheet_time_picker.png`):** Số giờ lớn `14:00`, 4 chip chọn nhanh (`09:00`, `12:00`, `14:00`, `17:00`), lưới chọn giờ/phút, 2 nút chân trang Hủy / Xác nhận.
    - **F. Bottom Sheet Chọn dự án:** Danh sách các dự án để người dùng chạm chọn nhanh.
    - **G. Confirmation Dialog Mở lại việc (`detail_task.png`):** Card nổi giữa màn hình trên nền tối mờ: Tiêu đề *"Mở lại công việc?"*, câu hỏi xác nhận, 2 nút *"Hủy"* và *"Xác nhận"*.
    - **H. Confirmation Dialog Xóa việc:** Card nổi giữa màn hình: Tiêu đề *"Xóa công việc?"*, thông báo xác nhận, 2 nút *"Hủy"* và *"Xóa"* (nút Xóa màu `Danger`).
    - **I. Giao diện Not Found State:** Hiển thị toàn màn hình khi `IsNotFound == true`, icon `DocumentError` cỡ 64dp màu đỏ, tiêu đề, thông điệp giải thích và nút quay lại danh sách.

---

## 5. Kế Hoạch Triển Khai Chi Tiết (Implementation Plan Preview)

1. **Bước 1:** Bổ sung các chuỗi ngôn ngữ còn thiếu vào `LocalizationService` (song ngữ VI/EN cho trang Chi tiết, các nhãn modal, dialog và thông báo).
2. **Bước 2:** Xây dựng `CalendarDayItem` model hỗ trợ lưới lịch cho Date Picker Sheet.
3. **Bước 3:** Xây dựng `TaskDetailViewModel` hoàn chỉnh kế thừa `BaseViewModel` và `IQueryAttributable`, xử lý đầy đủ các tương tác, trạng thái, modal và liên kết với `ITaskService`.
4. **Bước 4:** Cập nhật đăng ký DI trong `MauiProgram.cs` (`builder.Services.AddTransient<TaskDetailViewModel>();`).
5. **Bước 5:** Xây dựng giao diện XAML `TaskDetailPage.xaml` và code-behind `TaskDetailPage.xaml.cs` bám sát `DESIGN.md` và các ảnh thiết kế `ui_design/`.
6. **Bước 6:** Biên dịch (Build) giải pháp để kiểm tra 0 Warning, 0 Error.
7. **Bước 7:** Cập nhật `ROADMAP.md` đánh dấu hoàn thành Giai đoạn 6.