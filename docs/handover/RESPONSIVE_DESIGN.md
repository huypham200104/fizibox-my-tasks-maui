# BÁO CÁO GIẢI TRÌNH THIẾT KẾ RESPONSIVE
## HỆ THỐNG GIAO DIỆN ỨNG DỤNG "VIỆC CỦA TÔI" (MYTASKSAPP)

> **Dự án:** Việc của tôi (MyTasksApp) — .NET MAUI Mobile App  
> **Đơn vị đánh giá:** Fizibox (`https://fizibox.com`)  
> **Mục tiêu:** Giải thích chi tiết giải pháp responsive cho từng thành phần giao diện, nguyên nhân kỹ thuật và lợi ích trải nghiệm người dùng (UX) khi vận hành trên đa dạng kích thước màn hình di động (320dp – 600dp+) và 3 cấp độ cỡ chữ (Nhỏ / Thường / Lớn).

---

## 1. Triết Lý Cốt Lõi & Thách Thức Thiết Kế Trên Di Động

Trên nền tảng di động (.NET MAUI cross-platform), ứng dụng đối mặt với 3 thách thức hiển thị lớn:
1. **Đa dạng kích thước màn hình:** Từ các thiết bị màn hình nhỏ, siêu hẹp (**320dp – 360dp** như iPhone SE, Android phân khúc cơ bản), kích thước tiêu chuẩn (**390dp – 412dp**) đến máy tính bảng hoặc màn hình gập (**≥ 600dp**).
2. **Elastic Typography (Co giãn cỡ chữ theo người dùng):** Ứng dụng hỗ trợ 3 mức cỡ chữ (Nhỏ / Thường / Lớn). Khi chuyển sang cỡ **Lớn (Large)**, văn bản dài ra từ **30% đến 50%**. Nếu bố trí cố định hoặc nhồi nhét theo chiều ngang, chữ sẽ bị cắt cụt (`...`) hoặc đẩy các nút chức năng tràn ra ngoài mép màn hình.
3. **Công thái học ngón tay cái (Mobile Thumb-Zone & Touch Target):** Ngón tay người dùng cần diện tích tiếp xúc tối thiểu **44x44dp** để tránh thao tác trượt hoặc bấm nhầm.

Dưới đây là chi tiết cách xử lý và lý do kỹ thuật cho từng thành phần giao diện.

---

## 2. Thẻ Công Việc (Task Card Item) — Bố Cục 2 Tầng Chống Tràn Tuyệt Đối

*(Áp dụng tại: `TodayPage.xaml`, `WeekPage.xaml`, `HistoryPage.xaml`)*

### 2.1. Hiện trạng trước đây & Vấn đề
- **Cách làm cũ:** Checkbox tròn, Tiêu đề công việc, Chip Dự án và Badge thời hạn được xếp nối tiếp nhau trên cùng một dòng ngang (`HorizontalStackLayout`).
- **Hệ quả tiêu cực:** 
  - Khi tiêu đề công việc dài hơn 4 từ, hoặc khi người dùng bật cỡ chữ Lớn, tiêu đề bị cắt cụt thành dấu ba chấm (`...`).
  - Badge hạn chót (như *"Quá hạn 1 ngày"*) bị đẩy văng mất khỏi mép phải màn hình.

