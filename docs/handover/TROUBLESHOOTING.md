# Nhật ký lỗi & Giải pháp khắc phục (Troubleshooting Log)

Tài liệu ghi lại các lỗi phát sinh trong quá trình thiết lập, build và triển khai ứng dụng .NET MAUI trên thiết bị thực tế, kèm nguyên nhân gốc rễ và cách xử lý.

---

## 1. Lỗi XAML: Thiếu StaticResource (`MidnightBlue` & `Magenta`)

- **Hiện tượng:** Ứng dụng bị crash ngay khi khởi động sau màn hình Splash. Logcat ghi nhận:
  ```text
  [Microsoft.Maui.Controls.Xaml.XamlParseException]: Position 176:38. StaticResource not found for key MidnightBlue
  ```
- **Nguyên nhân:** Khi tái cấu trúc `Colors.xaml` theo hệ Design Tokens mới (`DESIGN.md`), các mã màu mặc định từ template gốc của .NET MAUI (`MidnightBlue`, `Magenta`) bị xóa bỏ, trong khi `Styles.xaml` vẫn còn một số style kế thừa tham chiếu đến các khóa này.
- **Cách sửa:**
  - Bổ sung fallback compatibility trong `Colors.xaml` ánh xạ `MidnightBlue` và `Magenta` về hệ màu token (`TextPrimary`, `Primary`).
  - Cập nhật triệt để các style trong `Styles.xaml` (Headline, SubHeadline, TabBar, NavigationPage) sang các token chuẩn `TextPrimary` và `Primary`.

---

## 2. Lỗi FastDeploy trên thiết bị Xiaomi (MIUI / HyperOS)

- **Hiện tượng:**
  - Chạy lệnh `dotnet build -t:Run -f net10.0-android` bị treo khoảng 130 giây và báo lỗi:
    ```text
    error XA0010: Selected device is not running / No available device
    ```
  - App cài được lên điện thoại nhưng bấm vào bị văng ra ngay. Logcat ghi nhận:
    ```text
    F monodroid: No assemblies found in '/data/user/0/.../files/.__override__/arm64-v8a'... Assuming this is part of Fast Deployment. Exiting...
    Fatal signal 6 (SIGABRT)
    ```
- **Nguyên nhân:**
  - Mặc định ở cấu hình `Debug`, .NET MAUI sử dụng cơ chế **FastDeployment** (tách toàn bộ file DLL ra khỏi file APK và đẩy vào thư mục tạm trên thiết bị để tăng tốc build).
  - Hệ điều hành MIUI/HyperOS có cơ chế bảo mật nghiêm ngặt chặn socket FastDeploy và hạn chế quyền ghi đè thư mục nội bộ, dẫn tới việc APK không tìm thấy DLL mã nguồn khi khởi động.
  - Xung đột giữa 2 phiên bản `adb.exe` trên hệ điều hành máy tính (`AppData\Local\Android\Sdk` v36 và `C:\Android\SDK` v37) liên tục ngắt kết nối daemon trên cổng TCP 5037.
- **Cách sửa:**
  - Bật cờ `-p:EmbedAssembliesIntoApk=true` khi build để đóng gói toàn bộ assemblies trực tiếp vào APK (Self-contained APK), không phụ thuộc vào FastDeploy.
  - Sử dụng quy trình cài đặt qua `pm install` và khởi chạy qua `am start` thay cho target `-t:Run`:
    ```powershell
    # Build APK tự chứa đầy đủ thư viện
    dotnet build -f net10.0-android -p:EmbedAssembliesIntoApk=true

    # Đẩy và cài đặt an toàn qua ADB
    adb push bin\Debug\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk
    adb shell pm install -r /data/local/tmp/app.apk

    # Khởi chạy ứng dụng
    adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity
    ```
