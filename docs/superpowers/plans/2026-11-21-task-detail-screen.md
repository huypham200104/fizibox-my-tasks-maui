# Task Detail Screen Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Xây dựng hoàn chỉnh màn hình Chi tiết công việc (`TaskDetailPage`), `TaskDetailViewModel`, và hệ thống Bottom Sheets/Modals tương tác tùy biến theo Clean Architecture, MVVM thuần và chuẩn UI design.

**Architecture:** `TaskDetailPage` liên kết với `TaskDetailViewModel` nhận `id` qua query parameter, giao tiếp với `ITaskService` (`GetTaskByIdAsync`, `UpdateTaskAsync`, `SetTaskStatusAsync`, `DeleteTaskAsync`). Toàn bộ hệ thống Bottom Sheets / Modals (Chọn trạng thái, Lịch ngày, Đồng hồ giờ, Danh sách dự án, Form sửa, Xác nhận mở lại, Xác nhận xóa, Not Found State) được xây dựng thuần MAUI theo tokens từ `DESIGN.md`.

**Tech Stack:** .NET 10 MAUI, C# 13, FluentIcons.Maui, xUnit, FluentAssertions.

**Spec:** `docs/superpowers/specs/2026-11-21-task-detail-screen-design.md`

## Global Constraints

- Kiến trúc Clean Architecture + MVVM thuần C# (không cài thêm thư viện MVVM hay popup 3rd-party ngoài `FluentIcons.Maui`).
- Sử dụng DynamicResource cho màu sắc và kích thước font (`DESIGN.md`).
- Vùng chạm tối thiểu 44x44dp cho các nút bấm tương tác.
- Chống tràn nội dung (Anti-overflow), tương thích responsive từ 320dp đến 412dp+.
- Hỗ trợ song ngữ VI/EN qua `LocalizationService`.
- Không tự ý commit hoặc push code lên repository.

## Review Focus

1. Khi mở với ID không hợp lệ hoặc `id=-1`: Phải hiển thị giao diện Not Found thân thiện kèm nút quay về thay vì throw exception hoặc trắng màn hình.
2. Thao tác Mở lại công việc (Reopen) từ trạng thái Đã hoàn thành: Phải hiển thị hộp thoại xác nhận bắt buộc trước khi chuyển trạng thái về Chưa hoàn thành.
3. Khi chỉnh sửa công việc (Modal Edit): Bộ đếm ký tự phải cập nhật realtime (Tên 100, Mô tả 1000, Ghi chú 500) và không bị mất dữ liệu khi Hủy.
4. Chọn ngày và giờ: Lưới lịch tháng phải điều hướng được tháng trước/tháng sau, tính đúng dải 42 ô ngày (chuẩn T2–CN), chọn nhanh chip giờ (09:00, 12:00, 14:00, 17:00) phản ánh tức thì lên số giờ hiển thị.
5. Xóa công việc: Phải hiển thị hộp thoại xác nhận, sau đó gọi `ITaskService.DeleteTaskAsync`, điều hướng quay về trang trước và đồng bộ trạng thái qua `TasksChanged`.

---

### Task 1: Bổ sung Localization Strings & Model CalendarDayItem

**Files:**
- Create: `src/MyTasksApp/Core/Models/CalendarDayItem.cs`
- Modify: `src/MyTasksApp/Services/LocalizationService.cs`

**Interfaces:**
- `CalendarDayItem`: `DateTime Date`, `int DayNumber`, `bool IsCurrentMonth`, `bool IsToday`, `bool IsSelected`.
- `LocalizationService`: Bổ sung từ điển song ngữ VI/EN cho trang Chi tiết công việc và các Modals.

- [x] **Step 1: Khai báo entity `CalendarDayItem` kế thừa `ObservableObject`**
- [x] **Step 2: Thêm các khóa bản dịch vào từ điển tiếng Việt (`_vi`) và tiếng Anh (`_en`) trong `LocalizationService.cs`**
- [x] **Step 3: Biên dịch kiểm tra cú pháp**

---

### Task 2: Xây dựng TaskDetailViewModel & Bộ Unit Tests

**Files:**
- Create: `src/MyTasksApp/ViewModels/TaskDetailViewModel.cs`
- Create: `tests/MyTasksApp.Tests/TaskDetailViewModelTests.cs`
- Modify: `src/MyTasksApp/MauiProgram.cs`