### 2.2. Giải pháp cải tiến: Bố cục 2 Tầng (Two-Tier Layout) bằng `Grid`
Cấu trúc XAML chuẩn hóa:
```xml
<Grid ColumnDefinitions="Auto, *" RowDefinitions="Auto, Auto" ColumnSpacing="12" RowSpacing="8">
    <!-- TẦNG 1: Dòng trên trọn vẹn cho Checkbox & Tiêu đề -->
    <Grid Grid.Row="0" Grid.Column="0" WidthRequest="36" HeightRequest="36">
        <Border WidthRequest="24" HeightRequest="24" StrokeShape="RoundRectangle 12">
            <!-- Checkbox đồ họa thanh lịch, vùng chạm mở rộng 36x36dp -->
        </Border>
    </Grid>

    <Label Grid.Row="0" Grid.Column="1"
           Text="{Binding Title}"
           FontFamily="InterSemiBold"
           FontSize="{DynamicResource FontSizeBody}"
           TextColor="{DynamicResource TextPrimary}"
           LineBreakMode="WordWrap"
           VerticalOptions="Center" />

    <!-- TẦNG 2: Dòng dưới dành riêng cho Metadata (Dự án & Thời hạn) -->
    <Grid Grid.Row="1" Grid.Column="1" ColumnDefinitions="Auto, *, Auto" VerticalOptions="Center">
        <!-- Bên trái: Chip Dự án -->
        <Border Grid.Column="0" BackgroundColor="{DynamicResource PrimaryLight}" StrokeShape="RoundRectangle 6" Padding="6,2">
            <HorizontalStackLayout Spacing="4">
                <fluent:SymbolIcon Symbol="Folder" FontSize="12" ForegroundColor="{DynamicResource Primary}" />
                <Label Text="{Binding ProjectName}" FontSize="{DynamicResource FontSizeBadge}" />
            </HorizontalStackLayout>
        </Border>

        <!-- Bên phải: Badge Quá hạn / Hôm nay / Giờ hoàn thành -->
        <Border Grid.Column="2" BackgroundColor="{DynamicResource DangerSurface}" StrokeShape="RoundRectangle 8" Padding="8,3">
            <HorizontalStackLayout Spacing="4">
                <fluent:SymbolIcon Symbol="Clock" FontSize="12" ForegroundColor="{DynamicResource Danger}" />
                <Label Text="{Binding DueBadgeText}" FontSize="{DynamicResource FontSizeBadge}" />
            </HorizontalStackLayout>
        </Border>
    </Grid>
</Grid>
```

### 2.3. Tại sao lại làm thế?
1. **Ưu tiên thông tin cốt lõi:** Tiêu đề công việc là nội dung quan trọng nhất đối với người dùng. Tầng 1 cấp trọn **100% chiều ngang còn lại (`*`)** cho Tiêu đề kết hợp `LineBreakMode="WordWrap"`, cho phép tự do xuống dòng khi cần mà không bị bất kỳ thẻ Tag hay Badge nào tranh chấp không gian.
2. **Miễn nhiễm với kích thước màn hình:** Dù hiển thị trên màn hình siêu hẹp **320dp** hay bật font chữ **Lớn**, tiêu đề vẫn luôn đọc được trọn vẹn, không bao giờ xuất hiện dấu `...` gây ức chế.

---

## 3. Thẻ Tiến Độ (Linear Progress Card) — Phân Cấp Lưới Thông Tin Động

*(Áp dụng tại: `TodayPage.xaml`, `WeekPage.xaml`)*

### 3.1. Hiện trạng trước đây & Vấn đề
- Tiêu đề *"Tiến độ hôm nay"*, con số phần trăm (`65%`) và tỷ số (*"5/8 công việc hoàn thành"*) đặt trong `HorizontalStackLayout`.
- Do tiếng Việt dài hơn tiếng Anh khoảng 20–30%, khi chuyển ngôn ngữ hoặc người dùng có nhiều công việc (ví dụ: *"12/15 công việc hoàn thành"*), các chuỗi chữ đè lên con số `%` hoặc bị tràn viền.

