# HỆ THỐNG THIẾT KẾ (DESIGN SYSTEM SPECIFICATION)

> **Dự án:** Việc của tôi (MyTasksApp) — .NET MAUI Mobile App  
> **Phiên bản:** 2.1 (Tinh gọn, chuẩn hóa theo bộ thiết kế thực tế tại `ui_design/`)  
> **Nguyên tắc cốt lõi:** 100% Token-Driven, Single Source of Truth (`AppColors.cs`), WCAG AA Contrast, Ergonomic Thumb-Zone.

---

## 1. Nguyên Tắc Cốt Lõi & Quy Chuẩn Chung (Core Principles)

1. **Token-Driven & Single Source of Truth:**
   - Toàn bộ màu sắc quản lý tập trung tại `AppColors.cs` và ánh xạ qua `Colors.xaml` (`DynamicResource`). Tuyệt đối không hardcode mã hex trong code hay XAML.
2. **Công thái học di động (Mobile Thumb-Zone):**
   - Vùng tương tác chính (TabBar 4 tab, nút FAB `+`, chuyển trạng thái, Bottom Sheets) bố trí ở 2/3 dưới màn hình.
   - **Vùng chạm tối thiểu (Touch Target):** Mọi phần tử tương tác (Checkbox, Button, FAB, Icon) đạt tối thiểu **44x44dp** (hoặc ≥ 36x36dp kèm padding).
3. **Bố cục Thẻ công việc 2 tầng (Two-Tier Task Card):**
   - Để tiêu đề không bao giờ bị cắt cụt (`...`), luôn bố trí 2 tầng: Dòng trên dành cho Checkbox và Tiêu đề; Dòng dưới dành cho Tag dự án và Badge thời hạn/giờ hẹn.
4. **Chống tràn nội dung (Anti-overflow) & Co giãn:**
   - Không đặt văn bản dài cùng số liệu động trong `HorizontalStackLayout` chật hẹp; ưu tiên `Grid` (`*` kết hợp `Auto`) hoặc `FlexLayout`.
   - Mọi `Label` nội dung công việc đều đặt `LineBreakMode="WordWrap"`.
5. **Độ tương phản cao & Dark Theme (WCAG AA):**
   - Đảm bảo tỷ lệ tương phản chữ trên nền đạt chuẩn WCAG AA ≥ 4.5:1 (chữ chính ≥ 7:1).
   - Dark Mode tối ưu độ tương phản bề mặt (Surface elevation) và đường viền phân tách thay vì thuần màu đen.
6. **Cắt góc ảnh chuẩn xác (Image Clipping):**
   - Với hình ảnh hoặc logo có nền riêng, luôn sử dụng `Clip` hình học (`RoundRectangleGeometry`) tương ứng với góc bo container để triệt tiêu các góc vuông nhọn tràn ra ngoài.

---

## 2. Hệ Thống Design Tokens

### 2.1. Bảng Màu (Color Tokens — Ánh xạ trực tiếp từ `AppColors.cs`)

| Token Key | Tên ngữ nghĩa | Light Mode | Dark Mode | Mục đích sử dụng |
|---|---|---|---|---|
| `Primary` | Màu thương hiệu chính | `#4F46E5` | `#6366F1` | Nút CTA, FAB (+), ngày đang chọn, tab active |
| `PrimaryVariant` | Nhấn bổ trợ | `#4338CA` | `#818CF8` | Trạng thái pressed, badge nổi bật |
| `PrimaryLight` | Nền thương hiệu nhạt | `#EEF2FF` | `#1E1B4B` | Nền thẻ tiến độ, chip active, viền ngày hôm nay |
| `Background` | Nền ứng dụng chính | `#F8FAFC` | `#0F172A` | Nền trang (`ContentPage.BackgroundColor`) |
| `Surface` | Nền thẻ & thành phần | `#FFFFFF` | `#1E293B` | Nền Task Card, Week Item, Bottom Sheet |
| `SurfaceVariant` | Nền phân cách/phụ | `#F1F5F9` | `#334155` | Nền Skeleton Shimmer, khung nhập liệu, chip unselected |
| `BorderColor` | Màu đường viền | `#E2E8F0` | `#334155` | Viền thẻ, đường kẻ phân cách, viền ô nhập |
| `TextPrimary` | Chữ chính | `#0F172A` | `#F8FAFC` | Tiêu đề công việc, heading (WCAG ≥ 7:1) |
| `TextSecondary` | Chữ phụ | `#475569` | `#94A3B8` | Tên dự án, mô tả, nhãn ngày tháng (WCAG ≥ 4.5:1) |
| `TextMuted` | Chữ mờ/chú thích | `#94A3B8` | `#64748B` | Placeholder, việc gạch ngang, bộ đếm ký tự |
| `Danger` | Cảnh báo quá hạn / Lỗi | `#DC2626` | `#EF4444` | Nhãn "Quá hạn", badge hạn chót đỏ |
| `DangerSurface` | Nền cảnh báo quá hạn | `#FEF2F2` | `#450A0A` | Nền chip quá hạn, nền icon báo lỗi |
| `Warning` | Đang làm / Hạn hôm nay | `#D97706` | `#F59E0B` | Chấm "Đang thực hiện", badge "Hôm nay" |
| `WarningSurface` | Nền hạn hôm nay | `#FFFBEB` | `#451A03` | Nền chip hạn hôm nay, nền ghi chú |
| `Success` | Hoàn thành / Đã xong | `#16A34A` | `#22C55E` | Nhãn "Đã xong", icon check xanh |
| `SuccessSurface` | Nền hoàn thành nhẹ | `#F0FDF4` | `#052E16` | Nền thẻ hoàn thành, nền icon tích xanh |
| `ProgressTrack` | Rãnh tiến độ | `#E0E7FF` | `#312E81` | Nền rãnh thanh ProgressBar |
| `ProgressFill` | Vệt tiến độ | `#4F46E5` | `#6366F1` | Vệt màu % hoàn thành |
| `SnackbarBg` | Nền Snackbar | `#FFFFFF` | `#020617` | Nền thanh thông báo nổi |
| `SnackbarUndoBg` | Nền nút Hoàn tác | `#EEF2FF` | `#1E1B4B` | Nền nút Hoàn tác |
| `SnackbarUndoText` | Chữ nút Hoàn tác | `#4F46E5` | `#818CF8` | Chữ nút Hoàn tác |

