# Tài Liệu Chuẩn Bị Phỏng Vấn Kỹ Thuật — UI "Việc Của Tôi"

> **Dành cho buổi review dự án với Fizibox (Reviewer: Thanh Nguyen)**  
> **Nguyên tắc xuyên suốt:** Tự tin, trung thực, phân biệt rạch ròi giữa *"Đã làm"*, *"Đã thử"* và *"Chưa kiểm chứng"*; làm nổi bật tư duy kỹ thuật và khả năng làm chủ công cụ AI.

---

## PHẦN 1: GIẢI TRÌNH 4 CÂU HỎI KIẾN TRÚC TRỌNG TÂM (ƯU TIÊN 3)

### 1. Cấu trúc XAML lớn (1.000+ dòng) & Kế hoạch Tách Component `TaskCard`
- **Câu hỏi của Reviewer:** *`TaskDetailPage.xaml` khoảng 1.391 dòng, `WeekPage.xaml` 1.268 dòng và `TodayPage.xaml` 1.027 dòng; template thẻ công việc đang lặp lại ở nhiều nơi. Em hãy giải thích cách tổ chức hiện tại và hướng tách thành `TaskCard` dùng chung?*
- **Câu trả lời chuẩn:**
  - **Lý do tổ chức hiện tại (Trade-off khi làm bài):**
    - Mỗi màn hình có hành vi thẻ công việc mang ngữ cảnh riêng biệt:
      - *Màn Hôm nay:* Phân 3 nhóm (Quá hạn, Hôm nay, Xong), SwipeItem là "Hoàn thành / Xóa" hoặc "Mở lại / Xóa", có hiệu ứng delay chuyển nhóm.
      - *Màn Tuần này:* Lọc theo ngày được chọn trong dải 7 ngày, cập nhật badge ngày động.
      - *Màn Lịch sử:* Nhóm theo ngày hoàn thành (`HistoryGroupItem`), SwipeItem chỉ có "Mở lại / Xóa".
    - Việc giữ DataTemplate inline giúp tối ưu hóa cơ chế compile XAML của .NET 10 (`MauiXamlInflator=SourceGen`), tận dụng kiểm tra kiểu biên dịch compile-time `x:DataType="models:TaskItem"` và tránh các lỗi binding context phức tạp khi lồng UserControl trong MAUI.
  - **Hướng tái cấu trúc thành `TaskCardView` dùng chung:**
    - Tạo `Controls/TaskCardView.xaml` kế thừa `ContentView`.
    - Sử dụng `BindableProperty` cho:
      - `TaskItem Task`: Đối tượng công việc chính.
      - `bool AllowToggleStatus`: Cho phép bấm checkbox hay chỉ xem.
      - `ICommand ToggleStatusCommand`, `ICommand DeleteCommand`, `ICommand TapCommand`.
      - `SwipeViewMode Mode`: Xác định các nút vuốt hiển thị theo ngữ cảnh (Complete/Delete hoặc Reopen/Delete).
    - Lợi ích: Giảm kích thước mỗi file XAML xuống dưới 400 dòng, tái sử dụng 100% bố cục 2 tầng chống tràn.

---

### 2. Sử dụng `Task.Delay(50/80/150)` trong Code-behind
- **Câu hỏi của Reviewer:** *Code có nhiều `Task.Delay(50/80/150)`. Em hãy giải thích từng trường hợp cần chờ điều gì, rủi ro trên thiết bị khác và có cách nào chắc chắn hơn không?*
- **Câu trả lời chuẩn:**
  - **3 trường hợp cụ thể trong code:**
    1. *Hiệu ứng thị giác khi bấm Checkbox (350ms):* Người dùng cần thấy rõ dấu tick đổi màu xanh và chữ bị gạch ngang trước khi thẻ công việc tự động biến mất và di chuyển xuống nhóm "Đã xong". Nếu không có delay, thẻ biến mất tức thì trong 0ms khiến người dùng giật mình tưởng bị lỗi mất dữ liệu.
    2. *Chờ đo đạc kích thước giao diện (Layout Measurement - 80ms):* Trên Android, sau khi thêm phần tử vào danh sách, cần đợi UI Layout Pass hoàn tất trước khi gọi `ScrollView.ScrollToAsync`.
    3. *Chống nhấp nháy Skeleton Shimmer (Anti-flicker - 150ms):* Khi đọc dữ liệu từ LiteDB quá nhanh (chỉ mất 2-3ms), màn hình Skeleton sẽ nhấp nháy cực nhanh gây mỏi mắt. Cần giữ Skeleton tối thiểu 150ms để mắt kịp ghi nhận.
  - **Rủi ro:** Trên các thiết bị cấu hình yếu hoặc khi máy bị nghẽn CPU, thời gian 80ms có thể chưa đủ để Layout Pass hoàn thành; ngược lại trên máy mạnh, thời gian chờ có thể tạo cảm giác trễ nhẹ.
  - **Giải pháp chắc chắn hơn:**
    - Thay vì delay thời gian cứng, sử dụng sự kiện vòng đời chuẩn: `View.Loaded`, `VisualElement.SizeChanged`.
    - Đối với hiệu ứng tick hoàn thành: Sử dụng Animation callback (`await taskCard.FadeTo(0, 250)`) rồi mới xóa phần tử khỏi collection; Animation kết thúc chính là tín hiệu chắc chắn 100% để tiếp tục luồng xử lý.