### 3.2. Giải pháp cải tiến: Lưới phân cấp thông tin đa tầng
```xml
<Grid ColumnDefinitions="Auto, *" ColumnSpacing="12" VerticalOptions="Center">
    <!-- Cột trái: Khối vuông icon thương hiệu 44x44dp -->
    <Border WidthRequest="44" HeightRequest="44" StrokeShape="RoundRectangle 12" BackgroundColor="{DynamicResource Primary}">
        <fluent:SymbolIcon Symbol="Checkmark" ForegroundColor="White" FontSize="22" />
    </Border>

    <!-- Cột phải: Lưới 2 hàng chống chồng chéo chữ -->
    <Grid Grid.Column="1" RowDefinitions="Auto, Auto" ColumnDefinitions="*, Auto" RowSpacing="3">
        <!-- Hàng 1: Tiêu đề bên trái, % to đậm nổi bật bên phải -->
        <Label Grid.Row="0" Grid.Column="0" Text="{Binding ProgressTodayTitle}" FontSize="{DynamicResource FontSizeCaption}" TextColor="{DynamicResource TextSecondary}" />
        <Label Grid.Row="0" Grid.Column="1" Text="{Binding ProgressPercentageText}" FontSize="{DynamicResource FontSizeTitle}" TextColor="{DynamicResource Primary}" HorizontalOptions="End" />

        <!-- Hàng 2: Chuỗi tỷ số chiếm trọn cả 2 cột -->
        <Label Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Text="{Binding ProgressRatioText}" FontSize="{DynamicResource FontSizeCaption}" TextColor="{DynamicResource TextSecondary}" />
    </Grid>
</Grid>
<!-- Thanh tiến độ dày dặn bên dưới -->
<ProgressBar Progress="{Binding ProgressRatio}" ProgressColor="{DynamicResource ProgressFill}" BackgroundColor="{DynamicResource ProgressTrack}" ScaleY="2" />
```

### 3.3. Tại sao lại làm thế?
1. **Định vị con số quan trọng nhất:** Con số phần trăm (`65%`) luôn được cố định chắc chắn ở góc trên bên phải (`HorizontalOptions="End"`), đóng vai trò mỏ neo thị giác (visual anchor).
2. **Không gian co giãn tự do:** Dòng tỷ số phía dưới được cấp toàn bộ bề ngang (`Grid.ColumnSpan="2"`), thoải mái mở rộng khi đổi sang tiếng Việt dài hoặc hiển thị số lượng lớn mà không sợ xô lệch bố cục.

---

## 4. Dải Lịch 7 Ngày Ngang (Week Strip) — Tỷ Lệ Tương Đối 7 Cột Cân Đối

*(Áp dụng tại: `WeekPage.xaml`)*

### 4.1. Hiện trạng trước đây & Vấn đề
- Nếu dùng danh sách cuộn ngang (`ScrollView Horizontal`): Người dùng bị mất góc nhìn tổng quan tuần, phải vuốt qua lại mới thấy ngày cuối tuần.
- Nếu gán cứng chiều rộng pixel (ví dụ `WidthRequest="50"`): Màn hình hẹp 320dp sẽ tràn mất cột Chủ Nhật; màn hình rộng 412dp sẽ bị hở một khoảng trống lớn bên phải.

### 4.2. Giải pháp cải tiến: Lưới chia đều 7 cột tương đối (`*`)
```xml
<Grid ColumnDefinitions="*, *, *, *, *, *, *" ColumnSpacing="5">
    <!-- CỘT 0 (T2) -> CỘT 6 (CN) -->
    <Border Grid.Column="0" BindingContext="{Binding WeekDays[0]}"
            StrokeShape="RoundRectangle 12"
            Padding="2,10"
            HorizontalOptions="Fill" VerticalOptions="Fill">
        <VerticalStackLayout Spacing="4" HorizontalOptions="Center">
            <Label Text="{Binding DayOfWeekShort}" FontSize="{DynamicResource FontSizeCaption}" />
            <Label Text="{Binding DayNumber}" FontSize="{DynamicResource FontSizeTitle}" />
            <Label Text="{Binding TaskCountText}" FontSize="{DynamicResource FontSizeBadge}" />
        </VerticalStackLayout>
    </Border>
    <!-- ... các cột còn lại tương tự ... -->
</Grid>
```

### 4.3. Tại sao lại làm thế?
1. **Tỷ lệ 1:1 hoàn hảo:** Ký hiệu sao (`*`) chia chiều ngang thực tế thành 7 phần đều nhau tuyệt đối.
2. **Khung nhìn trọn vẹn (Glanceable UI):** Trên mọi thiết bị, 7 ngày luôn hiển thị đồng thời trong một màn hình duy nhất, giúp người dùng nắm bắt lịch trình cả tuần chỉ trong 1 giây mà không cần thao tác vuốt cuộn phụ.

