# Week Screen Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Xây dựng hoàn chỉnh màn hình Tuần này (`WeekPage`) và `WeekViewModel` theo mô hình Clean Architecture & MVVM thuần, đáp ứng 100% thiết kế tại `ui_design/02.week_screen.png`, các quy tắc trong `BUSINESS.md`, `DESIGN.md` và `AGENTS.md`.

**Architecture:** MVVM thuần C# kết hợp Clean Architecture. ViewModel nhận `ITaskService` (Singleton), quản lý dải 7 ngày của tuần (`WeekDays`), ngày đang chọn (`SelectedDate`), và danh sách công việc của ngày đó sắp xếp theo mốc giờ tăng dần. Dải 7 ngày dùng `Grid` 7 cột bằng nhau chống tràn; thẻ công việc 2 tầng responsive; hỗ trợ hoàn thành tại chỗ (in-place), SwipeView xóa/hoàn tác, FAB tạo việc nhanh và đồng bộ thời gian thực qua sự kiện `TasksChanged`.

**Tech Stack:** .NET MAUI 10, C# 13, FluentIcons.Maui, xUnit, FluentAssertions.

**Spec:** [2026-11-20-week-screen-design.md](file:///c:/Workplace/GitHub/fizibox-my-tasks-maui/docs/superpowers/specs/2026-11-20-week-screen-design.md)

## Global Constraints

- Không tự ý commit hoặc push code lên repository (tuân thủ tuyệt đối quy tắc `AGENTS.md`).
- Không cài đặt thêm bất kỳ NuGet package nào ngoài `FluentIcons.Maui` đã có.
- Tuân thủ quy chuẩn thiết kế Responsive: Bố cục Task Card 2 tầng, chống tràn nội dung (Anti-overflow), vùng chạm tương tác tối thiểu 44x44dp.
- Bắt đầu tuần từ Thứ Hai và kết thúc vào Chủ Nhật (chuẩn ISO 8601 - Điểm tự quyết số 2).
- Giữ nguyên ngày đã chọn khi người dùng chuyển tab và quay lại màn hình Tuần này (Điểm tự quyết số 3).
- 100% Design Tokens qua `{DynamicResource}` cho màu sắc và cỡ chữ, không hardcode mã hex.

## Review Focus

1. **Tuần vắt qua tháng/năm:** Tuần bắt đầu ở tháng này và kết thúc ở tháng sau (ví dụ: 30/11 – 06/12), nhãn tuần và các ngày hiển thị chuẩn xác không bị lỗi hiển thị.
2. **Bảo toàn ngày chọn khi đổi tuần:** Khi bấm `<` hoặc `>`, ngày chọn tự động dịch chuyển đúng thứ tương ứng (ví dụ: Thứ Sáu 20/11 -> Thứ Sáu 27/11).
3. **Giữ nguyên vị trí việc khi hoàn thành:** Đánh dấu hoàn thành việc không làm nhảy vị trí của thẻ việc trên timeline của ngày, chỉ gạch ngang chữ và hiện tích xanh.
4. **Tạo việc ngày khác ngày đang xem:** Khi tạo việc từ FAB chọn ngày khác ngày đang xem, danh sách ngày hiện tại không bị lẫn việc, badge số lượng của ngày mới tạo được cập nhật tức thì.
5. **Kích thước siêu hẹp (320dp - 360dp):** 7 cột trên Week Strip tự động co giãn đều đặn, không bị cắt cụt số ngày hay chữ thứ.

---

### Task 1: Tạo Model `WeekDayItem`

**Files:**
- Create: `src/MyTasksApp/Core/Models/WeekDayItem.cs`
- Test: `tests/MyTasksApp.Tests/WeekDayItemTests.cs`

**Interfaces:**
- Produces: `WeekDayItem : ObservableObject` với các thuộc tính:
  - `Date`: `DateTime`
  - `DayOfWeekShort`: `string` ("T2", "T3", "T4", "T5", "T6", "T7", "CN")
  - `DayNumber`: `string` ("16", "17", ...)
  - `TaskCount`: `int`
  - `TaskCountText`: `string` (`• {TaskCount}` hoặc `• 0`)
  - `IsToday`: `bool`
  - `IsSelected`: `bool`

- [ ] **Step 1: Viết failing test cho `WeekDayItem`**
  Viết `tests/MyTasksApp.Tests/WeekDayItemTests.cs` kiểm tra: khởi tạo thuộc tính, tính toán `TaskCountText`, và phát sinh sự kiện `PropertyChanged` khi thay đổi `TaskCount`, `IsSelected`, `IsToday`.

- [ ] **Step 2: Chạy test để xác nhận test FAIL**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekDayItemTests`
  Kỳ vọng: FAIL vì chưa có file `WeekDayItem.cs`.

- [ ] **Step 3: Cài đặt `WeekDayItem.cs`**
  Tạo `src/MyTasksApp/Core/Models/WeekDayItem.cs` kế thừa `ObservableObject`.

- [ ] **Step 4: Chạy lại test để xác nhận PASS**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekDayItemTests`
  Kỳ vọng: PASS.

---

### Task 2: Cài đặt `WeekViewModel` Core (TDD: Tính dải tuần, Điều hướng & Lọc ngày)

**Files:**
- Create: `src/MyTasksApp/ViewModels/WeekViewModel.cs`
- Create: `tests/MyTasksApp.Tests/WeekViewModelTests.cs`

**Interfaces:**
- Consumes: `ITaskService`, `WeekDayItem`, `TaskItem`, `BaseViewModel`
- Produces: `WeekViewModel` với:
  - `WeekDays`: `ObservableCollection<WeekDayItem>` (7 phần tử)
  - `CurrentWeekStart`: `DateTime`
  - `SelectedDate`: `DateTime`
  - `DayTasks`: `ObservableCollection<TaskItem>`
  - `WeekRangeText`: `string`
  - `SelectedDayTitle`: `string`
  - `DayProgressRatio`: `double`
  - `DayProgressPercentageText`: `string`
  - `DayProgressRatioText`: `string`
  - `LoadWeekDataCommand`: `AsyncRelayCommand`
  - `PreviousWeekCommand`: `AsyncRelayCommand`
  - `NextWeekCommand`: `AsyncRelayCommand`
  - `SelectDayCommand`: `AsyncRelayCommand`

- [ ] **Step 1: Viết failing tests cho `WeekViewModel` Core**
  Trong `tests/MyTasksApp.Tests/WeekViewModelTests.cs`:
  - `LoadWeekData_ShouldGenerate7DaysFromMondayToSunday()`
  - `PreviousWeek_And_NextWeek_ShouldShift7DaysAndPreserveDayOfWeek()`
  - `SelectDay_ShouldUpdateSelectedDateAndFilterDayTasksInChronologicalOrder()`
  - `DayProgress_ShouldCalculateAccurateMetrics()`

- [ ] **Step 2: Chạy test để xác nhận test FAIL**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekViewModelTests`
  Kỳ vọng: FAIL vì chưa tạo `WeekViewModel.cs`.

- [ ] **Step 3: Cài đặt `WeekViewModel.cs` Core**
  Xây dựng class `WeekViewModel` kế thừa `BaseViewModel`:
  - Thuật toán xác định Thứ Hai đầu tuần (ISO 8601).
  - Khởi tạo 7 ngày `WeekDays` với tên thứ tiếng Việt chuẩn (`T2`, `T3`, `T4`, `T5`, `T6`, `T7`, `CN`).
  - Lấy danh sách việc cho cả 7 ngày qua `ITaskService.GetTasksByDateAsync` để gán `TaskCount` cho từng ngày.
  - Lọc việc của `SelectedDate`, sắp xếp theo `DueTime` tăng dần.
  - Tính toán `DayProgressRatio`, `DayProgressPercentageText`, `DayProgressRatioText`.

- [ ] **Step 4: Chạy lại test để xác nhận PASS**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekViewModelTests`
  Kỳ vọng: PASS.

---

### Task 3: Thao tác Công việc & Đồng bộ Realtime trong `WeekViewModel`

**Files:**
- Modify: `src/MyTasksApp/ViewModels/WeekViewModel.cs`
- Modify: `tests/MyTasksApp.Tests/WeekViewModelTests.cs`

**Interfaces:**
- Produces:
  - `ToggleTaskStatusCommand`: `AsyncRelayCommand` (hoàn thành / mở lại tại chỗ, cập nhật tiến độ ngay).
  - `DeleteTaskCommand`: `AsyncRelayCommand` (xóa việc khỏi ngày hiện tại, kích hoạt Undo Snackbar).
  - `UndoDeleteCommand`: `AsyncRelayCommand` (phục hồi việc vừa xóa).
  - `OpenCreateModalCommand` / `CloseCreateModalCommand` / `SaveNewTaskCommand`: Tạo việc với ngày mặc định `SelectedDate`.
  - Handler sự kiện `_taskService.TasksChanged`: Cập nhật lại số lượng `• n` 7 ngày và danh sách việc của `SelectedDate`.

- [ ] **Step 1: Viết failing tests cho Task Actions & Realtime Sync**
  Thêm tests vào `WeekViewModelTests.cs`:
  - `ToggleTaskStatus_ShouldKeepTaskInPlaceAndToggleStatus()`
  - `DeleteTask_ShouldRemoveFromDayTasksAndUpdateProgress()`
  - `SaveNewTask_WithDifferentDate_ShouldKeepCurrentDayAndIncrementTargetDayBadge()`
  - `OnTasksChanged_ShouldRefreshWeekDaysAndSelectedDayTasks()`

- [ ] **Step 2: Chạy test để xác nhận test FAIL**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekViewModelTests`
  Kỳ vọng: FAIL cho các test mới.

- [ ] **Step 3: Cài đặt các Commands và Event Handler trong `WeekViewModel.cs`**
  - Cài đặt `ExecuteToggleTaskStatusAsync`: Đổi trạng thái việc `Pending` <-> `Completed`, cập nhật tiến độ ngay lập tức, giữ nguyên vị trí trong `DayTasks`, đồng bộ xuống service.
  - Cài đặt `ExecuteDeleteTaskAsync` & `ExecuteUndoDeleteAsync`.
  - Cài đặt Modal tạo việc nhanh và `ExecuteSaveNewTaskAsync`.
  - Đăng ký lắng nghe sự kiện `_taskService.TasksChanged`.

- [ ] **Step 4: Chạy lại test để xác nhận PASS**
  Chạy `dotnet test tests/MyTasksApp.Tests --filter FullyQualifiedName~WeekViewModelTests`
  Kỳ vọng: Tất cả tests đều PASS.

---

### Task 4: Đăng ký DI & Kết nối `WeekPage.xaml.cs`

**Files:**
- Modify: `src/MyTasksApp/MauiProgram.cs`
- Modify: `src/MyTasksApp/Views/WeekPage.xaml.cs`

**Interfaces:**
- Consumes: `WeekViewModel`
- Produces: `WeekPage` nhận `WeekViewModel` qua constructor injection, tự động nạp dữ liệu khi hiển thị lần đầu qua `OnAppearing`.

- [ ] **Step 1: Đăng ký `WeekViewModel` vào DI container**
  Trong `src/MyTasksApp/MauiProgram.cs`, thêm `builder.Services.AddSingleton<WeekViewModel>();`.

- [ ] **Step 2: Cập nhật `WeekPage.xaml.cs`**
  - Inject `WeekViewModel` vào constructor của `WeekPage`.
  - Gán `BindingContext = viewModel;`.
  - Trong `OnAppearing`, gọi `_ = viewModel.EnsureDataLoadedAsync();`.

- [ ] **Step 3: Biên dịch kiểm tra lỗi DI**
  Chạy `dotnet build -f net10.0-windows10.0.19041.0` (hoặc build target có sẵn) để đảm bảo không lỗi cú pháp và DI resolving chuẩn.

---

### Task 5: Xây dựng Giao diện XAML Hoàn Chỉnh (`WeekPage.xaml`)

**Files:**
- Modify: `src/MyTasksApp/Views/WeekPage.xaml`

**Interfaces:**
- Binds to: `WeekViewModel`
- Components:
  1. `RefreshView` pull-to-refresh.
  2. Top Brand Bar: Logo Squircle viền mỏng + Tên app "MyTasks / Việc của tôi" + Chip dải tuần bên phải.
  3. Tiêu đề lớn "Tuần này" (`FontSizeDisplay`, `InterBold`).
  4. Week Navigator: Nút `<`, dải ngày `16 – 22 Tháng 11, 2026`, nút `>`.
  5. Week Strip: 7-Column `Grid` (`ColumnDefinitions="*,*,*,*,*,*,*"`), mỗi cột là 1 ngày hiển thị Thứ, Ngày, `• n`, highlight rõ ràng (hôm nay viền 2dp, đang chọn nền đặc `Primary`).
  6. Day Summary Card: Thẻ tiến độ ngày bo góc 16dp, nền `PrimaryLight`, thanh `ProgressBar` và phần trăm `%`.
  7. Danh sách công việc 2 tầng: Dòng 1 Checkbox + Tiêu đề; Dòng 2 Tag dự án + Badge giờ; Cột phải Chevron; `SwipeView` vuốt hoàn thành/xóa.
  8. Empty State: Icon `CalendarEmpty`, thông điệp và nút CTA "Lên lịch công việc".
  9. Shimmer Skeleton Loading: 3 card khung xương khi `IsBusy == true`.
  10. FAB (+) button nổi góc dưới phải.
  11. Modal tạo việc nhanh (Bottom sheet overlay) & Floating Snackbar thông báo.

- [ ] **Step 1: Cập nhật XAML cho WeekPage**
  Cập nhật file `src/MyTasksApp/Views/WeekPage.xaml` bám sát 100% Design Tokens trong `DESIGN.md` và bố cục thiết kế.

- [ ] **Step 2: Biên dịch XAML SourceGen**
  Chạy `dotnet build -f net10.0-windows10.0.19041.0` để xác nhận XAML hợp lệ, zero warning/error từ XAML Source Generator.

---

### Task 6: Kiểm Thử Toàn Diện, Responsive & Cập Nhật ROADMAP.md

**Files:**
- Modify: `ROADMAP.md`

- [ ] **Step 1: Chạy toàn bộ Unit Tests**
  Chạy `dotnet test` để đảm bảo 100% tests (Service, TodayViewModel, WeekViewModel) đều PASS.

- [ ] **Step 2: Kiểm tra biên dịch Android**
  Chạy `dotnet build -f net10.0-android` để đảm bảo tương thích 100% trên nền tảng Android.

- [ ] **Step 3: Cập nhật `ROADMAP.md`**
  Đánh dấu hoàn thành `[x]` cho toàn bộ mục Giai đoạn 4:
  - 4.1. WeekViewModel: Tính toán 7 ngày, tính số lượng badge `• n`, quản lý ngày đang chọn, lọc việc theo ngày và tính tỷ lệ hoàn thành.
  - 4.2. WeekPage UI (XAML): Thanh điều hướng tuần, Week Strip 7 ngày, Day Summary Card, Danh sách việc, FAB (+), Shimmer Loading và Empty State.