### 2.2. Khoảng Cách (Spacing Grid — Bội số 4dp / 8dp)

| Token | Giá trị | Ứng dụng |
|---|---|---|
| `SpacingXs` | `4dp` | Khoảng cách giữa icon và text nhỏ, padding badge/chip |
| `SpacingSm` | `8dp` | Khoảng cách giữa các chip lọc, tiêu đề và phụ đề |
| `SpacingMd` | `12dp` | Padding thẻ nhỏ, khoảng cách giữa các phần tử form |
| `SpacingBase` | `16dp` | Padding chuẩn của Task Card, khoảng cách giữa các card |
| `SpacingLg` | `20dp` | Padding lề màn hình (`ScreenPadding = 20dp`) |
| `SpacingXl` | `24dp` | Khoảng cách giữa các Section phân nhóm (Quá hạn / Hôm nay / Xong) |
| `SpacingXxl` | `32dp` | Khoảng cách trước tiêu đề lớn hoặc vùng Empty State |

### 2.3. Kiểu Chữ & Elastic Typography Scaling

Hỗ trợ 3 mức cỡ chữ co giãn động ngay lập tức qua `SettingsViewModel`:

| Token Key | Nhỏ (Small) | Thường (Medium - Mặc định) | Lớn (Large) | Font Weight | Line Height |
|---|---|---|---|---|---|
| `FontSizeDisplay` | `22sp` | `26sp` | `30sp` | Bold (700) | `1.2` |
| `FontSizeTitle` | `17sp` | `20sp` | `24sp` | SemiBold (600) | `1.3` |
| `FontSizeSubtitle` | `14sp` | `16sp` | `19sp` | Medium (500) | `1.4` |
| `FontSizeBody` | `13sp` | `15sp` | `18sp` | Regular (400) | `1.5` |
| `FontSizeCaption` | `11sp` | `13sp` | `15sp` | Regular (400) | `1.4` |
| `FontSizeBadge` | `10sp` | `12sp` | `13sp` | SemiBold (600) | `1.2` |

### 2.4. Bo Góc (Corner Radius) & Đổ Bóng (Elevation)

| Token | Giá trị | Ứng dụng |
|---|---|---|
| `RadiusSm` | `6dp` | Badge, chip trạng thái, tag dự án |
| `RadiusMd` | `12dp` | Task Card, Week Day Item, nút bấm, ô nhập liệu |
| `RadiusLg` | `16dp` | Thẻ tiến độ, Dialog Card, Bottom Sheet Card |
| `RadiusFull` | `999dp` | Checkbox tròn, Nút FAB (+), Pill Badge, Filter Chip |

* **Đổ bóng (Elevation / Shadow):**
  - **Light Mode:** `Shadow Offset="0,2" Radius="8" Opacity="0.06"` (thanh lịch, nhẹ nhàng).
  - **Dark Mode:** `Shadow Opacity="0"`, sử dụng viền tinh tế `BorderColor` (`1dp`) để phân định khối.

---

## 3. Quy Chuẩn Thành Phần Giao Diện (Core Components)

### 3.1. Thẻ Công Việc (Task Card Item)
- **Bố cục 2 tầng:**
  - **Tầng trên:** Checkbox tròn (vùng chạm 44x44dp) + Tiêu đề công việc (`FontSizeTitle`, gạch ngang khi đã hoàn thành).
  - **Tầng dưới:** Chip Dự án (icon Folder, nền `SurfaceVariant`) + Badge hạn chót/thời gian (`DangerSurface` nếu quá hạn, `WarningSurface` nếu hôm nay).
