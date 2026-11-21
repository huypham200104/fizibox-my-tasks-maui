# HỆ THỐNG THIẾT KẾ ỨNG DỤNG (DESIGN SYSTEM SPECIFICATION)

> **Dự án:** Việc của tôi (MyTasksApp) — .NET MAUI Mobile App  
> **Phiên bản:** 2.0 (Cập nhật chuẩn theo bộ thiết kế thực tế tại `ui_design/`)  
> **Đơn vị đánh giá:** Fizibox  
> **Nguyên tắc cốt lõi:** 100% Token-Driven, WCAG AA Contrast, Ergonomic Thumb-Zone, Zero Raw Magic Values.

---

## 1. Triết Lý Thiết Kế & Nguyên Tắc Cốt Lõi (Design Principles)

1. **Nhất quán & Tập trung (100% Token-Driven):**  
   Mọi màu sắc, khoảng cách, bo góc và cỡ chữ đều được khai báo qua Design Tokens trong `ResourceDictionary`. Tuyệt đối **không hardcode mã màu hex** hoặc giá trị khoảng cách tùy tiện trong XAML.
2. **Công thái học di động (Mobile Thumb-Zone Ergonomics):**  
   Các thao tác thường dùng (TabBar 4 tabs, nút FAB `+`, chuyển trạng thái, chọn ngày/giờ bằng Bottom Sheet, nút CTA hoàn thành) đều nằm trong vùng ngón tay cái dễ tiếp cận (2/3 phía dưới màn hình). Vùng chạm tối thiểu đạt chuẩn **44x44dp**.
3. **Phân cấp thị giác rõ ràng (Visual Hierarchy):**  
   Mỗi màn hình chỉ có **tối đa 1 nút CTA chính (Solid Primary)**. Các hành động phụ dùng dạng Ghost, Outline hoặc Text Button.
4. **Dark Mode hoàn chỉnh (Fidelity Dark Theme):**  
   Không chỉ đổi màu nền đen mà phải điều chỉnh đồng bộ độ tương phản bề mặt (Surface elevation), màu chữ, màu viền và màu trạng thái đạt chuẩn tương phản cao **WCAG AA (≥ 4.5:1)**.
5. **Responsive Cỡ chữ (Elastic Typography):**  
   Hỗ trợ 3 mức cỡ chữ (Nhỏ / Thường / Lớn) áp dụng ngay lập tức mà không cắt bớt chữ, vỡ khung hay che khuất các nút thao tác.
6. **Bộ Icon đồng bộ (Single-Family Iconography):**  
   Sử dụng thư viện **Microsoft Fluent Icons** đồng bộ phong cách, thanh lịch, chuyên nghiệp, hỗ trợ tối ưu cho ứng dụng quản lý công việc.

---

## 2. Hệ Thống Design Tokens

### 2.1. Bảng Màu (Color Tokens & Semantic Mapping)

Hệ màu chủ đạo dựa trên sắc thái **Modern Indigo / Slate** kết hợp các màu chức năng tiêu chuẩn:

| Token Key | Tên ngữ nghĩa | Light Mode (HEX) | Dark Mode (HEX) | Mục đích sử dụng |
|---|---|---|---|---|
| `Primary` | Màu thương hiệu chính | `#4F46E5` (Indigo-600) | `#6366F1` (Indigo-500) | Nút CTA chính, highlight ngày chọn, nút FAB (+), tab active |
| `PrimaryVariant`| Màu nhấn bổ trợ | `#4338CA` (Indigo-700) | `#818CF8` (Indigo-400) | Trạng thái hover/pressed, badge nổi bật |
| `PrimaryLight` | Nền thương hiệu nhạt | `#EEF2FF` (Indigo-50) | `#1E1B4B` (Indigo-950) | Nền thẻ tiến độ, nền chip active, viền ngày hôm nay |
| `Background` | Nền ứng dụng chính | `#F8FAFC` (Slate-50) | `#0F172A` (Slate-900) | Nền toàn bộ trang (`ContentPage.BackgroundColor`) |
| `Surface` | Nền thẻ & thành phần | `#FFFFFF` (White) | `#1E293B` (Slate-800) | Nền Task Card, Week Strip Item, Dialog Card, Bottom Sheet |
| `SurfaceVariant`| Nền phân cách/phụ | `#F1F5F9` (Slate-100) | `#334155` (Slate-700) | Nền Skeleton Shimmer, khung nhập liệu, chip lọc unselected |
| `BorderColor` | Màu đường viền | `#E2E8F0` (Slate-200) | `#334155` (Slate-700) | Viền thẻ, đường kẻ phân cách section, viền ô nhập liệu |
| `TextPrimary` | Chữ chính | `#0F172A` (Slate-900) | `#F8FAFC` (Slate-50) | Tiêu đề công việc, heading, nhãn quan trọng (WCAG ≥ 7:1) |
| `TextSecondary`| Chữ phụ | `#475569` (Slate-600) | `#94A3B8` (Slate-400) | Tên dự án, mô tả, nhãn ngày tháng (WCAG ≥ 4.5:1) |
| `TextMuted` | Chữ mờ/chú thích | `#94A3B8` (Slate-400) | `#64748B` (Slate-500) | Placeholder, chữ gạch ngang việc đã xong, bộ đếm ký tự |
| `TextOnPrimary`| Chữ trên nền Primary | `#FFFFFF` (White) | `#FFFFFF` (White) | Chữ trên nút CTA chính, chữ ngày đang chọn trong Week Strip |
| `Danger` | Cảnh báo quá hạn / Lỗi | `#DC2626` (Red-600) | `#EF4444` (Red-500) | Nhãn "Quá hạn", badge hạn chót đỏ, icon Snackbar lỗi |
| `DangerSurface`| Nền cảnh báo quá hạn | `#FEF2F2` (Red-50) | `#450A0A` (Red-950) | Nền chip quá hạn, nền icon báo lỗi |
| `Warning` | Đang thực hiện / Hạn hôm nay| `#D97706` (Amber-600) | `#F59E0B` (Amber-500) | Chấm "Đang thực hiện", badge "Hôm nay", Snackbar cảnh báo |
| `WarningSurface`| Nền hạn hôm nay | `#FFFBEB` (Amber-50) | `#451A03` (Amber-950) | Nền chip hạn hôm nay, nền ghi chú |
| `Success` | Hoàn thành / Đã xong | `#16A34A` (Green-600) | `#22C55E` (Green-500) | Nhãn "Đã xong", icon tích xanh, Snackbar thành công |
| `SuccessSurface`| Nền hoàn thành nhẹ | `#F0FDF4` (Green-50) | `#052E16` (Green-950) | Nền thẻ hoàn thành, nền icon tích xanh |
| `SnackbarBg` | Nền thanh thông báo | `#1E293B` (Slate-800) | `#020617` (Slate-950) | Nền thanh Floating Snackbar nổi |
| `ProgressTrack`| Nền thanh tiến độ | `#E0E7FF` (Indigo-100) | `#312E81` (Indigo-900) | Nền rãnh thanh tiến độ (Linear Progress Bar track) |
| `ProgressFill` | Vệt thanh tiến độ | `#4F46E5` (Indigo-600) | `#6366F1` (Indigo-500) | Vệt màu hiển thị % hoàn thành |

---

### 2.2. Hệ Thống Khoảng Cách (Spacing Grid — Bội số 4pt / 8pt)

| Token Key | Giá trị | Ngữ cảnh sử dụng |
|---|---|---|
| `SpacingXs` | `4dp` | Khoảng cách giữa icon và text nhỏ, padding bên trong badge/chip |
| `SpacingSm` | `8dp` | Khoảng cách giữa các chip lọc, khoảng cách giữa tiêu đề và phụ đề |
| `SpacingMd` | `12dp` | Padding bên trong các thẻ nhỏ, khoảng cách giữa các phần tử form |
| `SpacingBase` | `16dp` | Padding chuẩn của Task Card, khoảng cách giữa các card trong danh sách |
| `SpacingLg` | `20dp` | Padding lề chuẩn của toàn trang (`ScreenPadding = 20dp`) |
| `SpacingXl` | `24dp` | Khoảng cách giữa các Section (Quá hạn vs Hôm nay vs Xong) |
| `SpacingXxl` | `32dp` | Khoảng trống trên cùng trước tiêu đề lớn hoặc vùng Empty state |

---

### 2.3. Hệ Thống Kiểu Chữ & Responsive Font Scaling (Typography Tokens)

Ứng dụng hỗ trợ 3 cấp độ kích thước với khả năng co giãn chữ tự nhiên:

| Token Key | Cỡ Nhỏ (Small) | Cỡ Thường (Medium - Mặc định) | Cỡ Lớn (Large) | Font Weight | Line Height |
|---|---|---|---|---|---|
| `FontSizeDisplay` | `22sp` | `26sp` | `30sp` | Bold (700) | `1.2` |
| `FontSizeTitle` | `17sp` | `20sp` | `24sp` | SemiBold (600) | `1.3` |
| `FontSizeSubtitle`| `14sp` | `16sp` | `19sp` | Medium (500) | `1.4` |
| `FontSizeBody` | `13sp` | `15sp` | `18sp` | Regular (400) | `1.5` |
| `FontSizeCaption` | `11sp` | `13sp` | `15sp` | Regular (400) | `1.4` |
| `FontSizeBadge` | `10sp` | `12sp` | `13sp` | SemiBold (600) | `1.2` |

> **Nguyên tắc an toàn cỡ chữ Lớn:**
> - Tất cả các `Label` hiển thị nội dung công việc đều đặt `LineBreakMode="WordWrap"`.
> - Không set cứng `HeightRequest` cho các dòng chứa text; sử dụng `MinimumHeightRequest="44"` để đảm bảo chiều cao tự co giãn theo cỡ chữ mà không làm mất nội dung.

---

### 2.4. Bo Góc (Corner Radius) & Đổ Bóng (Elevation)

| Token Key | Giá trị | Ứng dụng |
|---|---|---|
| `RadiusSm` | `6dp` | Badge, chip trạng thái, tag dự án |
| `RadiusMd` | `12dp` | Task Card, Week Day Item, nút bấm thông thường, ô nhập liệu |
| `RadiusLg` | `16dp` | Thẻ tiến độ, Dialog Card, Khung chi tiết, Bottom Sheet Card |
| `RadiusFull` | `999dp` | Checkbox tròn, Nút FAB (+), Pill Badge đếm số, Chip lọc |

* **Đổ bóng (Elevation / Shadow):**
  - **Light Mode:** `Shadow Offset="0,2" Radius="8" Opacity="0.06"` (nhẹ nhàng, thanh lịch).
  - **Dark Mode:** `Shadow Opacity="0"`, thay vào đó sử dụng đường viền `BorderColor` tinh tế (`1dp`) để phân định khối.

---

## 3. Quy Chuẩn Thành Phần Giao Diện (Components Spec)

### 3.1. Thẻ Công Việc (Task Card Item)
*(Tham chiếu: `01.home_screen.png`, `02.week_screen.png`, `03.history_screen.png`)*

* **Kích thước & Vùng chạm:**
  - Vùng chạm tối thiểu `44x44dp` cho Checkbox tròn.
  - Padding nội bộ: `16dp`. Bo góc: `12dp`. Nền: `Surface`. Viền: `BorderColor`.
* **Cấu trúc bên trong (Horizontal Layout):**
  - **Cột trái (Checkbox tròn):**
    - `Chưa hoàn thành`: Vòng tròn rỗng viền `BorderColor` (khi nhấn có ripple).
    - `Đang thực hiện`: Vòng tròn màu cam kèm chấm cam tâm (`Warning`).
    - `Đã hoàn thành`: Nền `Success` đặc kèm icon check trắng.
  - **Cột giữa (Thông tin):**
    - Dòng 1: Tiêu đề công việc (`FontSizeTitle`). Nếu đã xong: chữ màu `TextMuted` kèm gạch ngang `TextDecorations="Strikethrough"`.
    - Dòng 2: Tag dự án (`SurfaceVariant` chip có icon Thư mục `Folder`) + Badge hạn chót/thời gian:
      - Quá hạn: Nền `DangerSurface`, chữ `Danger`, icon `Clock` (ví dụ: *"Quá hạn 1 ngày"*).
      - Hôm nay: Nền `WarningSurface`, chữ `Warning`, icon `Clock` (ví dụ: *"09:30 hôm nay"*).
      - Đã xong: Icon `Clock` xám + thời gian hoàn thành (ví dụ: *"10:30"*).
  - **Cột phải (Điều hướng):** Icon mũi tên `ChevronRight` màu `TextMuted`.
* **Hành vi Vuốt (SwipeView):**
  - **Vuốt từ phải sang trái (Right-to-Left):** Nút xanh lá `Success` có icon Check để hoàn thành nhanh.
  - **Vuốt từ trái sang phải (Left-to-Right):** Nút đỏ `Danger` có icon Trash để xóa nhanh công việc (kích hoạt Undo Snackbar).

---

