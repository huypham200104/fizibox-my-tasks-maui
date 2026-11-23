# Phản hồi bài thực hành .NET MAUI/C# — UI “Việc của tôi”

- **Người gửi:** Thanh Nguyen
- **Thời gian:** 22/11/2026, 00:21
- **Chủ đề:** Mời Huy thực hiện bài MAUI/C# cùng AI — UI “Việc của tôi”

---

Chào Huy,

Cảm ơn em đã nộp bài *“Việc của tôi”*. Bên anh đã clone repository, chạy unit test và chạy thử ứng dụng trên iOS Simulator. Bài làm có nhiều điểm tốt: đủ 4 màn hình theo đề, có skeleton loading, empty state và Not Found; có snackbar hoàn tác và công cụ giả lập cho reviewer; trạng thái giữa các màn được đồng bộ qua `TasksChanged`. README có ảnh minh họa, còn AI-NOTES ghi được những ví dụ cụ thể trong quá trình làm.

Trước buổi review, em xem lại các điểm dưới đây. *Không cần làm thêm tính năng mới; ưu tiên tính đúng, nhất quán và bằng chứng kiểm thử.*

## 1. Giữ nguyên bản đã nộp

Vì hạn nộp đã qua, em giữ nguyên phiên bản đã gửi, chẳng hạn gắn tag `submission` vào commit `1ff9e64`. Phần sửa thực hiện trên nhánh hoặc các commit tiếp theo, đồng thời thêm mục **“Thay đổi sau hạn nộp”** vào README, ghi rõ:

- Vấn đề đã sửa.
- Cách sửa và lý do lựa chọn.
- Cách kiểm tra lại cùng kết quả thực tế.

Bên anh cần đối chiếu được bản trước và sau, đồng thời hiểu cách em xử lý phản hồi.

## 2. Ưu tiên 1 — Các điểm cần sửa

### a. Lựa chọn lưu trữ chưa khớp với mô tả
Giả định số 5 trong README ghi dữ liệu lưu in-memory và reset khi mở lại ứng dụng. Hộp thoại “Đặt lại dữ liệu mẫu” cũng dùng chữ “in-memory”. Tuy nhiên, `MauiProgram.cs` đang đăng ký `LiteDbTaskService`, nên dữ liệu được lưu vào `mytasks.db` và giữ lại sau khi đóng ứng dụng.

Em chọn một hướng, rồi sửa **code, README và nội dung giao diện cho thống nhất**. Sau đó kiểm tra hành vi khi đóng/mở lại ứng dụng và khi đặt lại dữ liệu mẫu.

### b. Chuyển sang EN vẫn còn nội dung tiếng Việt
Đề yêu cầu chuyển ngôn ngữ cho toàn bộ nhãn, thông báo và hộp thoại. Hiện còn các vị trí:

- Nút vuốt “Hoàn thành / Xóa / Mở lại” trong `TodayPage`, `WeekPage`, `HistoryPage`.
- Form chỉnh sửa, các thứ “T2…CN”, “Hôm nay”, “Hủy / Xác nhận” và “Chọn dự án” trong `TaskDetailPage.xaml`.
- Snackbar tại `TodayViewModel.cs` dòng 399, 428, 452 và `WeekViewModel.cs` dòng 704.
- Các nhãn trong `MockStatePreviewPage.xaml`.

Các số dòng trên theo bản đã nộp. Em rà lại cả luồng thao tác, không chỉ các màn hình lúc mới mở.

### c. Bảng màu chưa có một nguồn dùng chung
Ngoài `Colors.xaml`, màu sáng/tối còn được khai báo lại bằng `Color.FromArgb` trong:

- `PreferencesSettingsService.cs`, dòng 158–201.
- `SettingsViewModel.cs`, dòng 112–205.

Một số file XAML trong `Views/` cũng còn mã màu hex viết trực tiếp. Mục tiêu là **đổi màu chủ đạo tại một nơi và các thành phần liên quan dùng lại đúng giá trị đó**, thay vì sửa đồng thời nhiều bảng màu. Đây cũng là dạng thay đổi nhỏ có thể được kiểm tra trong buổi review.

## 3. Ưu tiên 2 — Kiểm thử và tài liệu bàn giao

### a. Cập nhật đúng kết quả và nguyên nhân test thất bại
Khi bên anh chạy `dotnet test`, kết quả là **42/48 test pass, 6 test fail**. README đang ghi **41/47** và giải thích các test fail đều do ngày viết cứng. Cần tách rõ:

- **3 test settings:** `SettingsViewModelTests` cho Theme, FontSize và Language không liên quan đến ngày. Khả năng là test đang dùng `PreferencesSettingsService` thật, phụ thuộc vào `Preferences` của MAUI; em cần kiểm tra để xác nhận nguyên nhân.
- **3 test phụ thuộc ngày:** hai test trong `HistoryViewModelTests` và `WeekViewModelTests.OnTasksChanged_…`.

Hướng xử lý đề xuất là dùng service giả cho settings và truyền nguồn ngày vào thay vì gọi trực tiếp `DateTime.Today`. Sau khi sửa, em chạy lại và ghi đúng kết quả. Nếu còn test fail, ghi rõ test nào, nguyên nhân đã xác minh hay mới là giả thuyết, và phần chưa xử lý.

### b. Sửa các tham chiếu không tồn tại
README đang dẫn đến các mục chưa thấy trong repository:

- `docs/mistake.md`
- `docs/CHI_TIET_VIEC_UX_NOTES.md`
- `Resources/Strings.xaml`
- `Controls/TaskCard`

Em bổ sung nếu đó là nội dung cần bàn giao, hoặc sửa/xóa tham chiếu cho đúng thực tế.

### c. Dọn repository
- Loại bỏ `prefs.xml` ở thư mục gốc nếu đây chỉ là file dump preferences phục vụ kiểm tra trên thiết bị.
- Giữ một bộ ảnh dùng chung, tránh trùng giữa `screenshots/` và `docs/screenshots/`.

## 4. Ưu tiên 3 — Chuẩn bị giải thích, không bắt buộc sửa trước

- **Cấu trúc giao diện:** `TaskDetailPage.xaml` khoảng 1.391 dòng, `WeekPage.xaml` 1.268 dòng và `TodayPage.xaml` 1.027 dòng; template thẻ công việc đang lặp lại ở nhiều nơi. Em chuẩn bị giải thích cách tổ chức hiện tại và hướng tách thành `TaskCard` dùng chung. Nếu refactor, cần kiểm tra để không làm thay đổi hành vi ngoài ý muốn.
- **Chờ giao diện bằng thời gian cố định:** code-behind có nhiều `Task.Delay(50/80/150)`. Em chuẩn bị giải thích từng trường hợp cần chờ điều gì, rủi ro trên thiết bị khác và có cách nào chắc chắn hơn không.
- **Khả năng tiếp cận:** ứng dụng tắt `FontAutoScalingEnabled` toàn bộ, nên không đi theo cỡ chữ hệ thống. Nếu đây là lựa chọn có chủ đích, em ghi rõ lý do và đánh đổi trong phần quyết định thiết kế.
- **Phạm vi làm thêm:** các phần Lịch sử, tạo/sửa/xóa và chọn dự án không được cộng điểm chỉ vì vượt đề. Một phần đáng kể lỗi ngôn ngữ và kiểm thử hiện nằm ở các phần mở rộng này. Vì vậy, em nên tập trung hoàn thiện phạm vi hiện có thay vì bổ sung chức năng.

## 5. Chuẩn bị cho buổi review

Em có thể tự thử một vài thay đổi nhỏ:

- Đổi màu chủ đạo.
- Thêm số công việc quá hạn lên header.
- Đổi ngày bắt đầu tuần.

Với mỗi thay đổi, hãy xác định phạm vi ảnh hưởng, cách thực hiện và cách kiểm tra lại — gồm các chế độ **Sáng/Tối, cỡ chữ Lớn và VI/EN** khi có liên quan.

*Em tiếp tục được dùng AI.* Trong buổi review, bên anh muốn hiểu cách em giao việc cho AI, xem xét thay đổi, quyết định giữ/sửa/bỏ và dùng kiểm thử để xác nhận kết quả. Không cần chuẩn bị một lịch sử làm việc “đẹp”; những lần phát hiện sai và sửa có căn cứ cũng rất có giá trị.

Khi trình bày, em phân biệt rõ **“đã làm”, “đã thử” và “chưa kiểm chứng”**. Nếu chưa xử lý hết, ghi trung thực phần còn lại và lý do ưu tiên.

Có điểm nào chưa rõ hoặc gặp vướng khi chạy lại, em phản hồi trong luồng email này nhé. Hẹn gặp em ở buổi review!

Trân trọng,  
**Thanh Nguyen**