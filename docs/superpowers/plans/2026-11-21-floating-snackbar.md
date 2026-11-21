# Floating SnackBar & Undo System Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Xây dựng hoàn chỉnh Hệ thống Custom Floating SnackBar đa năng & Hoàn tác (Undo) theo Clean Architecture, MVVM thuần và chuẩn UI design `snack_bar.png` (Giai đoạn 7 trong `ROADMAP.md`).

**Architecture:** 
- Xây dựng dịch vụ tập trung `ISnackbarService` (Singleton) quản lý trạng thái, phát event điều phối, hỗ trợ 4 kiểu thông báo (`Success`, `Danger`, `Warning`, `Undo`) và callback `onUndo`.
- Xây dựng Custom Reusable View `FloatingSnackbarView` (Controls) tự động lắng nghe `ISnackbarService`, có hiệu ứng trượt & mờ dần (Slide & Fade: 250ms cubic-out / 200ms cubic-in), bộ đếm tự ẩn 5000ms, hỗ trợ bấm `✕` hoặc vuốt xuống để đóng sớm.
- Tích hợp `FloatingSnackbarView` lên các màn hình (`TodayPage`, `WeekPage`, `HistoryPage`, `SettingsPage`, `TaskDetailPage`), tái cấu trúc các ViewModels để gọi `ISnackbarService` thống nhất.
- Bổ sung nhóm nút thử nghiệm 4 loại SnackBar vào Reviewer Tools (`SettingsPage`).

**Tech Stack:** .NET 10 MAUI, C# 13, FluentIcons.Maui, xUnit, FluentAssertions.

**Spec:** `BUSINESS.md` (mục 3.6), `DESIGN.md` (mục 3.6, Token `SnackbarBg`), `ui_design/snack_bar.png`, `ROADMAP.md` (mục 1.3 & 7.1).

## Global Constraints

- Tuân thủ MVVM + Clean Architecture thuần C#, không thêm package ngoài NuGet.
- Vùng chạm tối thiểu 44x44dp cho các nút tương tác (`Hoàn tác`, nút đóng `✕`).
- Bảng màu Token-driven: Nền `#1E293B` (`SnackbarBg`), bo góc 12dp (`RadiusMd`), padding `16,12`, margin `16,0,16,24` nổi phía trên TabBar.
- Chế độ hiển thị: Thay thế tức thì (Immediate Replacement) khi có thông báo mới và khởi động lại timer 5000ms.
- Phản hồi Hoàn tác: Phục hồi công việc qua `UndoDeleteAsync()`, đóng thanh Undo và kích hoạt tiếp SnackBar Thành công (`Đã hoàn tác: [Tên công việc]`).
- Hỗ trợ song ngữ VI/EN qua `LocalizationService`.
- Không tự ý commit hoặc push code lên repository.

## Review Focus

1. Hiển thị đúng 4 kiểu icon và màu sắc theo `snack_bar.png`:
   - `Success`: Icon vòng tròn xanh check trắng (`CheckmarkCircle`, `Success`).
   - `Danger`: Icon vòng tròn đỏ chấm than (`ErrorCircle`, `Danger`).
   - `Warning`: Icon tam giác vàng (`Warning`, `Warning`).
   - `Undo`: Icon thùng rác (`Delete`, trắng) + Nút "Hoàn tác" (`PrimaryVariantDark` / tím bo góc 8dp).
2. Tự động ẩn sau 5000ms; bấm nút `✕` hoặc vuốt xuống phải đóng ngay lập tức mà không crash hay rò rỉ bộ nhớ.
3. Thao tác xóa việc tại `TaskDetailPage`: Sau khi xóa và `GoToAsync("..")`, SnackBar Undo phải hiển thị nổi trên màn hình trước đó (`TodayPage`, `WeekPage`, hoặc `HistoryPage`).
4. Khi bấm "Hoàn tác": Công việc vừa xóa được khôi phục thành công trong database (`ITaskService.UndoDeleteAsync()`), danh sách cập nhật realtime qua `TasksChanged`, và xuất hiện SnackBar Success tiếp nối.
5. Kiểm thử nhanh trong Reviewer Tools (`SettingsPage`): 4 nút bấm kiểm thử kích hoạt chuẩn xác cả 4 loại SnackBar.