---

## 5. Màn Hình Chi Tiết (TaskDetailPage) — `FlexLayout Wrap` & Cố Định Đáy

*(Áp dụng tại: `TaskDetailPage.xaml`)*

### 5.1. Bố cục Header với `FlexLayout Wrap="Wrap"`
- **Cách sửa:**
  ```xml
  <FlexLayout Wrap="Wrap" AlignItems="Center">
      <!-- Tag Dự án -->
      <Border Margin="0,0,8,8" BackgroundColor="{DynamicResource PrimaryLight}" Padding="10,6" StrokeShape="RoundRectangle 10">
          <HorizontalStackLayout Spacing="6">
              <fluent:SymbolIcon Symbol="Folder" FontSize="14" ForegroundColor="{DynamicResource Primary}" />
              <Label Text="{Binding CurrentTask.ProjectName}" FontFamily="InterMedium" />
          </HorizontalStackLayout>
      </Border>
      <!-- Badge Trạng thái -->
      <Border Margin="0,0,8,8" Padding="10,6" StrokeShape="RoundRectangle 10">
          <Label Text="{Binding CurrentTask.StatusText}" FontFamily="InterSemiBold" />
      </Border>
  </FlexLayout>
  ```
- **Tại sao:** Khi người dùng chọn cỡ chữ Lớn, nếu đặt 2 Badge này trong `HorizontalStackLayout`, Badge thứ hai sẽ bị cắt ngang hoặc biến mất khỏi màn hình. `FlexLayout Wrap="Wrap"` tự động đưa Badge thứ hai xuống dòng dưới mượt mà, tự nhiên.

### 5.2. Nút Thao Tác Chính (CTA) Ghim Cố Định Đáy Màn Hình
- **Cách sửa:** Đặt nút CTA (`Hoàn thành` / `Mở lại`) nằm ngoài `ScrollView`, ghim cố định ở đáy:
  ```xml
  <Border Grid.Row="2" BackgroundColor="{DynamicResource Surface}" Padding="20,12" VerticalOptions="End">
      <Border StrokeShape="RoundRectangle 14" BackgroundColor="{DynamicResource Primary}" Padding="0,14">
          <!-- Nút CTA chính đạt chiều cao chạm 48dp+ -->
      </Border>
  </Border>
  ```
- **Tại sao (Công thái học Thumb-Zone):** Khi ghi chú hoặc mô tả công việc dài đến mức phải cuộn trang 2–3 màn hình, người dùng vẫn có thể bấm nút hoàn thành ngay bằng ngón tay cái mà không cần phải cuộn ngược trở lại đầu trang.

---

## 6. Thanh Thông Báo Nổi (Floating Snackbar) — Co Giãn Tự Động Theo Văn Bản

*(Áp dụng tại: `FloatingSnackbarView.xaml`)*

### 6.1. Cách sửa
```xml
<Grid ColumnDefinitions="Auto, *, Auto, Auto" ColumnSpacing="12" VerticalOptions="Center">
    <fluent:SymbolIcon Grid.Column="0" Symbol="CheckmarkCircle" FontSize="20" />
    <Label Grid.Column="1" Text="{Binding Message}" LineBreakMode="WordWrap" VerticalOptions="Center" />
    <Button Grid.Column="2" Text="Hoàn tác" />
    <Button Grid.Column="3" Text="✕" />
</Grid>
```
Nổi cách đáy `Margin="16,0,16,24"` (ngay phía trên thanh TabBar).

### 6.2. Tại sao lại làm thế?
1. **Tự động co giãn (Self-sizing):** Cột text thông điệp đặt là `*` cùng `LineBreakMode="WordWrap"` cho phép thanh thông báo tự nở chiều cao linh hoạt khi câu thông báo dài xuống 2 dòng, mà các nút chức năng (Hoàn tác, nút Đóng) vẫn giữ nguyên vị trí căn giữa dọc (`VerticalOptions="Center"`).
2. **Không che khuất thanh TabBar:** Khoảng cách đáy 24dp giúp thanh thông báo nổi tách biệt, không đè lên các icon điều hướng chính của ứng dụng.

