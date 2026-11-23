# HƯỚNG DẪN CHẠY VÀ REBUILD DỰ ÁN (.NET MAUI)
## "VIỆC CỦA TÔI" (MYTASKSAPP)

> **Mục tiêu:** Cung cấp hướng dẫn từng bước để cài đặt, khởi chạy dự án trên Windows / Android và quy trình chuẩn để build lại (rebuild/redeploy) nhanh chóng mỗi khi có thay đổi mã nguồn (C# hoặc XAML).

---

## 1. Yêu Cầu Môi Trường (Prerequisites)

1. **.NET 10 SDK:** Kiểm tra bằng lệnh:
   ```powershell
   dotnet --version
   # Kết quả: 10.0.xxx
   ```
2. **MAUI Workloads:**
   ```powershell
   dotnet workload list
   # Đảm bảo có: maui-android, maui-windows (hoặc maui)
   ```
3. **Môi trường chạy:**
   - **Cách 1: Máy tính Windows (Nhanh nhất):** Không cần điện thoại hay máy ảo, chạy trực tiếp trên Windows Desktop.
   - **Cách 2: Điện thoại Android thật:** Đã bật *"Tùy chọn cho nhà phát triển"* (Developer Options) và *"Gỡ lỗi USB"* (USB Debugging). Kiểm tra kết nối bằng lệnh:
     ```powershell
     adb devices
     # Kết quả hiển thị ID thiết bị: <Device_ID> device
     ```

---

## 2. Hướng Dẫn Khởi Chạy Dự Án Lần Đầu

### 2.1. Khởi chạy trên Windows Desktop (Khuyên dùng khi code nhanh)
Chạy ứng dụng trực tiếp dưới dạng Windows Desktop App:
```powershell
# Chuyển về thư mục gốc dự án
cd c:\Workplace\GitHub\fizibox-my-tasks-maui

# Build và chạy ngay lập tức
dotnet build src\MyTasksApp\MyTasksApp.csproj -t:Run -f net10.0-windows10.0.19041.0
```

---

### 2.2. Khởi chạy trên Điện thoại Android (Cắm cáp qua ADB)

> [!IMPORTANT]
> Trên các dòng máy Android có bảo mật cao (như **Xiaomi HyperOS / MIUI**), cơ chế FastDeployment mặc định của .NET MAUI Debug có thể bị chặn. Vì vậy, ta luôn kèm cờ `-p:EmbedAssembliesIntoApk=true` để toàn bộ thư viện được đóng gói trực tiếp vào file APK độc lập (Self-contained).

#### Quy trình chuẩn 3 bước:
```powershell
# Bước 1: Build gói APK tự chứa
dotnet build src\MyTasksApp\MyTasksApp.csproj -f net10.0-android -p:EmbedAssembliesIntoApk=true

# Bước 2: Đẩy và cài đặt đè lên máy qua ADB
adb push src\MyTasksApp\bin\Debug\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk
adb shell pm install -r /data/local/tmp/app.apk

# Bước 3: Khởi chạy app lên màn hình
adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity
```

---

## 3. Quy Trình Build Lại Mỗi Khi Thay Đổi Code (Rebuild Workflow)

Mỗi khi bạn sửa code (C# ViewModel, Service hoặc XAML View), hãy làm theo các hướng dẫn dưới đây tùy theo môi trường bạn đang test:

### 3.1. Đối với Windows Desktop
Mỗi khi sửa code, chỉ cần chạy lại 1 lệnh duy nhất:
```powershell
dotnet build src\MyTasksApp\MyTasksApp.csproj -t:Run -f net10.0-windows10.0.19041.0
```
*.NET Compiler sẽ tự động phát hiện các file bị thay đổi (Incremental Build), biên dịch chỉ các file đó và bật cửa sổ ứng dụng mới.*

---

### 3.2. Đối với Điện thoại Android: Lệnh One-Liner (Khuyên Dùng)
Để không phải gõ lại từng bước `build -> push -> install -> start`, bạn chỉ cần copy nguyên khối lệnh 1 dòng dưới đây vào PowerShell:

#### 👉 Lệnh One-Liner cho bản DEBUG (Biên dịch nhanh khi đang phát triển):
```powershell
dotnet build src\MyTasksApp\MyTasksApp.csproj -f net10.0-android -p:EmbedAssembliesIntoApk=true; if ($?) { adb push src\MyTasksApp\bin\Debug\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk; adb shell pm install -r /data/local/tmp/app.apk; adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity }
```

#### 👉 Lệnh One-Liner cho bản RELEASE (Chạy mượt nhất, Native AOT, không giật lag):
Khi muốn trải nghiệm độ mượt tối đa hoặc chuẩn bị demo phỏng vấn:
```powershell
dotnet build src\MyTasksApp\MyTasksApp.csproj -f net10.0-android -c Release -p:EmbedAssembliesIntoApk=true; if ($?) { adb push src\MyTasksApp\bin\Release\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk; adb shell pm install -r /data/local/tmp/app.apk; adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity }
```

---

## 4. Các Thao Tác Bổ Trợ Thường Dùng Khi Phát Triển

### 4.1. Xóa sạch dữ liệu cũ để test lại từ đầu (Reset Database)
Nếu bạn vừa thêm tính năng mới và muốn ứng dụng trên điện thoại quay về trạng thái ban đầu (xóa sạch database LiteDB và Preferences):
```powershell
# Xóa sạch dữ liệu ứng dụng trên máy Android
adb shell pm clear com.companyname.mytasksapp

# Khởi chạy lại app
adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity
```

---

### 4.2. Chạy kiểm thử tự động (Unit Tests)
Trước khi bàn giao hoặc sau khi sửa logic core, kiểm tra lại 48 unit tests:
```powershell
dotnet test tests\MyTasksApp.Tests\MyTasksApp.Tests.csproj -f net10.0-windows10.0.19041.0
# Đảm bảo: Passed: 48, Failed: 0
```

---

### 4.3. Xử lý sự cố khi Build bị lỗi (Troubleshooting)

| Hiện tượng | Nguyên nhân | Cách xử lý |
|---|---|---|
| **Lỗi `file is being used by another process`** | Ứng dụng Windows cũ đang mở ngầm hoặc bị lock DLL. | Tắt cửa sổ app đang chạy, hoặc chạy `taskkill /IM MyTasksApp.exe /F` rồi build lại. |
| **Lỗi `error XA0010: No available device`** | Điện thoại bị ngắt kết nối USB hoặc chưa cấp quyền Debug. | Rút cáp cắm lại, kiểm tra màn hình điện thoại bấm "Cho phép gỡ lỗi USB", gõ `adb devices`. |
| **App cài được nhưng bấm vào văng ra ngay (Crash)** | Thiết bị chặn FastDeploy. | Đảm bảo lệnh build luôn có cờ `-p:EmbedAssembliesIntoApk=true`. |
| **Code sửa rồi nhưng trên app không thấy đổi** | Build cache bị dính file cũ. | Chạy lệnh dọn sạch cache: `dotnet clean src\MyTasksApp\MyTasksApp.csproj` sau đó build lại. |

---

## 5. Tóm Tắt Cheat-Sheet Các Lệnh Cơ Bản

```powershell
# 1. Chạy trên Windows
dotnet build src\MyTasksApp\MyTasksApp.csproj -t:Run -f net10.0-windows10.0.19041.0

# 2. Build & Deploy Android (One-Liner)
dotnet build src\MyTasksApp\MyTasksApp.csproj -f net10.0-android -p:EmbedAssembliesIntoApk=true; adb push src\MyTasksApp\bin\Debug\net10.0-android\com.companyname.mytasksapp-Signed.apk /data/local/tmp/app.apk; adb shell pm install -r /data/local/tmp/app.apk; adb shell am start -n com.companyname.mytasksapp/crc64f7d2969e836db1e0.MainActivity

# 3. Chạy Unit Tests
dotnet test tests\MyTasksApp.Tests\MyTasksApp.Tests.csproj

# 4. Xóa dữ liệu app trên điện thoại
adb shell pm clear com.companyname.mytasksapp
```