---

### Task 1: Khai báo Enums, Models & Giao diện ISnackbarService

**Files:**
- Create: `src/MyTasksApp/Core/Enums/SnackbarType.cs`
- Create: `src/MyTasksApp/Core/Models/SnackbarModel.cs`
- Create: `src/MyTasksApp/Services/Interfaces/ISnackbarService.cs`
- Create: `src/MyTasksApp/Services/SnackbarService.cs`
- Modify: `src/MyTasksApp/MauiProgram.cs`
- Modify: `src/MyTasksApp/Services/LocalizationService.cs`

**Interfaces:**
- `SnackbarType`: `Success`, `Danger`, `Warning`, `Undo`.
- `SnackbarModel`: `string Message`, `SnackbarType Type`, `Func<Task>? OnUndoAction`, `int DurationMs = 5000`.
- `ISnackbarService`:
  - `event EventHandler<SnackbarModel>? ShowRequested;`
  - `event EventHandler? DismissRequested;`
  - `void Show(string message, SnackbarType type = SnackbarType.Success, Func<Task>? onUndo = null, int durationMs = 5000);`
  - `void ShowSuccess(string message, int durationMs = 5000);`
  - `void ShowDanger(string message, int durationMs = 5000);`
  - `void ShowWarning(string message, int durationMs = 5000);`
  - `void ShowUndo(string message, Func<Task> onUndo, int durationMs = 5000);`
  - `void Dismiss();`

- [x] **Step 1: Khai báo enum `SnackbarType`**
- [x] **Step 2: Khai báo class `SnackbarModel`**
- [x] **Step 3: Khai báo interface `ISnackbarService`**
- [x] **Step 4: Hiện thực `SnackbarService` với xử lý thread-safe trên `MainThread`**
- [x] **Step 5: Đăng ký `ISnackbarService` vào DI container (`MauiProgram.cs`)**
- [x] **Step 6: Bổ sung từ điển song ngữ VI/EN cho các thông báo SnackBar trong `LocalizationService.cs`**

---

### Task 2: Viết Unit Tests cho SnackbarService & Luồng Undo

**Files:**
- Create: `tests/MyTasksApp.Tests/SnackbarServiceTests.cs`

- [x] **Step 1: Viết test cho `ShowSuccess`, `ShowDanger`, `ShowWarning` kích hoạt event `ShowRequested` đúng kiểu và message**
- [x] **Step 2: Viết test cho `ShowUndo` truyền đúng callback `OnUndoAction`**
- [x] **Step 3: Viết test cho `Dismiss` kích hoạt `DismissRequested`**
- [x] **Step 4: Chạy `dotnet test` xác nhận test pass**

---

### Task 3: Xây dựng Reusable Control `FloatingSnackbarView` (XAML & Code-behind)

**Files:**
- Create: `src/MyTasksApp/Controls/FloatingSnackbarView.xaml`
- Create: `src/MyTasksApp/Controls/FloatingSnackbarView.xaml.cs`

**UI Specs:**
- Bố cục `Grid ColumnDefinitions="Auto, *, Auto, Auto"` bọc trong `Border` bo góc 12dp (`StrokeShape="RoundRectangle 12"`), nền `SnackbarBg` (`#1E293B`).
- Icon cột 0: Đổi linh hoạt theo 4 loại:
  - `Success`: `SymbolIcon Symbol="CheckmarkCircle"` màu `Success`.
  - `Danger`: `SymbolIcon Symbol="ErrorCircle"` màu `Danger`.
  - `Warning`: `SymbolIcon Symbol="Warning"` màu `Warning`.
  - `Undo`: `SymbolIcon Symbol="Delete"` màu White.
- Nhãn cột 1: `Label` hiển thị nội dung message (White, `FontSizeBody`, `InterMedium`).
- Nút Hoàn tác cột 2: `Button` ("Hoàn tác", nền `PrimaryVariantDark` / tím, bo góc 8dp, cao 32dp), chỉ hiển thị khi `Type == SnackbarType.Undo`.
- Nút đóng cột 3: `SymbolIcon Symbol="Dismiss"` màu `TextMuted`, vùng chạm hỗ trợ 44x44dp.
- Animation:
  - Hiện: `TranslationY = 30`, `Opacity = 0` -> trượt lên `TranslationY = 0`, `FadeToAsync(1, 250, CubicOut)`.
  - Ẩn: Trượt xuống `TranslationY = 20`, `FadeToAsync(0, 200, CubicIn)` -> `IsVisible = false`.