---

## 7. Chuẩn Vùng Chạm Công Thái Học (Touch Target ≥ 44x44dp)

### 7.1. Cách sửa
- **Checkbox công việc:** Hình vẽ vòng tròn được thiết kế kích thước thanh lịch `24x24dp`, nhưng khung `Grid` nhận sự kiện chạm bên ngoài được đặt `WidthRequest="36"`, `HeightRequest="36"` kết hợp padding bao quanh của thẻ card để vùng chạm thực tế luôn đạt từ **`44x44dp` đến `48x48dp`**.
- **Nút FAB `+`:** Kích thước chuẩn **`56x56dp`**, bo tròn tuyệt đối, cố định ở góc dưới bên phải.
- **Các icon điều hướng (`<`, `>`, `✕`, `⋮`):** Khung bọc tối thiểu đạt **`44x44dp`**.

### 7.2. Tại sao lại làm thế?
- Theo nghiên cứu nhân trắc học của Apple (Human Interface Guidelines) và Google (Material Design), đầu ngón tay cái có diện tích tiếp xúc từ **8–10mm** (tương đương 44–48dp). Vùng chạm dưới 36dp khiến tỷ lệ chạm trượt (misclick) tăng gấp 3 lần, gây ức chế lớn cho người sử dụng.

---

## 8. Cắt Góc Khối Chuẩn Xác (Surface & Border Clipping)

### 8.1. Cách sửa
- Tất cả các thẻ và dialog đều áp dụng hình học bo góc nhất quán qua `StrokeShape="RoundRectangle <radius>"`.
- Các thành phần con có nền màu riêng luôn nằm trọn vẹn trong `Padding` an toàn của thẻ cha.

### 8.2. Tại sao lại làm thế?
- Triệt tiêu lỗi vỡ giao diện kinh điển trên **Android Skia Canvas**: Khi phần tử con có màu nền riêng bị tràn góc nhọn ra ngoài đường viền cong của thẻ cha, làm giao diện bị thô ráp và mất đi vẻ hoàn thiện cao cấp.

---

## 9. Bảng Ma Trận Tương Thích Kích Thước Màn Hình

| Nhóm thiết bị | Chiều rộng (dp) | Thách thức chính | Giải pháp kiến trúc áp dụng |
|---|---|---|---|
| **Màn hình siêu hẹp** | 320dp – 360dp | Dễ tràn chữ, vỡ hàng ngang, badge bị che khuất | Bố cục 2 tầng triệt để, dải tuần 7 cột `*`, `FlexLayout Wrap`, padding lề gọn gàng `14dp`. |
| **Màn hình tiêu chuẩn** | 390dp – 412dp | Màn hình phổ biến nhất hiện nay | Padding chuẩn `16–20dp`, hiển thị đầy đủ icon và badge thời gian, tỷ lệ vàng giữa các section. |
| **Tablet / Màn hình gập** | ≥ 600dp | Giao diện bị giãn bè ngang quá mức gây khó nhìn | Cấu trúc form có giới hạn bề rộng hợp lý, căn giữa trang, các cột lưới dãn đều tự nhiên. |

---

## 10. Lời Kết Dành Cho Buổi Review Kỹ Thuật

Toàn bộ các giải pháp Responsive trong dự án không sử dụng bất kỳ "mẹo vặt" (hardcode hacks) nào mà tuân thủ chặt chẽ:
1. **Lưới tỷ lệ tương đối (`Grid *` kết hợp `Auto`)** thay cho các con số pixel cố định.
2. **`WordWrap` tự nhiên** kết hợp phân tầng cho các trường dữ liệu động.
3. **Chuẩn công thái học ngón tay cái (Mobile Thumb-Zone)** cho vùng chạm và các nút hành động chính.
4. **Hệ thống Design Tokens tập trung** giúp giao diện co giãn đồng bộ từ màu sắc, khoảng cách đến cỡ chữ.
