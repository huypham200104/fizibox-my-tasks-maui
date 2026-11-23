# Bài thực hành Fresher Mobile (.NET MAUI/C#) — UI "Việc của tôi"

## 1. Thông tin chung & Lịch trình

| Hạng mục | Chi tiết |
|---|---|
| **Đơn vị đánh giá** | Fizibox (`https://fizibox.com`) |
| **Liên hệ** | Luna (thay mặt Thanh Nguyen — `thanh@fizibox.com`, SĐT: `0353256811`) |
| **Vị trí** | Fresher Mobile (.NET MAUI / C#) |
| **Nền tảng mục tiêu** | Android Emulator hoặc điện thoại Android *(không yêu cầu iOS)* |
| **Thời lượng** | 2 ngày lịch: **20/11 – 21/11/2026** |
| **Hạn nộp bài dự kiến** | Trước **23:59 ngày 21/11/2026** |
| **Hình thức nộp** | Link Git repo **Private** *(đang chờ Fizibox cung cấp tài khoản/email reviewer để cấp quyền Read/Collaborator)* |
| **Buổi review online** | ~30 phút — đề xuất ngày **22/11 (CN)** hoặc **23/11/2026 (T2)** |

> **Lưu ý định hướng:**
> - Được sử dụng AI và tài liệu thoải mái; không yêu cầu kinh nghiệm MAUI trước đây.
> - Đây là bài đánh giá độc lập, không phải công việc production.
> - Ưu tiên đánh giá **tính thẩm mỹ, sự nhất quán của giao diện** và khả năng **giải thích quyết định thiết kế, hiểu & kiểm soát code** — không đánh giá theo số dòng code, độ dài prompt hay làm thêm tính năng ngoài phạm vi.

---

## 2. Bối cảnh & Yêu cầu 4 màn hình

Xây dựng giao diện ứng dụng **"Việc của tôi"** giúp nhân viên theo dõi công việc cá nhân trong tuần. Chỉ sử dụng **dữ liệu giả (mock data)** lưu in-memory hoặc file local, **không cần backend**.

### Màn hình 1 — Hôm nay
- Hiển thị danh sách công việc gồm: **tiêu đề, dự án, hạn và trạng thái**.
- Phân nhóm thành 3 nhóm: **Quá hạn / Hôm nay / Xong**.
- Hỗ trợ **bấm hoặc vuốt** để hoàn thành công việc.
- Có giao diện trạng thái **Đang tải (Loading)** và **Danh sách trống (Empty)**.

### Màn hình 2 — Tuần này
- Hiển thị lịch hoặc dải chọn **7 ngày**, kèm **dấu hiệu hoặc số công việc** mỗi ngày.
- Chạm vào một ngày để xem danh sách công việc của ngày đó.
- Có giao diện trạng thái **Đang tải (Loading)** và **Danh sách trống (Empty)**.

### Màn hình 3 — Chi tiết việc
- Hiển thị đầy đủ: **tiêu đề, mô tả, dự án, hạn, trạng thái và ghi chú**.
- Có thao tác **Hoàn thành / Mở lại**.
- **Bắt buộc xác nhận (Confirmation Dialog)** trước khi mở lại công việc đã hoàn thành.
- Có giao diện xử lý phù hợp khi **không tìm thấy công việc (Not Found)**.

### Màn hình 4 — Cài đặt
- Chuyển giao diện: **Sáng / Tối** (Light / Dark mode).
- Chọn cỡ chữ: **Nhỏ / Thường / Lớn**.
- Chuyển ngôn ngữ: **Việt / Anh** (VI / EN).
- Tất cả thay đổi phải **áp dụng ngay lập tức trên toàn ứng dụng**, không cần khởi động lại.
- *Không cần làm trạng thái Loading hoặc Empty nếu không có lý do sử dụng thực tế.*

---

## 3. Yêu cầu về Thiết kế, Hành vi & Điểm tự quyết

### 3.1. Hệ thống thiết kế nhất quán (Design System)
- Tự chọn màu sắc, font, cách điều hướng và bố cục nhưng phải thống nhất.
- **Quản lý tập trung & tái sử dụng:** màu sắc, kiểu chữ, spacing, bo góc phải khai báo tập trung *(không hardcode/sửa màu rải rác ở từng màn hình)*.
- **Phân cấp & Khả năng thao tác:** phân cấp thông tin rõ ràng, chữ dễ đọc, nút dễ bấm.
- **Dark Mode hoàn chỉnh:** xử lý đầy đủ nền, chữ, viền, biểu tượng và các trạng thái — không chỉ đổi màu nền.
- **Responsive cỡ chữ:** khi tăng cỡ chữ lên mức Lớn, nội dung và thao tác quan trọng không bị cắt hoặc chồng lấn.
- **Font chữ:** được dùng font hệ thống, không bắt buộc font riêng hay trang trí cầu kỳ *(đơn giản nhưng cân đối, có chủ đích và hoàn thiện tốt ở Sáng/Tối, chữ Lớn và VI/EN vẫn được đánh giá cao)*.

### 3.2. Trạng thái & Dữ liệu
- **Đồng bộ state:** hoàn thành hoặc mở lại một việc phải cập nhật nhất quán ngay trên cả **Hôm nay, Tuần này và Chi tiết**.
- **Chế độ giả lập cho reviewer:** có cơ chế kích hoạt các trạng thái **Loading**, **Empty** và **Not Found** *(chấp nhận nút hoặc mục "Giả lập" riêng cho bài)*.
- **Không tạo độ trễ giả dài** chỉ để trình diễn Loading.
- **Song ngữ VI/EN:** đổi ngôn ngữ phải cập nhật toàn bộ nhãn giao diện, thông báo và hộp thoại xác nhận *(không bắt buộc dịch nội dung công việc do người dùng nhập)*.

### 3.3. Năm điểm tự quyết (Phải ghi rõ giả định & lý do trong `README.md`)
Ứng dụng phải hoạt động nhất quán đúng với lựa chọn đã mô tả:
1. Nhóm **"Xong"** gồm những việc nào?
2. **Tuần** bắt đầu vào ngày nào?
3. Khi quay lại màn hình **Tuần này**, ngày đã chọn được **giữ lại** hay **đặt lại**?
4. **Ghi chú** chỉ hiển thị hay cho phép chỉnh sửa?
5. Trạng thái công việc và cài đặt có được **lưu bền vững** sau khi đóng và mở lại app không?

### 3.4. Ngoài phạm vi (Out of Scope — Không cần làm)
- Đăng nhập / xác thực
- Backend / API thật
- Push notification
- Đồng bộ offline
- Animation phức tạp
- Phát hành lên store
- Test tự động

---

## 4. Hồ sơ nộp bài

### 4.1. Mã nguồn
- Đầy đủ source code và tài nguyên để người khác dựng và chạy được ứng dụng.
- **Cấm tuyệt đối:** API key, mật khẩu, dữ liệu thật hoặc mã nguồn của công ty khác.
- *Commit history không bắt buộc số lượng/tần suất cụ thể và không chấm điểm riêng.*

### 4.2. File `README.md` (7 nội dung bắt buộc)
1. Phiên bản SDK / .NET MAUI, công cụ và thiết bị/emulator đã dùng.
2. Các bước cài đặt, build và chạy.
3. Cấu trúc project.
4. Các quyết định thiết kế chính và 5 giả định nghiệp vụ tự quyết.
5. Cách kích hoạt các trạng thái giả lập (Loading, Empty, Not Found).
6. Ảnh chụp **4 màn hình**, bổ sung ảnh thể hiện **Dark mode** và **Cỡ chữ lớn**.
7. Những trường hợp đã kiểm thử, phần chưa hoàn thành hoặc chưa kiểm chứng.

### 4.3. File `AI-NOTES.md` (Ngắn gọn, chỉ nội dung của bài này)
- Công cụ AI đã sử dụng và cách chia công việc.
- **3 ví dụ tiêu biểu:** Yêu cầu/prompt -> Kết quả AI -> Quyết định giữ, sửa hay bỏ + lý do.
- **1 trường hợp AI tạo kết quả sai hoặc chưa phù hợp:** cách phát hiện và kiểm tra lại sau khi sửa *(nếu không gặp, ghi rõ và mô tả 1 đề xuất đã kiểm tra kỹ)*.
- *Không cần nộp toàn bộ lịch sử chat hay thông tin tài khoản.*

---

## 5. Tiêu chí nghiệm thu (Acceptance Checklist)

- [ ] Bốn màn hình chạy được trên Android, điều hướng không gây crash.
- [ ] Hoàn thành / mở lại công việc cập nhật nhất quán giữa các màn hình.
- [ ] Theme (Sáng/Tối), cỡ chữ (Nhỏ/Thường/Lớn) và ngôn ngữ (VI/EN) thay đổi ngay trên toàn app.
- [ ] Chứng minh được các trạng thái giả lập theo đề (Loading, Empty, Not Found).
- [ ] Tài nguyên thiết kế được quản lý tập trung, sử dụng nhất quán.
- [ ] `README.md` đủ rõ để người khác dựng và chạy bài.
- [ ] `AI-NOTES.md` phản ánh đúng các quyết định thực tế trong quá trình làm.

---

## 6. Thang điểm đánh giá

| Nhóm đánh giá | Điểm tối đa |
|---|---:|
| **Thẩm mỹ, dễ sử dụng và nhất quán** | **40** |
| Hoàn thiện chức năng và chạy được | 15 |
| Quyết định thiết kế có lý do | 15 |
| Code rõ ràng, tổ chức và quản lý state hợp lý | 15 |
| Giải thích, kiểm chứng và sửa thay đổi nhỏ trong buổi review | 15 |
| **Tổng** | **100** |

> **Cách chấm:** Đối chiếu giữa **repo, app chạy thật, `README.md`, `AI-NOTES.md` và buổi trao đổi**. Thang điểm dùng để trao đổi/so sánh, không tự động quyết định đạt hay không đạt; mức độ hoàn thiện tính năng và khả năng kiểm soát quá trình làm được ghi nhận riêng.

---

## 7. Cấu trúc buổi Review online (30 phút)

- **10 phút đầu:** Trình bày ứng dụng và các quyết định thiết kế chính.
- **20 phút sau:**
  - Trao đổi về code, cách dùng AI và cách kiểm thử.
  - Thực hiện **1 thay đổi nhỏ tại chỗ** *(ví dụ: đổi màu chủ đạo hoặc thêm số việc quá hạn)*.
  - Vẫn được dùng AI và tài liệu; người review quan sát **cách xác định phạm vi, đọc thay đổi và kiểm tra kết quả**, không chỉ tốc độ thao tác.
- **Nguyên tắc minh bạch:** Nếu gặp lỗi môi trường, hai bên đối chiếu hướng dẫn và điều kiện chạy trước khi đánh giá. Nếu có phần chưa hoàn thành, cần nêu rõ — phân biệt được **"đã làm"**, **"đã thử"** và **"chưa kiểm chứng"** là năng lực quan trọng.