### 3.2. Thẻ Tiến Độ Ngang (Linear Progress Card)
*(Tham chiếu: `01.home_screen.png`, `02.week_screen.png`)*

* **Vị trí:** Nằm ngay dưới tiêu đề tại màn Hôm nay và màn Tuần này.
* **Cấu trúc:**
  - Thẻ bo góc `RadiusLg` (16dp), nền `PrimaryLight` (Indigo nhạt).
  - Hàng trên:
    - Trái: Icon hộp kiểm vuông bo góc màu `Primary` với dấu tích trắng.
    - Giữa: Tiêu đề *"Tiến độ hôm nay"* (hoặc Thứ/Ngày ở màn Tuần).
    - Dưới tiêu đề: Chỉ số phần trăm nổi bật (ví dụ: **65%**) và tỷ số `5/8 công việc hoàn thành`.
  - Hàng dưới: Thanh tiến độ ngang (`ProgressBar`):
    - Màu rãnh nền: `ProgressTrack` (Indigo nhạt).
    - Vệt tiến độ: `ProgressFill` (Indigo-600 / Solid Primary). Chiều cao thanh: `6dp`, bo tròn 2 đầu.

---

### 3.3. Dải Lịch 7 Ngày Ngang (Week Strip Component)
*(Tham chiếu: `02.week_screen.png`)*

* **Thanh điều hướng tuần (Week Navigation Bar):**
  - Nút `<` và `>` (bo góc `RadiusMd`, nền `Surface`, viền `BorderColor`).
  - Ở giữa: Icon lịch `Calendar` + nhãn dải ngày *"16 – 22 Tháng 11, 2026"*.
* **Dải 7 cột ngày (Thứ 2 đến Chủ Nhật):**
  - Mỗi cột gồm:
    - Tên thứ viết tắt (`T2`, `T3`, `T4`, `T5`, `T6`, `T7`, `CN`).
    - Ngày trong tháng (`16`, `17`, `18`, `19`, `20`, `21`, `22`).
    - Dấu chấm tròn kèm số lượng việc: `• 4` (nếu 0 việc: chữ mờ `• 0`).
* **Trạng thái hiển thị cột:**
  - **Ngày thường:** Nền `Surface`, chữ `TextPrimary`.
  - **Ngày hôm nay:** Viền màu `Primary` nổi bật (viền `2dp`).
  - **Ngày đang chọn (Active):** Nền đặc màu `Primary`, chữ trắng `TextOnPrimary`, dấu chấm màu trắng.

---

### 3.4. Nút Hành Động Nổi (Floating Action Button — FAB `+`)
*(Tham chiếu: `01.home_screen.png`, `02.week_screen.png`)*

* **Kích thước:** `56x56dp`, bo góc tròn tuyệt đối `RadiusFull` (999dp).
* **Màu sắc:** Nền `Primary` (Indigo), icon `+` màu trắng (cỡ 24dp). Đổ bóng nổi (`Elevation = 6`).
* **Vị trí:** Cố định ở góc dưới bên phải màn hình, cách lề phải `20dp` và cách đỉnh TabBar `20dp`.
* **Hành vi:** Khi chạm vào, mở modal tạo nhanh công việc mới.

---

### 3.5. Hệ Thống Bottom Sheets & Modals
*(Tham chiếu: `bottom_sheet_change_status.png`, `bottom_sheet_date_picker.png`, `bottom_sheet_time_picker.png`, `popup_edit_text.png`, `detail_task.png`)*

Mọi Bottom Sheet đều có thanh kéo (Drag Handle) ở đỉnh, bo góc trên `24dp`, nền `Surface`.

1. **Bottom Sheet "Chọn trạng thái" (`bottom_sheet_change_status.png`):**
   - 3 lựa chọn thẻ độc lập:
     - *Chưa hoàn thành:* Icon tròn rỗng, chữ `TextPrimary`, mô tả *"Công việc chưa được thực hiện"*.
     - *Đang thực hiện:* Icon chấm tròn cam, chữ `TextPrimary`, mô tả *"Công việc đang được xử lý"*.
     - *Đã hoàn thành:* Icon tròn xanh tích trắng, chữ `TextPrimary`, mô tả *"Công việc đã hoàn tất"*.
   - Mục đang chọn có viền `Primary` và dấu tích tím/xanh bên phải.
