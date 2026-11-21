# TÀI LIỆU ĐẶC TẢ THIẾT KẾ MÀN HÌNH TUẦN NÀY (WEEKPAGE SPECIFICATION)

> **Dự án:** Việc của tôi (MyTasksApp) — .NET MAUI Mobile App (.NET 10)  
> **Màn hình:** Màn hình 2 — Tuần này (`WeekPage`)  
> **Tài liệu tham chiếu:** `BUSINESS.md`, `DESIGN.md`, `ROADMAP.md`, `ui_design/02.week_screen.png`  
> **Nguyên tắc:** Clean Architecture, Pure MVVM (Zero 3rd-party MVVM packages), 100% Token-Driven, Responsive 320dp–412dp, Touch Target ≥ 44x44dp.

---

## 1. Mục Tiêu & Phạm Vi (Goals & Scope)

Màn hình **Tuần này (WeekPage)** là màn hình trung tâm thứ hai của ứng dụng (Tab 2 trên AppShell TabBar), cho phép người dùng:
1. Có cái nhìn toàn cảnh về lịch trình 7 ngày trong tuần (từ Thứ Hai đến Chủ Nhật theo chuẩn ISO 8601).
2. Dễ dàng điều hướng tuần trước / tuần sau (`<` `>`) và chuyển đổi nhanh giữa các ngày trong tuần.
3. Xem tóm tắt tiến độ thực hiện công việc của từng ngày với tỷ số và thanh tiến độ phần trăm (`%`).
4. Quản lý danh sách công việc của ngày được chọn: Sắp xếp theo mốc giờ tăng dần (`DueTime`), hỗ trợ đánh dấu hoàn thành/mở lại tức thì (in-place status toggle with strikethrough), vuốt SwipeView để hoàn thành/xóa nhanh, và Undo trong 5 giây.
5. Tạo nhanh công việc mới bằng nút FAB (`+`) với ngày hạn chót mặc định là ngày đang xem.

---

## 2. Các Quyết Định Nghiệp Vụ Đã Thống Nhất (/grill-me)

1. **Header & Top Bar:** Đồng bộ hoàn toàn với màn Hôm nay: Giữ Top Brand Bar (Logo MyTasks bo tròn + Tên app + Chip dải tuần), Tiêu đề lớn *"Tuần này"* (tinh gọn, lược bỏ Avatar và Chuông thông báo để đồng nhất 4 tab).
2. **Cơ chế chuyển tuần (`<` `>`):** Khi chuyển tuần, giữ nguyên thứ tương ứng (ví dụ: đang xem Thứ Sáu tuần này thì bấm `>` sang tuần sau sẽ tự động chọn Thứ Sáu tuần sau).
3. **Trạng thái ngày chọn (Điểm tự quyết 3):** Khi người dùng chuyển sang tab khác rồi quay lại, ngày đang chọn được bảo toàn nguyên vẹn nhờ ViewModel Singleton.
4. **Thứ tự công việc trong ngày:** Sắp xếp theo thứ tự thời gian (`DueTime`). Khi một công việc được tích hoàn thành, công việc giữ nguyên vị trí trên timeline của ngày, gạch ngang chữ và hiện tích xanh (không bị đẩy xáo trộn vị trí).
5. **Nút FAB (+) & Tạo việc:** Tự động điền ngày hạn chót mặc định là ngày đang chọn. Nếu người dùng chọn hạn chót sang một ngày khác ngày đang xem: Màn hình giữ nguyên ngày đang xem, cập nhật ngay số lượng badge `• n` trên thanh tuần và hiển thị Snackbar thông báo tạo thành công.
6. **Empty State:** Khi một ngày không có công việc nào, hiển thị icon `CalendarEmpty`, tiêu đề *"Không có công việc"*, chú thích *"Ngày này chưa có công việc nào được lên lịch"* kèm nút CTA *"Lên lịch công việc"* mở modal tạo việc cho ngày đó.

---

## 3. Kiến Trúc & Thiết Kế Thành Phần (Component Architecture)

### 3.1. Sub-model `WeekDayItem : ObservableObject`
Nằm tại: `src/MyTasksApp/Core/Models/WeekDayItem.cs` hoặc `src/MyTasksApp/ViewModels/WeekDayItem.cs`
* `DateTime Date`: Ngày trong tuần.
* `string DayOfWeekShort`: Tên thứ viết tắt tiếng Việt (`T2`, `T3`, `T4`, `T5`, `T6`, `T7`, `CN`).
* `string DayNumber`: Ngày trong tháng (`16`, `17`, ...).
* `int TaskCount`: Số lượng công việc trong ngày.
* `string TaskCountText`: Chuỗi hiển thị chấm tròn kèm số lượng (`• 4` hoặc `• 0`).
* `bool IsToday`: Cờ nhận diện ngày hôm nay thực tế.
* `bool IsSelected`: Cờ nhận diện ngày đang được người dùng bấm chọn.

