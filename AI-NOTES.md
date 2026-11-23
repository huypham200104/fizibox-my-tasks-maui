# Nhật Ký Ứng Dụng AI (AI-NOTES.md)

> **Dự án thực hành Fresher Mobile (.NET MAUI / C#) — "Việc của tôi" (MyTasksApp)**  
> **Đơn vị đánh giá:** Fizibox (`https://fizibox.com`)  
> **Tài liệu tham chiếu:** [ASSIGNMENT.md](docs/handover/ASSIGNMENT.md) (Mục 4.3), [ROADMAP.md](ROADMAP.md) (Mục 8.2)

---

## 1. Công Cụ AI Đã Sử Dụng & Phương Pháp Phân Chia Công Việc

### 1.1. Công cụ AI sử dụng
- **Công cụ:** Trợ lý trí tuệ nhân tạo **Antigravity AI Agent** tích hợp mô hình ngôn ngữ lớn **Gemini 3.8 Flash (High)** của Google DeepMind.
- **Kỹ thuật & Tiện ích hỗ trợ:**
  - Quy trình tương tác phản biện thiết kế qua Slash Command `/grill-me` nhằm làm rõ mọi giả định nghiệp vụ trước khi code.
  - Xây dựng kế hoạch triển khai chi tiết (`Superpowers Implementation Plans`) theo phương pháp Test-Driven Development (TDD).

### 1.2. Cách thức phân chia công việc (Human-AI Collaboration)
Ứng dụng nguyên tắc **"Người định hướng & kiểm soát — AI hiện thực & mở rộng"**:

| Vai trò | Phân công nhiệm vụ cụ thể |
|---|---|
| **Người lập trình (Human Developer)** | • **Định hướng kiến trúc & tiêu chuẩn:** Đặt ra quy tắc Clean Architecture + MVVM, hệ thống Design Tokens tập trung và 5 quyết định nghiệp vụ tự quyết.<br>• **Trực tiếp kiểm soát chất lượng (Code Review):** Đọc kỹ từng đoạn code XAML và C#, từ chối các thư viện 3rd-party không cần thiết.<br>• **Kiểm thử thực tế trên phần cứng:** Trực tiếp chạy app trên thiết bị thật **Xiaomi POCO X6 Pro (Android 15 HyperOS)**, phát hiện các lỗi môi trường runtime mà AI không nhìn thấy trên môi trường giả định.<br>• **Ra quyết định cuối cùng:** Điều chỉnh UX/UI khi tương tác thực tế bị giật/khựng hoặc thiếu tự nhiên. |
| **Trợ lý AI (Antigravity / Gemini)** | • **Lập dàn ý & kế hoạch chi tiết:** Phân rã bài toán thành các sub-tasks theo mẫu TDD (viết test trước -> viết mã nguồn -> kiểm thử pass).<br>• **Sinh mã nguồn chuẩn hóa:** Viết mã XAML theo Design Tokens, viết ViewModels kế thừa `BaseViewModel`, triển khai Data Seeder và Services.<br>• **Xây dựng bộ Unit Tests:** Tạo 47 test cases bao quát logic ViewModel, Seeder và State synchronization bằng `xUnit` và `FluentAssertions`.<br>• **Phân tích log & gỡ lỗi:** Phân tích stack trace từ Android Logcat và lỗi MSBuild để tìm nguyên nhân gốc rễ. |

---

## 2. Ba Ví Dụ Tiêu Biểu Trong Quá Trình Phát Triển

### Ví dụ 1: Thiết kế giao diện và xử lý tương tác Custom Floating SnackBar đa năng

* **Yêu cầu / Prompt thực tế từ người dùng:**
  > *"Thanh snackbar hiện tại đang hiển thị chưa đúng light mode, chỉ hiển thị dark mode. Ngoài ra khi dài nó nó đang hiển thị ... thay vì hiển thị hết /grill-me"*  
  > *"Tại sao tôi bấm dấu X trong snack bar mà nó không chịu tắt vậy"*
* **Kết quả AI tạo ban đầu:**
  - AI thiết kế một Custom Control với nền cố định màu tối `#1E293B`, dùng layout `HorizontalStackLayout`. Khi thông báo dài kết hợp cùng nút bấm, dòng chữ bị tràn ra ngoài màn hình và bị cắt cụt ba chấm (`...`).
  - Về logic đóng nhanh, AI gọi `Dismiss()` nhưng không hủy được token của `CancellationTokenSource`, dẫn đến việc timer đếm lùi 5000ms vẫn chạy ngầm và hiệu ứng đóng không phản hồi tức thì.
* **Quyết định của người lập trình & Lý do:**
  - **Quyết định: SỬA VÀ TÁI CẤU TRÚC (Modify & Refactor).**
  - **Lý do & Cách giải quyết:**
    1. Đổi toàn bộ layout sang `Grid ColumnDefinitions="Auto, *, Auto, Auto"` với cột văn bản chiếm `*` linh hoạt, kết hợp `LineBreakMode="WordWrap"` để không bao giờ bị cắt chữ dù ở cỡ chữ Lớn (tuân thủ quy tắc chống tràn trong [AGENTS.md](AGENTS.md)).
    2. Chuyển màu nền sang `{DynamicResource SnackbarBg}` để tự động thích ứng với cả Light Mode và Dark Mode.
    3. Tái cấu trúc `SnackbarService` để khi gọi `Dismiss()` sẽ lập tức kích hoạt `_cts?.Cancel()`, đồng thời chạy animation trượt xuống `TranslationY = 100` trong 200ms trên `MainThread`. Giữ lại phần logic hoàn tác `UndoDeleteAsync()` vì cơ chế này hoạt động rất mượt mà.

---

### Ví dụ 2: Điều hướng xem chi tiết công việc từ danh sách (Task Item Tap)

* **Yêu cầu / Prompt thực tế từ người dùng:**
  > *"Tại sao khi tôi bấm vào 1 task thì lại không thấy có gì xảy ra, tôi muốn nó hiển thị detail task chứ /grill-me"*
* **Kết quả AI tạo ban đầu:**
  - AI đề xuất truyền nguyên cả thực thể phức tạp `TaskItem` qua tham số điều hướng Shell:
    ```csharp
    var navigationParameter = new Dictionary<string, object> { { "Task", selectedTask } };
    await Shell.Current.GoToAsync(nameof(TaskDetailPage), navigationParameter);
    ```
* **Quyết định của người lập trình & Lý do:**
  - **Quyết định: THAY ĐỔI KIẾN TRÚC ĐIỀU HƯỚNG (Modify Architecture).**
  - **Lý do & Cách giải quyết:**
    1. Việc truyền nguyên đối tượng phức tạp giữa các trang trong MAUI dễ gây lỗi serialization, không hỗ trợ deeplink, và quan trọng nhất là **không kiểm thử được trạng thái Not Found** theo yêu cầu của đề bài [ASSIGNMENT.md](docs/handover/ASSIGNMENT.md).
    2. Người lập trình yêu cầu AI chỉ truyền duy nhất tham số nguyên thủy `id`:
       ```csharp
       await Shell.Current.GoToAsync($"{nameof(TaskDetailPage)}?id={selectedTask.Id}");
       ```
    3. Tại `TaskDetailViewModel`, hiện thực `IQueryAttributable` để trích xuất `id` và gọi `ITaskService.GetTaskByIdAsync(id)`. Nếu `id` không hợp lệ (ví dụ `id = -1` kích hoạt từ nút Reviewer Tools), ViewModel sẽ bật `IsNotFound = true` để hiển thị giao diện báo lỗi chuyên nghiệp thay vì văng ứng dụng.

---

### Ví dụ 3: Hiệu ứng chuyển đổi ngày trên màn hình Tuần này (Week Screen Skeleton Loading)

* **Yêu cầu / Prompt thực tế từ người dùng:**
  > *"Ở week-screen, khi đang ở 1 ngày và chuyển qua ngày hôm khác thì nên có skeleton để loading thay vì chỉ khựng như hiện tại /grill-me"*
* **Kết quả AI tạo ban đầu:**
  - AI đề xuất cài đặt thêm thư viện ngoài (NuGet package bên thứ ba để làm Shimmer animation) hoặc thêm hàm giả lập độ trễ `await Task.Delay(2000)` để người dùng "kịp nhìn thấy hiệu ứng".
* **Quyết định của người lập trình & Lý do:**
  - **Quyết định: BỎ THƯ VIỆN NGOÀI & BỎ ĐỘ TRỄ GIẢ DÀI (Reject 3rd-party & Fake Delay).**
  - **Lý do & Cách giải quyết:**
    1. Đề bài `ASSIGNMENT.md` (Mục 3.2) ghi rõ: *"Không tạo độ trễ giả dài chỉ để trình diễn Loading"*. Quy tắc `AGENTS.md` cũng cấm tự ý cài đặt thêm package ngoài.
    2. Người lập trình chỉ đạo AI tự viết các thẻ Skeleton Cards thuần XAML MAUI sử dụng `Border` bo góc và animation mờ dần (`Opacity` fade 150ms) kết hợp cờ `IsLoading`. Dữ liệu được nạp tức thì trong vòng vài mili-giây, giao diện chuyển cảnh mượt mà, tự nhiên mà không gây khó chịu cho người dùng.

---

## 3. Một Trường Hợp AI Tạo Kết Quả Sai / Chưa Phù Hợp & Cách Khắc Phục

### 3.1. Bối cảnh sự cố: Lỗi FastDeploy trên thiết bị thật Xiaomi (HyperOS / MIUI)

* **Hiện tượng:**
  Khi lập trình viên yêu cầu triển khai ứng dụng lên điện thoại thật Xiaomi để kiểm thử, AI liên tục hướng dẫn dùng lệnh chuẩn của MAUI:
  ```powershell
  dotnet build src\MyTasksApp\MyTasksApp.csproj -t:Run -f net10.0-android
  ```
  Lệnh này bị treo cứng suốt 130 giây và kết thúc với lỗi:
  ```text
  error XA0010: Selected device is not running / No available device
  ```
  Khi cố gắng copy file APK sang cài đặt thủ công, ứng dụng cài được nhưng vừa bấm vào logo là **bị văng (crash) tức thì ngay sau Splash Screen**.

### 3.2. Cách phát hiện nguyên nhân gốc rễ (Root Cause Analysis)
* Người lập trình chủ động dùng lệnh ADB để soi Logcat hệ thống Android:
  ```powershell
  adb logcat -d | Select-String "monodroid"
  ```
* Phát hiện thông báo lỗi nghiệm trọng:
  ```text
  F monodroid: No assemblies found in '/data/user/0/com.companyname.mytasksapp/files/.__override__/arm64-v8a'... Assuming this is part of Fast Deployment. Exiting...
  Fatal signal 6 (SIGABRT)
  ```
* **Bản chất vấn đề mà AI không lường trước được:**
  1. Mặc định ở chế độ Debug, .NET MAUI dùng cơ chế **FastDeployment** (tách toàn bộ DLL ra khỏi APK và đẩy qua socket vào thư mục tạm `__override__` trên thiết bị để tăng tốc độ build).
  2. Hệ điều hành **Xiaomi HyperOS / MIUI** có cơ chế bảo mật Sandbox nghiêm ngặt chặn các socket nội bộ này, khiến ứng dụng không tìm thấy thư viện mã nguồn khi nạp bộ nhớ.
  3. Môi trường máy tính của người dùng tồn tại 2 phiên bản `adb.exe` khác nhau (v36 trong Android SDK và v37 ở thư mục khác) dẫn đến việc daemon ADB trên cổng TCP 5037 bị khởi động lại liên tục, làm lệnh `-t:Run` của MAUI bị mất kết nối.

### 3.3. Giải pháp khắc phục triệt để & Quy trình kiểm tra lại
* **Cách khắc phục:**
  1. Bác bỏ hoàn toàn cơ chế FastDeploy của MAUI bằng cách thêm cờ bắt buộc:
     `-p:EmbedAssembliesIntoApk=true`  
     Cờ này ép buộc compiler đóng gói toàn bộ thư viện DLL trực tiếp vào trong file APK độc lập (Self-contained APK).
  2. Thay thế lệnh `dotnet build -t:Run` bằng quy trình CLI ba bước tiêu chuẩn qua ADB:
     ```powershell
     # Bước 1: Build APK tự chứa đầy đủ thư viện
     dotnet build src\MyTasksApp\MyTasksApp.csproj -f net10.0-android -p:EmbedAssembliesIntoApk=true

     # Bước 2: Đẩy và cài đặt an toàn
     adb push src\MyTasksApp\bin\Debug\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk
     adb shell pm install -r /data/local/tmp/app.apk

     # Bước 3: Khởi chạy Activity chính
     adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity
     ```
* **Kiểm tra lại kết quả:**
  - Ứng dụng khởi động thành công ngay tức thì trên điện thoại Xiaomi, đạt hiệu năng 60/120fps mượt mà, **0 crash**.
  - Toàn bộ bài học kinh nghiệm này đã được đúc kết vào tài liệu hướng dẫn trong [README.md](README.md).

---

## 4. Bài Học Kinh Nghiệm Khi Làm Việc Với AI

1. **AI là trợ thủ hiện thực hóa, không phải người kiến trúc thay thế con người:** AI rất mạnh trong việc sinh boilerplate code, thiết lập unit test và tra cứu cú pháp; nhưng con người phải là người giữ vững nguyên tắc kiến trúc (Clean Architecture, MVVM) và trải nghiệm người dùng thực tế.
2. **Kỹ thuật "Phỏng vấn ngược" (`/grill-me`) giúp tiết kiệm 80% thời gian sửa lỗi:** Thay vì để AI tự suy diễn và tạo ra mã nguồn mơ hồ, việc đối thoại làm rõ từng giả định thiết kế trước khi bắt tay vào viết code giúp hạn chế tối đa các sửa đổi lớn về sau.
3. **Môi trường thực tế là tiêu chuẩn nghiệm thu duy nhất:** Mọi đoạn code "chạy được trên lý thuyết" của AI đều cần được đưa lên thiết bị thật để kiểm chứng độ mượt, phản hồi chạm, màu sắc tương phản và tính tương thích của hệ điều hành.

---
*Hoàn tất ngày 21/11/2026 — Hồ sơ thực hành Fresher Mobile (.NET MAUI) Fizibox.*