2. **Bottom Sheet "Chọn ngày đến hạn" (`bottom_sheet_date_picker.png`):**
   - Tiêu đề *"Chọn ngày đến hạn"*.
   - Khung hiển thị ngày chọn: Icon lịch + *"Thứ Sáu, 20 tháng 11, 2026"*.
   - Lưới lịch tháng: Điều hướng tháng `< Tháng 11, 2026 >`, 7 cột thứ (`T2`–`CN`), ngày chọn là hình tròn xanh `Primary`.
   - Phím tắt nhanh: Nút *"Hôm nay"*.
   - 2 nút chân trang: Nút *"Hủy"* (Outlined) và nút *"Xác nhận"* (Solid Primary).
3. **Bottom Sheet "Chọn giờ đến hạn" (`bottom_sheet_time_picker.png`):**
   - Tiêu đề *"Chọn giờ đến hạn"*.
   - Khung số giờ lớn: Chữ **14:00** nổi bật màu `Primary`.
   - Mặt đồng hồ số / Dải chọn giờ trực quan.
   - Các nút chọn giờ nhanh (Quick Presets): `09:00`, `12:00`, `14:00`, `17:00`.
   - 2 nút chân trang: *"Hủy"* và *"Xác nhận"*.
4. **Modal "Chỉnh sửa công việc" (`popup_edit_text.png`):**
   - Nút đóng `✕` ở góc trên bên phải.
   - Trường 1: Tiêu đề công việc — Ô nhập văn bản kèm bộ đếm ký tự `21/100`.
   - Trường 2: Mô tả — Ô nhập nhiều dòng kèm bộ đếm ký tự `58/1000`.
   - Trường 3: Ghi chú — Ô nhập nhiều dòng kèm bộ đếm ký tự `37/500`.
   - 2 nút chân trang: *"Hủy"* và *"Lưu thay đổi"* (Solid Primary).
5. **Hộp thoại xác nhận mở lại việc (`detail_task.png`):**
   - Tiêu đề: *"Mở lại công việc?"*
   - Nội dung: *"Bạn có chắc chắn muốn mở lại công việc này không? Trạng thái sẽ được chuyển về Chưa hoàn thành."*
   - Nút *"Hủy"* (Ghost/SurfaceVariant) và nút *"Xác nhận"* (Solid Primary).

---

### 3.6. Hệ Thống Floating SnackBar
*(Tham chiếu: `snack_bar.png`)*

Nổi phía trên TabBar khoảng `16dp`, lề 2 bên `16dp`, bo góc `RadiusMd` (12dp), nền tối `SnackbarBg` (`#1E293B`).

* **Cấu trúc 4 loại:**
  1. `Success`: Icon vòng tròn xanh check trắng + Thông điệp *"Đã hoàn thành công việc"* + Nút `✕`.
  2. `Danger`: Icon vòng tròn đỏ chấm than trắng + Thông điệp *"Không thể cập nhật công việc"* + Nút `✕`.
  3. `Warning`: Icon tam giác vàng chấm than + Thông điệp *"Bạn có thay đổi chưa được lưu"* + Nút `✕`.
  4. `Undo`: Icon thùng rác trắng + Thông điệp *"Đã xóa công việc"* + Nút bấm tím nhạt *"Hoàn tác"* + Nút `✕`.

---

## 4. Đặc Tả Trạng Thái Giao Diện (States & Feedback)

### 4.1. Skeleton Shimmer Loading (Màn 1 & Màn 2)
* **Quy chuẩn:** Không sử dụng Spinner tròn quay ở giữa màn hình làm người dùng thụ động chờ đợi.
* **Mô phỏng:** Hiển thị 3–4 thẻ khung xương (Skeleton Cards) với đúng kích thước, bo góc `12dp`, padding `16dp` giống hệt Task Card thật.
* **Hiệu ứng:** Vệt sáng chạy mượt mà chu kỳ `1200ms` từ trái qua phải trên nền `SurfaceVariant`.
* **Hỗ trợ Reduce Motion:** Nếu hệ điều hành bật chế độ giảm chuyển động, vệt sáng sẽ dừng lại, giữ màu xám tĩnh để tránh khó chịu thị giác.