---

### 3. Khả năng Tiếp Cận (Accessibility) & Tắt `FontAutoScalingEnabled`
- **Câu hỏi của Reviewer:** *Ứng dụng tắt `FontAutoScalingEnabled` toàn bộ, nên không đi theo cỡ chữ hệ thống. Nếu đây là lựa chọn có chủ đích, em ghi rõ lý do và đánh đổi?*
- **Câu trả lời chuẩn:**
  - **Lý do có chủ đích:**
    - Ứng dụng công việc có nhiều bố cục đòi hỏi tính chính xác cao: Thanh TabBar 4 tab, dải lịch 7 ngày trong tuần, huy hiệu (Badge) giờ hẹn và nút Checkbox tròn.
    - Khi người dùng bật tính năng Text Scale của hệ điều hành lên mức cực đại (150% - 200%), font hệ thống sẽ phá vỡ layout, làm tràn chữ ra ngoài màn hình hoặc đẩy thanh TabBar xuống dưới đáy.
  - **Giải pháp thay thế chủ động:**
    - Thay vì để hệ thống tự động phóng to không kiểm soát, ứng dụng đã cung cấp sẵn **Tính năng Cỡ chữ (Small / Normal / Large)** trong màn hình Cài đặt.
    - Khi người dùng chọn cỡ chữ Lớn (Large), ứng dụng tự động tăng tỷ lệ typography thông qua `DynamicResource FontSizeBody`, đồng thời bố cục 2 tầng của Task Card đảm bảo tiêu đề vẫn hiển thị nguyên vẹn mà không bị vỡ khung.

---

### 4. Phạm vi Làm Thêm (Lịch sử, Tạo/Sửa/Xóa, Chọn dự án)
- **Câu hỏi của Reviewer:** *Các phần Lịch sử, CRUD và chọn dự án không được cộng điểm chỉ vì vượt đề. Em rút ra bài học gì về phạm vi sản phẩm?*
- **Câu trả lời chuẩn:**
  - **Nhìn nhận trung thực:** Ban đầu em muốn xây dựng một ứng dụng trọn vẹn, có thể dùng thật hàng ngày nên đã mở rộng thêm các tính năng này.
  - **Bài học kỹ thuật sâu sắc:**
    - Trong phát triển phần mềm doanh nghiệp, **"Scope Creep"** (ôm đồm tính năng) luôn đi kèm chi phí bảo trì và rủi ro: Làm thêm màn hình đồng nghĩa với việc phải xử lý thêm bản dịch song ngữ, viết thêm unit test và kiểm soát thêm trạng thái bất đồng bộ.
    - Điểm quan trọng nhất mà một kỹ sư cần hướng tới là **Tính đúng đắn (Correctness)**, **Sự ổn định (Stability)** và **Độ hoàn thiện xuất sắc của phạm vi cốt lõi** trước khi nghĩ đến việc mở rộng chức năng.

---

## PHẦN 2: HƯỚNG DẪN THỰC HÀNH 3 BÀI TEST LIVE-CODING (MỤC 5)

Nếu trong buổi review, Reviewer yêu cầu bạn thử sửa code trực tiếp:

### Bài test 1: Đổi màu chủ đạo (Primary Brand Color) trong 30 giây
- **Yêu cầu:** Đổi màu chủ đạo ứng dụng từ tím Indigo (`#4F46E5`) sang xanh ngọc Teal (`#0D9488`).
- **Thao tác:**
  1. Mở file: `src/MyTasksApp/Core/DesignTokens/AppColors.cs`.
  2. Sửa 2 dòng đầu tiên:
     ```csharp
     public static readonly Color Primary = Color.FromArgb("#0D9488");     // Teal-600
     public static readonly Color PrimaryDark = Color.FromArgb("#14B8A6"); // Teal-500
     ```
  3. Mở file: `src/MyTasksApp/Resources/Styles/Colors.xaml` (cập nhật tương ứng cho static token ban đầu):
     ```xml
     <Color x:Key="Primary">#0D9488</Color>
     <Color x:Key="PrimaryDark">#14B8A6</Color>
     ```
  4. **Kết quả:** Toàn bộ thanh tiêu đề, thanh tiến độ, các nút bấm, màu highlight tab và các card chọn trong Cài đặt lập tức chuyển sang tông xanh Teal đồng bộ.