### 3.2. `WeekViewModel : BaseViewModel`
Nằm tại: `src/MyTasksApp/ViewModels/WeekViewModel.cs`
* **Dependencies:** `ITaskService` (Singleton).
* **Properties:**
  * `CurrentWeekStart`: `DateTime` (Thứ Hai của tuần đang hiển thị).
  * `WeekRangeText`: `string` (ví dụ: *"16 – 22 Tháng 11, 2026"*).
  * `WeekDays`: `ObservableCollection<WeekDayItem>` (7 ngày cố định).
  * `SelectedDate`: `DateTime` (Ngày đang chọn xem công việc).
  * `SelectedDayTitle`: `string` (ví dụ: *"Thứ Sáu, 20 tháng 11"*).
  * `DayTasks`: `ObservableCollection<TaskItem>` (Danh sách việc có `DueDate.Date == SelectedDate.Date`, sắp xếp tăng dần theo `DueTime`).
  * `DayTotalTasksCount`, `DayCompletedTasksCount`: `int`.
  * `DayProgressRatio`: `double` (0.0 -> 1.0).
  * `DayProgressPercentageText`: `string` (ví dụ: `"50%"`).
  * `DayProgressRatioText`: `string` (ví dụ: `"4 công việc • 2/4 hoàn thành"`).
  * `IsRefreshing`: `bool`.
  * `IsEmpty`: `bool` (Khi `DayTasks.Count == 0` và không trong trạng thái `IsBusy`).
  * Modal Create Task properties: `IsCreateModalVisible`, `NewTaskTitle`, `NewTaskDescription`, `NewTaskProject`, `NewTaskDueDate`, `NewTaskDueTime`.
  * Snackbar properties: `IsSnackbarVisible`, `SnackbarMessage`, `CanUndo`.
* **Commands:**
  * `LoadWeekDataCommand`: Tải dữ liệu toàn bộ tuần và ngày đang chọn.
  * `RefreshCommand`: Kéo để làm mới (`Pull-to-refresh`).
  * `PreviousWeekCommand`: Lùi 1 tuần (`CurrentWeekStart.AddDays(-7)`).
  * `NextWeekCommand`: Tiến 1 tuần (`CurrentWeekStart.AddDays(7)`).
  * `SelectDayCommand`: Nhận tham số `DateTime` hoặc `WeekDayItem`, cập nhật `SelectedDate` và tải danh sách việc.
  * `ToggleTaskStatusCommand`: Đánh dấu hoàn thành / mở lại việc.
  * `DeleteTaskCommand`: Xóa việc kèm Undo.
  * `UndoDeleteCommand`: Hoàn tác việc vừa xóa.
  * `OpenTaskDetailCommand`: Điều hướng sang `TaskDetailPage?id={task.Id}`.
  * `OpenCreateModalCommand` / `CloseCreateModalCommand`: Mở / đóng modal tạo việc (ngày mặc định là `SelectedDate`).
  * `SaveNewTaskCommand`: Lưu công việc mới vào `ITaskService`.
  * `DismissSnackbarCommand`: Đóng snackbar.

---

## 4. Đặc Tả Giao Diện Người Dùng (UI Layout Specification)

### 4.1. Cấu Trúc Bố Cục `WeekPage.xaml`
* **`RefreshView`**: Bọc toàn bộ nội dung chính, hỗ trợ kéo làm mới mượt mà.
* **`ScrollView`**: `VerticalScrollBarVisibility="Never"`, padding đáy `110dp` để tránh bị FAB hoặc Bottom TabBar che khuất.
* **1. Top Brand Bar**:
  * Logo Squircle bo góc tròn `10dp` viền mỏng `BorderColor`.
  * Nhãn "MyTasks" (`InterBold`, cỡ 18) và "Việc của tôi" (`InterRegular`, cỡ 10).
  * Chip dải tuần bên phải: `Border` bo góc `20dp`, icon lịch `Calendar` + dải ngày tuần hiện tại.
* **2. Tiêu đề trang**: Label "Tuần này" (`FontSizeDisplay`, `InterBold`, `TextPrimary`).
* **3. Thanh điều hướng tuần (Week Navigation Bar)**:
  * Nút `<`: `Border` `WidthRequest="40"` `HeightRequest="40"` bo góc `12dp`, icon `ChevronLeft`.
  * Nhãn dải tuần ở giữa: Icon `Calendar` + nhãn ví dụ: *"16 – 22 Tháng 11, 2026"*.
  * Nút `>`: `Border` `WidthRequest="40"` `HeightRequest="40"` bo góc `12dp`, icon `ChevronRight`.