### 4.2. Empty State (Giao diện trạng thái trống - Chuẩn hóa đồng bộ)
* **Quy chuẩn hiển thị chung (100% Token-Driven qua `EmptyStateView`):**
  - **Khối Icon tròn thương hiệu:** Vòng tròn kích thước `88x88dp`, bo góc `44dp` (`Border StrokeThickness="0"`), nền `PrimaryLight` (`#EEF2FF` Light / `#1E1B4B` Dark).
  - **Màu sắc Icon:** Sử dụng màu thương hiệu `Primary` (`#4F46E5` Light / `#6366F1` Dark), kích thước `42dp` căn giữa hoàn hảo.
  - **Bố cục 2 tầng thông điệp:** Dòng trên là Tiêu đề in đậm (`FontSizeTitle`, `InterSemiBold`, `TextPrimary`); Dòng dưới là Phụ đề giải thích (`FontSizeBody`, `InterRegular`, `TextSecondary`, `LineBreakMode="WordWrap"`, `MaximumWidthRequest="340"`).
  - **Tinh gọn không nút CTA:** Giao diện tập trung tối đa vào thông điệp, người dùng tạo mới việc qua nút FAB (+) luôn nổi sẵn trên màn hình.

* **Nội dung & Icon chi tiết từng màn hình:**
  - **Màn hình Hôm nay (`TodayPage`):**
    - Icon minh họa: `FluentIcons.CheckmarkCircle` (42dp, màu `Primary` trên nền `PrimaryLight`).
    - Tiêu đề: *"Hôm nay bạn không có việc cần làm!"* (EN: *"You have no tasks to do today!"*).
    - Phụ đề: *"Hãy nghỉ ngơi hoặc lên kế hoạch cho những ngày tiếp theo."* (EN: *"Take a break or plan ahead for the upcoming days."*).
  - **Màn hình Tuần này (`WeekPage` - Ngày không có việc):**
    - Icon minh họa: `FluentIcons.CalendarEmpty` (42dp, màu `Primary` trên nền `PrimaryLight`).
    - Tiêu đề: *"Không có công việc"* (EN: *"No tasks scheduled"*).
    - Phụ đề: *"Ngày này chưa có công việc nào được lên lịch."* (EN: *"There are no tasks scheduled for this day."*).
  - **Màn hình Đã xong (`HistoryPage`):**
    - Icon minh họa: `FluentIcons.ClipboardCheckmark` (42dp, màu `Primary` trên nền `PrimaryLight`).
    - Tiêu đề: *"Chưa có công việc hoàn thành"* (EN: *"No completed tasks yet"*).
    - Phụ đề: *"Những công việc bạn hoàn thành sẽ xuất hiện tại đây."* (EN: *"Tasks that you complete will show up here."*).
  - **Màn hình Mô phỏng (`MockStatePreviewPage`):**
    - Icon minh họa: `FluentIcons.CalendarEmpty` (42dp, màu `Primary` trên nền `PrimaryLight`).
    - Tiêu đề: *"Danh sách công việc trống"*.
    - Phụ đề: *"Hôm nay bạn không có công việc nào cần làm. Hãy tận hưởng thời gian hoặc tạo công việc mới!"*.
    - Huy hiệu ghi chú Reviewer bên dưới: *"💡 Trạng thái này xuất hiện khi chưa có việc hoặc đã hoàn tất 100%"*.

### 4.3. Not Found State (Màn hình Chi tiết)
* **Biểu hiện:** Hiển thị khi nhận ID không hợp lệ hoặc kích hoạt từ Reviewer Tools.
* **Giao diện:** Icon cảnh báo `FluentIcons.DocumentError` (cỡ 64dp) màu `Danger` + Tiêu đề *"Không tìm thấy công việc"* + Mô tả *"Công việc này không tồn tại hoặc đã bị xóa"* + Nút bấm CTA *"Quay lại danh sách"*.

---

## 5. Bố Cục Chi Tiết Các Màn Hình (Screen Layouts)

### 5.1. Màn hình 1 — Hôm nay (`TodayPage`)
```
┌──────────────────────────────────────────────┐
│ Thứ Sáu, 20 tháng 11                         │
│ Hôm nay                                      │
│ Cố lên! Bạn đã làm rất tốt rồi! 💙           │
├──────────────────────────────────────────────┤
│ ┌─ [Progress Card] ────────────────────────┐ │
│ │ [✓] Tiến độ hôm nay                      │ │
│ │     65% | 5/8 công việc hoàn thành       │ │
│ │     ═════════════════════─────── (Bar)   │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ● QUÁ HẠN ────────────────────────────────── │
│ ┌──────────────────────────────────────────┐ │
│ │ ○  Hoàn thiện báo cáo tuần   [Quá hạn 1d] >│
│ │    📁 Dự án Mobile App                   │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ● HÔM NAY ────────────────────────────────── │
│ ┌──────────────────────────────────────────┐ │
│ │ ○  Thiết kế giao diện Home... [09:30 hn] >│
│ │    📁 MyTasks App                        │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ● XONG ───────────────────────────────────── │
│ ┌──────────────────────────────────────────┐ │
│ │ 🗹  Kiểm tra tiến độ dự án (Strikethrough) │
│ └──────────────────────────────────────────┘ │
│                                        [ + ] │  <-- FAB
├──────────────────────────────────────────────┤
│ [🏠 Hôm nay]  [📅 Tuần này]  [🗹 Đã xong]  [⚙ Cài đặt] │
└──────────────────────────────────────────────┘
```