---

### Bài test 2: Thêm số công việc quá hạn lên Header màn hình Hôm nay
- **Yêu cầu:** Trên thanh tiêu đề màn hình Hôm nay, hiển thị thêm số lượng công việc đang bị quá hạn.
- **Thao tác:**
  1. Mở `src/MyTasksApp/ViewModels/TodayViewModel.cs`:
     Thêm property:
     ```csharp
     public string OverdueCountBadge => OverdueTasks.Count > 0 ? $"{OverdueTasks.Count} quá hạn" : string.Empty;
     public bool HasOverdueTasks => OverdueTasks.Count > 0;
     ```
     Trong hàm `UpdateProgressMetrics()`, thêm:
     ```csharp
     OnPropertyChanged(nameof(OverdueCountBadge));
     OnPropertyChanged(nameof(HasOverdueTasks));
     ```
  2. Mở `src/MyTasksApp/Views/TodayPage.xaml`:
     Trong khối Header `NormalHeaderTitleGroup`, thêm nhãn Badge:
     ```xml
     <Border IsVisible="{Binding HasOverdueTasks}"
             BackgroundColor="{DynamicResource DangerSurface}"
             StrokeShape="RoundRectangle 8" StrokeThickness="0" Padding="8,2">
         <Label Text="{Binding OverdueCountBadge}" TextColor="{DynamicResource Danger}" FontSize="11" FontFamily="InterSemiBold" />
     </Border>
     ```

---

### Bài test 3: Đổi ngày bắt đầu tuần (Chủ nhật thay vì Thứ hai)
- **Yêu cầu:** Chuyển đổi dải 7 ngày trong màn Tuần này bắt đầu từ Chủ Nhật (theo chuẩn Mỹ/US).
- **Thao tác:**
  1. Mở `src/MyTasksApp/ViewModels/WeekViewModel.cs`:
     Sửa hàm `GetMondayOfWeek`:
     ```csharp
     public static DateTime GetMondayOfWeek(DateTime dt)
     {
         int diff = (7 + (dt.DayOfWeek - DayOfWeek.Sunday)) % 7;
         return dt.AddDays(-1 * diff).Date;
     }
     ```
  2. Sửa hàm `GetDayNames`:
     ```csharp
     return _localizationService?.CurrentLanguage == AppLanguage.English
         ? new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }
         : new[] { "CN", "T2", "T3", "T4", "T5", "T6", "T7" };
     ```
  3. **Kết quả:** Dải 7 ngày bắt đầu từ Chủ Nhật đến Thứ Bảy, badge và danh sách công việc tự động cập nhật khớp với ngày mới.

---

## PHẦN 3: CÁCH TRÌNH BÀY VÀ LÀM VIỆC CÙNG AI TRONG BUỔI REVIEW

1. **Minh bạch về vai trò của AI:**
   - *"Em sử dụng AI như một công cụ hỗ trợ lập trình đắc lực (Co-pilot) để sinh mã boilerplate, tra cứu cú pháp .NET 10 MAUI và phác thảo các kịch bản Unit Test."*
   - *"Tuy nhiên, toàn bộ quyết định kiến trúc (Clean Architecture, MVVM), thẩm định logic nghiệp vụ và bằng chứng kiểm thử thực tế đều do em trực tiếp xem xét, phát hiện sai sót và điều chỉnh."*
2. **Ví dụ cụ thể khi AI sai và cách em xử lý:**
   - *Ví dụ 1 (Test Settings fail):* AI ban đầu dùng trực tiếp `PreferencesSettingsService` thật trong xUnit, dẫn đến lỗi Preferences không có context. Em đã yêu cầu tách interface và tạo `MockSettingsService` in-memory.
   - *Ví dụ 2 (Lệch ngày kiểm thử):* AI viết test so sánh trực tiếp với `DateTime.Today` của máy tính. Em đã phát hiện vấn đề khi test chạy vào ngày thứ Hai và quyết định đưa vào abstraction `IDateTimeProvider`.
3. **Phong thái trả lời:**
   - Ngắn gọn, đi thẳng vào nguyên nhân gốc rễ (Root Cause).
   - Luôn kèm theo bằng chứng xác thực: Kết quả test `48/48 pass`, log thực thi hoặc kiểm chứng trực quan trên giao diện.