- Thao tác gạt tắt: Bổ sung `SwipeGestureRecognizer` (Direction: Down) hoặc Drag.

- [x] **Step 1: Tạo file XAML `FloatingSnackbarView.xaml`**
- [x] **Step 2: Viết code-behind `FloatingSnackbarView.xaml.cs` kết nối với `ISnackbarService` qua DI / ServiceProvider**
- [x] **Step 3: Triển khai hiệu ứng Animation mượt mà và bộ đếm CancellationTokenSource**
- [x] **Step 4: Xử lý sự kiện bấm Hoàn tác: gọi `OnUndoAction`, ẩn thanh và kích hoạt thông báo thành công tiếp nối**

---

### Task 4: Tích hợp FloatingSnackbarView & Tái cấu trúc ViewModels

**Files:**
- Modify: `src/MyTasksApp/Views/TodayPage.xaml`
- Modify: `src/MyTasksApp/ViewModels/TodayViewModel.cs`
- Modify: `src/MyTasksApp/Views/WeekPage.xaml`
- Modify: `src/MyTasksApp/ViewModels/WeekViewModel.cs`
- Modify: `src/MyTasksApp/Views/HistoryPage.xaml`
- Modify: `src/MyTasksApp/Views/TaskDetailPage.xaml`
- Modify: `src/MyTasksApp/ViewModels/TaskDetailViewModel.cs`
- Modify: `src/MyTasksApp/Views/SettingsPage.xaml`
- Modify: `src/MyTasksApp/ViewModels/SettingsViewModel.cs`

- [x] **Step 1: Cập nhật `TodayPage.xaml` & `TodayViewModel.cs`**:
  - Thay thế khối Border SnackBar cũ bằng `<controls:FloatingSnackbarView />`.
  - Inject `ISnackbarService` vào `TodayViewModel`, thay thế các biến `_isSnackbarVisible` bằng `_snackbarService.Show...`.
- [x] **Step 2: Cập nhật `WeekPage.xaml` & `WeekViewModel.cs`**:
  - Tương tự, tích hợp `<controls:FloatingSnackbarView />` và inject `ISnackbarService`.
- [x] **Step 3: Cập nhật `TaskDetailPage.xaml` & `TaskDetailViewModel.cs`**:
  - Nhúng `<controls:FloatingSnackbarView />`.
  - Khi xóa việc: gọi `_snackbarService.ShowUndo(...)` trước khi `GoToAsync("..")` để màn hình trước hiển thị ngay lập tức thanh Hoàn tác.
- [x] **Step 4: Cập nhật `HistoryPage.xaml`**:
  - Nhúng `<controls:FloatingSnackbarView />`.
- [x] **Step 5: Cập nhật `SettingsPage.xaml` & `SettingsViewModel.cs`**:
  - Nhúng `<controls:FloatingSnackbarView />`.
  - Thay thế logic snackbar cục bộ của SettingsViewModel bằng `_snackbarService.ShowSuccess(...)`.
  - Thêm 4 nút thử nghiệm SnackBar vào khu vực Reviewer Tools: "Thử Thành công", "Thử Lỗi", "Thử Cảnh báo", "Thử Hoàn tác".

---

### Task 5: Kiểm thử Toàn diện & Cập nhật ROADMAP

**Files:**
- Modify: `ROADMAP.md`

- [x] **Step 1: Chạy toàn bộ Unit Tests (`dotnet test`) xác nhận 100% tests pass**
- [x] **Step 2: Build ứng dụng `dotnet build -f net10.0-android` kiểm tra 0 Warning, 0 Error**
- [x] **Step 3: Cập nhật `ROADMAP.md` đánh dấu hoàn thành mục 1.3 (`SnackbarService`) và mục 7.1 (`Hệ Thống Custom Floating SnackBar & Hoàn Tác`)**