### 5.2. Màn hình 2 — Tuần này (`WeekPage`)
```
┌──────────────────────────────────────────────┐
│ Tuần này                                     │
│ Theo dõi kế hoạch và tiến độ của bạn         │
├──────────────────────────────────────────────┤
│ [<]       📅 16 – 22 Tháng 11, 2026       [>]│
│ ┌───┐ ┌───┐ ┌───┐ ┌───┐ ┌───┐ ┌───┐ ┌───┐    │
│ │T2 │ │T3 │ │T4 │ │T5 │ │T6*│ │T7 │ │CN │    │
│ │16 │ │17 │ │18 │ │19 │ │20 │ │21 │ │22 │    │
│ │• 1│ │• 0│ │• 2│ │• 1│ │• 4│ │• 0│ │• 1│    │
│ └───┘ └───┘ └───┘ └───┘ └───┘ └───┘ └───┘    │
├──────────────────────────────────────────────┤
│ ┌─ [Day Summary Card] ─────────────────────┐ │
│ │ 📅 Thứ Sáu, 20 tháng 11                  │ │
│ │    4 công việc • 2/4 hoàn thành          │ │
│ │    ═══════════─────────── 50%            │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ┌──────────────────────────────────────────┐ │
│ │ 🗹 Hoàn thiện thiết kế...     (09:30)   >│ │
│ └──────────────────────────────────────────┘ │
│ ┌──────────────────────────────────────────┐ │
│ │ ○  Review Pull Request        (14:00)   >│ │
│ └──────────────────────────────────────────┘ │
│                                        [ + ] │  <-- FAB
├──────────────────────────────────────────────┤
│ [🏠 Hôm nay]  [📅 Tuần này]  [🗹 Đã xong]  [⚙ Cài đặt] │
└──────────────────────────────────────────────┘
```

### 5.3. Màn hình 3 — Đã xong / Lịch sử (`HistoryPage`)
```
┌──────────────────────────────────────────────┐
│ Lịch sử                                      │
│ Những công việc bạn đã hoàn thành            │
├──────────────────────────────────────────────┤
│ ┌─ [Achievement Summary Card] ─────────────┐ │
│ │ 🗹  24 Công việc đã hoàn thành           │ │
│ │     Trong tháng 11, 2026                 │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ 🔍 [Tìm kiếm công việc đã hoàn thành...]     │
│ [ Tất cả ] [ Hôm nay ] [ Tuần này ] [Tháng này] │
│                                              │
│ Hôm nay / 20 tháng 11, 2026                  │
│ ┌──────────────────────────────────────────┐ │
│ │ 🗹 Kiểm tra tiến độ dự án      10:30    >│ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ Hôm qua / 19 tháng 11, 2026                  │
│ ┌──────────────────────────────────────────┐ │
│ │ 🗹 Hoàn thiện giao diện login   16:45    >│ │
│ └──────────────────────────────────────────┘ │
├──────────────────────────────────────────────┤
│ [🏠 Hôm nay]  [📅 Tuần này]  [🗹 Đã xong]  [⚙ Cài đặt] │
└──────────────────────────────────────────────┘
```