- **Hành vi Vuốt (SwipeView):**
  - Vuốt phải qua trái (RTL): Nút xanh `Success` để hoàn thành việc.
  - Vuốt trái qua phải (LTR): Nút đỏ `Danger` để xóa việc (kích hoạt Snackbar Undo).

### 3.2. Thẻ Tiến Độ Ngang (Linear Progress Card)
- Bo góc `RadiusLg` (16dp), nền `PrimaryLight`.
- Hàng trên: Icon Checkbox thương hiệu + Tiêu đề + Chỉ số % hoàn thành và tỷ số việc (VD: `5/8`).
- Hàng dưới: `ProgressBar` chiều cao `6dp`, nền rãnh `ProgressTrack`, vệt tiến độ `ProgressFill`.

### 3.3. Dải Lịch 7 Ngày Ngang (Week Strip)
- **Thanh điều hướng:** Nút `<` / `>` điều hướng tuần + Nhãn khoảng ngày (*"16 – 22 Tháng 11, 2026"*).
- **7 Cột ngày (T2–CN):** Tên thứ + Số ngày + Chấm tròn số lượng việc (`• 3`).
- **Trạng thái:** Ngày thường (nền `Surface`), Ngày hôm nay (viền `Primary` 2dp), Ngày đang chọn (nền đặc `Primary`, chữ trắng).

### 3.4. Nút Hành Động Nổi (FAB `+`)
- Kích thước `56x56dp`, bo tròn `RadiusFull`, nền `Primary`, icon `+` trắng 24dp, đổ bóng `Elevation=6`.
- Cố định góc dưới bên phải, cách lề phải 20dp và cách đỉnh TabBar 20dp.

### 3.5. Bottom Sheets & Modals
- Thanh kéo (Drag Handle) trên cùng, bo góc trên `24dp`, nền `Surface`.
- **Chọn trạng thái:** 3 thẻ độc lập (Chưa hoàn thành, Đang thực hiện, Đã hoàn thành).
- **Chọn ngày đến hạn:** Khung ngày chọn + Lưới lịch tháng + Phím tắt "Hôm nay" + Nút Hủy/Xác nhận.
- **Chọn giờ đến hạn:** Số giờ lớn nổi bật + Presets nhanh (`09:00`, `12:00`, `14:00`, `17:00`) + Nút Hủy/Xác nhận.
- **Sửa nhanh công việc:** Ô nhập Tiêu đề, Mô tả, Ghi chú kèm bộ đếm ký tự + Nút Hủy/Lưu.

### 3.6. Floating Snackbar
- Nổi phía trên TabBar `16dp`, bo góc `RadiusMd` (12dp), viền `BorderColor` 1dp.
- Đầy đủ 4 loại trạng thái: `Success`, `Danger`, `Warning`, `Undo` (kèm nút Hoàn tác và nút đóng `✕`).
- Tự động co giãn theo văn bản (`LineBreakMode="WordWrap"`), tự ẩn sau 4 giây.

---

## 4. Đặc Tả Trạng Thái Giao Diện (States & Feedback)

### 4.1. Skeleton Shimmer Loading
- Hiển thị 3–4 thẻ khung xương mô phỏng chính xác cấu trúc Task Card trên nền `SurfaceVariant`.
- Vệt sáng chu kỳ `1200ms`. Hỗ trợ **Reduce Motion** (tự động chuyển sang xám tĩnh nếu người dùng kích hoạt giảm chuyển động trên thiết bị).

### 4.2. Empty State (Chuẩn hóa qua `EmptyStateView`)
- Khối icon tròn kích thước `88x88dp`, bo góc `44dp`, nền `PrimaryLight`, icon cỡ `42dp` màu `Primary`.
- Bố cục 2 tầng: Tiêu đề in đậm (`FontSizeTitle`) + Phụ đề giải thích (`FontSizeBody`, `TextSecondary`, wrap tối đa 340dp).
- Không nhồi nhét nút CTA thừa thãi, tận dụng nút FAB (+) luôn sẵn sàng trên màn hình.

### 4.3. Not Found State
- Icon `FluentIcons.DocumentError` (64dp) màu `Danger` + Tiêu đề báo lỗi + Nút CTA "Quay lại danh sách".

---

## 5. Quy Chuẩn Responsive & Tương Thích Thiết Bị

| Nhóm thiết bị | Kích thước màn hình | Quy chuẩn thích ứng |
|---|---|---|
| **Siêu hẹp** | 320dp – 360dp | Bố cục 2 tầng triệt để, padding lề giảm còn `12–16dp`, Week Strip co giãn đều tỷ lệ 1:1, tự xuống dòng chống tràn. |
| **Tiêu chuẩn** | 390dp – 412dp | Padding lề chuẩn `20dp`, hiển thị đầy đủ icon và badge thời gian, khoảng cách Section `24dp`. |
| **Màn hình lớn / Tablet** | ≥ 600dp | Giới hạn độ rộng tối đa (`MaxWidthRequest="680"`), căn giữa màn hình để tránh giãn rộng các phần tử nhập liệu và card. |