**Interfaces:**
- Consumes: `ITaskService`, `ILocalizationService`, `TaskItem`, `CalendarDayItem`.
- Produces: `TaskDetailViewModel` kế thừa `BaseViewModel` và thực thi `IQueryAttributable`.

- [x] **Step 1: Viết bộ unit tests cho `TaskDetailViewModelTests`**
  - Test nạp việc thành công theo ID hợp lệ.
  - Test nạp việc với ID không hợp lệ -> `IsNotFound == true`.
  - Test đổi trạng thái Hoàn thành -> `Status == Completed` và có `CompletedAt`.
  - Test Mở lại công việc -> Kích hoạt Dialog xác nhận, xác nhận xong -> `Status == Pending`.
  - Test mở modal Chỉnh sửa và lưu thay đổi thành công.
  - Test chọn ngày trên lưới lịch và chọn giờ.
  - Test xóa công việc -> gọi `DeleteTaskAsync`.
- [x] **Step 2: Triển khai `TaskDetailViewModel.cs`**
  - Quản lý `CurrentTask`, `IsNotFound`, `IsBusy`.
  - Triển khai logic tạo lưới lịch 42 ngày cho Date Picker Sheet.
  - Triển khai logic chọn giờ và chip giờ nhanh cho Time Picker Sheet.
  - Triển khai logic form modal chỉnh sửa với bộ đếm ký tự.
  - Xử lý các command xác nhận mở lại, xác nhận xóa.
- [x] **Step 3: Đăng ký `TaskDetailViewModel` vào DI Container (`MauiProgram.cs`)**
- [x] **Step 4: Chạy `dotnet test` kiểm tra toàn bộ test pass**

---

### Task 3: Xây dựng Giao Diện TaskDetailPage (XAML & Code-behind)

**Files:**
- Modify: `src/MyTasksApp/Views/TaskDetailPage.xaml`
- Modify: `src/MyTasksApp/Views/TaskDetailPage.xaml.cs`

**Interfaces:**
- Consumes: `TaskDetailViewModel`.
- Produces: Giao diện chi tiết công việc chuẩn pixel theo `ui_design/detail_task.png` cùng hệ thống Bottom Sheets/Modals.

- [x] **Step 1: Xây dựng Top Bar tùy biến (Back `<` + Tiêu đề + Menu `⋮`)**
- [x] **Step 2: Xây dựng nội dung chi tiết dạng thẻ (Header, Tag Dự án, Chip Trạng thái, Card Mô tả, Card Thông tin, Card Ghi chú)**
- [x] **Step 3: Xây dựng nút CTA cố định ở đáy (Hoàn thành / Mở lại)**
- [x] **Step 4: Xây dựng Custom Action Sheet Menu (Menu 3 chấm)**
- [x] **Step 5: Xây dựng Modal Chỉnh sửa công việc (`popup_edit_text.png`) có đếm ký tự**
- [x] **Step 6: Xây dựng Bottom Sheet Chọn trạng thái (`bottom_sheet_change_status.png`)**
- [x] **Step 7: Xây dựng Bottom Sheet Chọn ngày (`bottom_sheet_date_picker.png`)**
- [x] **Step 8: Xây dựng Bottom Sheet Chọn giờ (`bottom_sheet_time_picker.png`)**
- [x] **Step 9: Xây dựng Bottom Sheet Chọn dự án**
- [x] **Step 10: Xây dựng Confirmation Dialogs (Mở lại việc & Xóa việc)**
- [x] **Step 11: Xây dựng Not Found State View**
- [x] **Step 12: Cập nhật code-behind `TaskDetailPage.xaml.cs` nhận `TaskDetailViewModel` qua DI**

---

### Task 4: Kiểm thử, Tối ưu & Cập nhật Roadmap

**Files:**
- Modify: `ROADMAP.md`

- [x] **Step 1: Chạy `dotnet test` toàn diện cho toàn bộ solution**
- [x] **Step 2: Chạy `dotnet build` để đảm bảo 0 Warning, 0 Error**
- [x] **Step 3: Cập nhật `ROADMAP.md` đánh dấu hoàn thành Giai đoạn 6**
