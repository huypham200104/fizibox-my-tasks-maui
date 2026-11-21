# Script tự động chụp 9 ảnh màn hình kiểm thử từ điện thoại qua ADB
# Sử dụng: Chạy file này trong PowerShell: .\capture_screenshots.ps1

$device = "UCYTC6XK79KFGMMZ"
$outputDir = "screenshots"

if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}

$steps = @(
    @{ Name = "01_today_light.png"; Desc = "1. Màn hình 'Hôm nay' (Giao diện Sáng, Cỡ chữ Thường, Tiếng Việt)" },
    @{ Name = "02_week_light.png"; Desc = "2. Màn hình 'Tuần này' (Giao diện Sáng)" },
    @{ Name = "03_detail_light.png"; Desc = "3. Màn hình 'Chi tiết việc' (Giao diện Sáng - bấm vào 1 công việc)" },
    @{ Name = "04_history_light.png"; Desc = "4. Màn hình 'Lịch sử' (Tab Đã xong - Giao diện Sáng)" },
    @{ Name = "05_settings_light.png"; Desc = "5. Màn hình 'Cài đặt' (Giao diện Sáng, hiển thị các tùy chọn)" },
    @{ Name = "06_today_dark.png"; Desc = "6. Màn hình 'Hôm nay' ở chế độ Dark Mode (Giao diện Tối)" },
    @{ Name = "07_today_large_font.png"; Desc = "7. Màn hình 'Hôm nay' ở chế độ Cỡ chữ Lớn (Large Font)" },
    @{ Name = "08_state_loading.png"; Desc = "8. Trạng thái 'Đang tải' (Loading Skeleton - mở từ Cài đặt -> Reviewer Tools)" },
    @{ Name = "09_state_not_found.png"; Desc = "9. Trạng thái 'Không tìm thấy' (Not Found - mở từ Cài đặt -> Reviewer Tools)" },
    @{ Name = "10_state_empty.png"; Desc = "10. Trạng thái 'Danh sách trống' (Empty State - mở từ Cài đặt -> Reviewer Tools)" }
)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  TRÌNH HỖ TRỢ CHỤP MÀN HÌNH KIỂM THỬ MYTASKSAPP" -ForegroundColor Yellow
Write-Host "  Thiết bị mục tiêu: $device" -ForegroundColor Green
Write-Host "  Thư mục lưu ảnh: $outputDir" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host ""

foreach ($step in $steps) {
    Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host "BƯỚC: $($step.Desc)" -ForegroundColor Yellow
    Write-Host "-> Hãy mở màn hình tương ứng trên điện thoại của bạn."
    $null = Read-Host "-> Khi màn hình đã sẵn sàng, nhấn ENTER để chụp"
    
    $filePath = Join-Path $outputDir $step.Name
    adb -s $device shell screencap -p /sdcard/screencap_temp.png
    adb -s $device pull /sdcard/screencap_temp.png $filePath | Out-Null
    adb -s $device shell rm /sdcard/screencap_temp.png
    
    if (Test-Path $filePath) {
        Write-Host "=> ĐÃ CHỤP THÀNH CÔNG: $($step.Name)" -ForegroundColor Green
    } else {
        Write-Host "=> LỖI: Không lưu được $($step.Name)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  HOÀN TẤT TOÀN BỘ 10 ẢNH CHỤP MÀN HÌNH!" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Cyan