* **4. Dải 7 ngày (Week Strip 7-Column Grid)**:
  * `Grid ColumnDefinitions="*,*,*,*,*,*,*"` `ColumnSpacing="6"`.
  * Mỗi cột là 1 nút bấm / card ngày bo góc `12dp`, padding `4,8`:
    * Hàng 1: Thứ viết tắt (`T2` - `CN`), `FontSizeCaption`.
    * Hàng 2: Số ngày (`16` - `22`), `FontSizeTitle`, `FontFamily="InterBold"`.
    * Hàng 3: Badge số việc (`• n`), `FontSizeBadge`.
    * **Hiệu ứng trực quan:**
      * Ngày đang chọn (`IsSelected == true`): Nền đặc `Primary`, chữ trắng `TextOnPrimary`.
      * Ngày hôm nay (`IsToday == true && !IsSelected`): Viền `Primary` `2dp`, nền `Surface`, chữ `Primary`.
      * Ngày thường: Nền `Surface`, viền `BorderColor` `1dp`, chữ `TextPrimary`.
* **5. Thẻ Tóm tắt tiến độ ngày (Day Summary Card)**:
  * Bo góc `16dp`, nền `PrimaryLight`, viền `BorderColor`.
  * Hàng 1: Icon vuông bo góc màu `Primary` với icon `CalendarCheckMark` trắng + Tiêu đề ngày (ví dụ: *"Thứ Sáu, 20 tháng 11"*) + Dòng tóm tắt (*"4 công việc • 2/4 hoàn thành"*).
  * Hàng 2: Thanh tiến độ ngang (`ProgressBar`) vệt màu `ProgressFill` (Indigo-600) + Nhãn phần trăm (ví dụ: *"50%"*).
* **6. Danh sách công việc (Task Cards 2 tầng)**:
  * Thẻ công việc chuẩn `AGENTS.md` Rule 6:
    * Dòng trên: Checkbox tròn 24x24dp (touch target 44x44dp) + Tiêu đề công việc (`LineBreakMode="WordWrap"`).
    * Dòng dưới: Tag dự án (`SurfaceVariant` chip có icon Thư mục `Folder`) + Badge mốc giờ (`🕒 {DueTime}`).
    * Cột phải: Chevron `>` màu `TextMuted`.
  * `SwipeView`:
    * Vuốt phải: Hoàn thành nhanh (Xanh `Success`).
    * Vuốt trái: Xóa nhanh (Đỏ `Danger`).
* **7. Trạng thái Trống (Empty State)**:
  * Hiển thị khi `IsEmpty == true`. Icon `CalendarEmpty` (56dp), Tiêu đề *"Không có công việc"*, Chú thích *"Ngày này chưa có công việc nào được lên lịch"*, nút *"Lên lịch công việc"* (`PrimaryVariant` button).
* **8. Trạng thái Đang tải (Skeleton Shimmer Cards)**:
  * Hiển thị khi `IsBusy == true`. 3 Skeleton Cards mô phỏng cấu trúc Task Card.
* **9. Nút Nổi FAB (`+`)**:
  * Vị trí: Góc dưới bên phải (`TranslationX="-20"`, `TranslationY="-100"`), kích thước `56x56dp`, bo tròn `RadiusFull`, nền `Primary`, icon `+` trắng.
* **10. Modal Tạo việc nhanh & Floating Snackbar**:
  * Đồng bộ cơ chế như `TodayPage`.

---

## 5. Chiến Lược Kiểm Thử (Testing Strategy)

1. **Unit Tests (`WeekViewModelTests.cs`)**:
   * Kiểm tra tính toán chính xác 7 ngày trong tuần từ Thứ Hai đến Chủ Nhật (chuẩn ISO 8601).
   * Kiểm tra số lượng công việc `• n` tính toán đúng cho từng ngày.
   * Kiểm tra lệnh chuyển tuần `NextWeekCommand` / `PreviousWeekCommand` giữ nguyên thứ tương ứng.
   * Kiểm tra chọn ngày `SelectDayCommand` cập nhật danh sách việc và tính đúng tỷ lệ hoàn thành.
   * Kiểm tra `ToggleTaskStatusCommand` cập nhật trạng thái việc in-place và cập nhật metrics ngày.
   * Kiểm tra phản hồi sự kiện `TasksChanged`.
2. **UI & Responsive Check**:
   * Test trên giả lập kích thước 320dp, 360dp, 412dp và Dark Mode.
   * Đảm bảo dải 7 ngày hiển thị đều đặn, không bị cắt bớt hoặc tràn chữ.
