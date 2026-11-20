# TÀI LIỆU QUY TẮC NGHIỆP VỤ (BUSINESS.MD)

Tài liệu định nghĩa toàn bộ quy tắc nghiệp vụ, các giả định thiết kế và 5 điểm tự quyết của ứng dụng **"Việc của tôi" (MyTasksApp)**.

---

## 1. Năm Điểm Tự Quyết (5 Self-Determined Decisions)

Ứng dụng tuân thủ nghiêm ngặt 5 quyết định đã thống nhất để đảm bảo tính nhất quán:

| STT | Điểm tự quyết | Lựa chọn thực thi | Lý do & Ý nghĩa nghiệp vụ |
|---|---|---|---|
| **1** | **Nhóm "Xong" (Màn 1)** | Chỉ hiển thị các công việc có hạn chót trong ngày hôm nay đã hoàn thành, hoặc các việc vừa được đánh dấu hoàn thành trong phiên làm việc hôm nay. | Giúp người dùng tập trung tuyệt đối vào kết quả và năng suất của ngày hiện tại, tránh làm tràn màn hình bởi danh sách việc cũ từ quá khứ. |
| **2** | **Ngày bắt đầu tuần (Màn 2)** | Bắt đầu từ **Thứ Hai** và kết thúc vào **Chủ Nhật** (chuẩn ISO 8601). | Phù hợp với văn hóa làm việc và lịch công tác thực tế của người dùng và doanh nghiệp tại Việt Nam. |
| **3** | **Trạng thái ngày chọn (Màn 2)** | **Giữ nguyên ngày đã chọn** khi người dùng chuyển tab và quay lại màn hình Tuần này. | Tránh ngắt quãng luồng tư duy; người dùng không bị mất ngữ cảnh ngày đang kiểm tra khi tạm thời chuyển sang tab khác. |
| **4** | **Ghi chú công việc (Màn 3)** | **Cho phép chỉnh sửa** và lưu ngay vào bộ dữ liệu in-memory. | Gia tăng tính tương tác, hỗ trợ ghi chú bổ sung khi xử lý việc và chứng minh năng lực ràng buộc dữ liệu 2 chiều (Two-way binding). |
| **5** | **Tính bền vững của dữ liệu** | **Phân tầng:**<br>- **Cài đặt** (Theme, Cỡ chữ, Ngôn ngữ): Lưu bền vững qua `Preferences`.<br>- **Dữ liệu công việc**: Lưu In-memory (tự reset về mock data chuẩn khi khởi động lại, kèm nút Reset chủ động). | Đảm bảo trải nghiệm cá nhân hóa không bị mất, đồng thời cung cấp môi trường kiểm thử an toàn, sạch sẽ, dễ dàng lặp lại cho Reviewer. |

---

## 2. Quy Tắc Nghiệp Vụ Theo Từng Màn Hình

### 2.1. Màn hình 1 — Hôm nay (Today)
* **Mục đích:** Giúp người dùng nắm bắt việc cần làm gấp, việc trong ngày và việc đã hoàn tất.
* **Cấu trúc danh sách (3 nhóm):**
  1. **Quá hạn (Overdue):** Các việc chưa hoàn thành có ngày hết hạn trước ngày hiện tại (`DueDate < Today && !IsCompleted`). Hiển thị cảnh báo trực quan (màu sắc/badge).
  2. **Hôm nay (Due Today):** Các việc chưa hoàn thành có hạn chót là ngày hiện tại (`DueDate == Today && !IsCompleted`).
  3. **Xong (Completed):** Các việc đã hoàn thành của ngày hôm nay (`IsCompleted == true`).
* **Thao tác hoàn thành việc:**
  - Hỗ trợ cả **chạm (tap vào checkbox/icon)** hoặc **vuốt (SwipeView)** để chuyển nhanh trạng thái hoàn thành.
* **Trạng thái hệ thống:**
  - **Loading:** Hiển thị giao diện shimmer/spinner khi đang tải dữ liệu.
  - **Empty:** Khi một nhóm hoặc toàn bộ danh sách không có việc, hiển thị hình ảnh minh họa + thông điệp tích cực (ví dụ: *"Tuyệt vời! Bạn không còn việc nào quá hạn"*).

---

### 2.2. Màn hình 2 — Tuần này (This Week)
* **Mục đích:** Giúp nhân viên có cái nhìn tổng quan về khối lượng công việc trong cả tuần làm việc.
* **Dải chọn 7 ngày (Week Strip):**
  - Hiển thị 7 ngày từ Thứ Hai đến Chủ Nhật của tuần hiện tại.
  - Mỗi ngày hiển thị: Tên thứ (T2, T3...), ngày trong tháng (vd: 20, 21...), và **badge số lượng việc** cần làm trong ngày đó.
  - Highlight rõ ràng **ngày đang được chọn** và **ngày hôm nay**.