### 5.4. Màn hình 4 — Cài đặt (`SettingsPage`)
```
┌──────────────────────────────────────────────┐
│ Cài đặt                                      │
│ Cá nhân hóa trải nghiệm của bạn              │
├──────────────────────────────────────────────┤
│ 🎨 Giao diện                                 │
│    ┌──────────────┐   ┌──────────────┐       │
│    │ 🗔 Sáng   (✓)│   │ 🗔 Tối        │       │
│    └──────────────┘   └──────────────┘       │
│    Cỡ chữ:                                   │
│    [   Nhỏ   ] [  Thường (✓) ] [    Lớn   ]  │
│                                              │
│ 🌐 Ngôn ngữ                                  │
│    [ 🇻🇳 Tiếng Việt (✓) ]   [ 🇬🇧 English ]    │
│                                              │
│ 👁 Xem trước (Live Preview)                  │
│    ┌───────────────────────────────────────┐ │
│    │ ○ Review Pull Request                 │ │
│    │   📁 MyTasks App                      │ │
│    └───────────────────────────────────────┘ │
│                                              │
│ 🔧 Công cụ kiểm thử (Reviewer Tools)         │
│    • Trạng thái đang tải       [ Xem thử ]   │
│    • Danh sách trống           [ Xem thử ]   │
│    • Không tìm thấy việc       [ Xem thử ]   │
│    • Đặt lại dữ liệu mẫu       [ Đặt lại ]   │
│                                              │
│ ℹ Thông tin ứng dụng: MyTasks v1.0.0         │
├──────────────────────────────────────────────┤
│ [🏠 Hôm nay]  [📅 Tuần này]  [🗹 Đã xong]  [⚙ Cài đặt] │
└──────────────────────────────────────────────┘
```

### 5.5. Màn hình Chi tiết công việc (`TaskDetailPage`)
```
┌──────────────────────────────────────────────┐
│ < Chi tiết công việc                       ⋮ │
├──────────────────────────────────────────────┤
│ Review Pull Request                          │
│ [📁 Dự án Backend]        [○ Chưa hoàn thành]│
│                                              │
│ 📄 Mô tả công việc                           │
│ ┌──────────────────────────────────────────┐ │
│ │ Kiểm tra và đánh giá các thay đổi...     │ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ℹ Thông tin công việc                        │
│ ┌──────────────────────────────────────────┐ │
│ │ 📅 Hạn hoàn thành: 20/11/2026           >│ │
│ │ 🕒 Thời gian: 14:00                     >│ │
│ │ 📁 Dự án: Dự án Backend                  │ │
│ │ 🗹 Trạng thái: Chưa hoàn thành          >│ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ 📝 Ghi chú                                   │
│ ┌──────────────────────────────────────────┐ │
│ │ 🟨 Cần kiểm tra kỹ phần authentication...│ │
│ └──────────────────────────────────────────┘ │
│                                              │
│ ┌──────────────────────────────────────────┐ │
│ │       [✓] Hoàn thành công việc           │ │  (Nếu chưa xong)
│ └──────────────────────────────────────────┘ │
│                     - HOẶC -                 │
│ ┌──────────────────────────────────────────┐ │
│ │       [↻] Mở lại công việc               │ │  (Nếu đã xong)
│ └──────────────────────────────────────────┘ │
└──────────────────────────────────────────────┘
```

---

## 6. Cơ Chế Ánh Xạ Kỹ Thuật Vào .NET MAUI

1. **Quản lý DynamicResource:**  
   Tất cả các màu sắc, font scale và bo góc được đăng ký vào `Application.Current.Resources`.  
   Khi người dùng đổi Theme (Sáng/Tối) hoặc đổi Cỡ chữ (Nhỏ/Thường/Lớn), `SettingsService` cập nhật các giá trị tương ứng trong `Application.Current.Resources`. Mọi View dùng `{DynamicResource ...}` sẽ tự động render lại ngay lập tức mà không cần khởi động lại ứng dụng.
2. **Cơ chế Song ngữ VI/EN tức thì:**  
   `LocalizationService` cung cấp đối tượng Bindable từ điển. Toàn bộ chuỗi văn bản giao diện hiển thị qua `{Binding Strings[TodayTitle]}` hoặc `{DynamicResource Str_TodayTitle}`, đảm bảo nhãn đổi ngay khi switch ngôn ngữ.
3. **Thư viện Icon:**  
   Sử dụng `FluentIcons.Maui` đã được người dùng phê duyệt, nhúng trực tiếp qua XAML namespace:  
   `xmlns:fluent="clr-namespace:FluentIcons.Maui;assembly=FluentIcons.Maui"`.
4. **Shell Navigation 4 Tab:**  
   `AppShell.xaml` cấu hình thanh `TabBar` chứa 4 `ShellContent`: `TodayPage`, `WeekPage`, `HistoryPage`, `SettingsPage`.  
   `TaskDetailPage` được đăng ký qua `Routing.RegisterRoute(nameof(TaskDetailPage), typeof(TaskDetailPage))` để hỗ trợ điều hướng truyền tham số `id`.