* **Tương tác:**
  - Chạm vào bất kỳ ngày nào trong dải 7 ngày để tải danh sách công việc của ngày đó bên dưới.
  - Chuyển tab quay lại vẫn bảo toàn ngày đã chọn (theo Điểm tự quyết 3).
* **Trạng thái hệ thống:**
  - Có đầy đủ trạng thái Loading và Empty (khi ngày được chọn không có công việc nào).

---

### 2.3. Màn hình 3 — Chi tiết việc (Task Detail)
* **Mục đích:** Xem chi tiết toàn diện và cập nhật trạng thái/ghi chú của một công việc cụ thể.
* **Thông tin hiển thị:** Tiêu đề (Title), Dự án (Project tag/name), Hạn chót (Due date), Trạng thái (Status badge), Mô tả chi tiết (Description), và Ghi chú (Notes).
* **Thao tác trạng thái:**
  - **Hoàn thành (Mark as Done):** Cho phép bấm hoàn thành trực tiếp.
  - **Mở lại (Reopen):** Bắt buộc hiển thị **Hộp thoại xác nhận (Confirmation Dialog)**: *"Bạn có chắc chắn muốn mở lại công việc này?"*. Chỉ khi người dùng bấm xác nhận thì trạng thái mới chuyển lại thành Chưa xong.
* **Thao tác Ghi chú:**
  - Cho phép người dùng chỉnh sửa nội dung ghi chú và bấm nút Lưu (cập nhật vào mock service).
* **Trường hợp Not Found:**
  - Nếu truy cập với ID không tồn tại (hoặc qua chế độ test của Reviewer), hiển thị màn hình thông báo rõ ràng *"Không tìm thấy công việc"* kèm nút điều hướng quay về màn hình trước.

---

### 2.4. Màn hình 4 — Cài đặt (Settings)
* **Mục đích:** Cá nhân hóa giao diện và cung cấp công cụ kiểm thử cho người đánh giá.
* **Các thiết lập người dùng (Áp dụng tức thì, không cần restart):**
  1. **Giao diện (Theme):** Sáng (Light) / Tối (Dark). Thay đổi đồng bộ toàn bộ Background, Surface, Text, Border, Icons.
  2. **Cỡ chữ (Font Size):** Nhỏ (Small) / Thường (Medium) / Lớn (Large). Responsive toàn diện, cỡ chữ Lớn không làm vỡ bố cục hay che khuất nút bấm.
  3. **Ngôn ngữ (Language):** Tiếng Việt (VI) / Tiếng Anh (EN). Cập nhật ngay toàn bộ tiêu đề, nhãn, nút bấm, thông điệp dialog và văn bản trạng thái.
* **Bộ công cụ Reviewer (Reviewer Tools / Debug Panel):**
  - **Switch Giả lập Loading:** Bật để ép danh sách ở Màn 1 & Màn 2 luôn ở trạng thái Đang tải.
  - **Switch Giả lập Empty:** Bật để ép danh sách ở Màn 1 & Màn 2 hiển thị trạng thái Trống rỗng.
  - **Nút "Mở công việc không tồn tại":** Điều hướng sang Màn hình 3 với ID rác (`id=-1`) để chứng minh xử lý Not Found.
  - **Nút "Đặt lại dữ liệu mẫu":** Khôi phục dữ liệu in-memory về trạng thái khởi tạo gốc.

---

## 3. Cơ Chế Quản Lý State & Đồng Bộ Dữ Liệu
* **Single Source of Truth:** `ITaskService` được đăng ký dạng Singleton trong Dependency Injection container của MAUI (`MauiProgram.cs`).
* **Đồng bộ đa màn hình:** Khi một công việc được hoàn thành hoặc mở lại ở bất kỳ màn hình nào (Hôm nay, Tuần này hoặc Chi tiết), `ITaskService` kích hoạt sự kiện `TaskChanged`. Tất cả các ViewModel đang lắng nghe sẽ tự động làm mới danh sách hiển thị, đảm bảo tính nhất quán tuyệt đối.
* **Tuân thủ quy tắc Agent:** Toàn bộ cơ chế MVVM được xây dựng bằng C# thuần (không sử dụng thư viện ngoài), sạch sẽ, tối ưu và dễ giải trình.
