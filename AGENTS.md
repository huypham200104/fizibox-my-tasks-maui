# AGENTS Rules & Guidelines

1. **Kiến trúc & Pattern**: Sử dụng mô hình **MVVM** kết hợp **Clean Architecture**. Phân tách rõ ràng giữa Core/Domain, Application/Services, và UI (Views/ViewModels).
2. **Quản lý Git**: **Không** tự ý commit hoặc push code lên repository.
3. **Giới hạn thao tác & Quản lý Package**:
   - **Chỉ cài đặt, gỡ bỏ hoặc nâng cấp package (NuGet, npm,...) khi có sự đồng ý/cho phép rõ ràng của người dùng qua câu hỏi xác nhận**.
   - Không xóa file, đổi tên file, di chuyển vị trí thư mục.
   - Không can thiệp vào các tệp cấu hình cốt lõi (`.slnx`, `.csproj`, build configs) trừ khi được cấp phép để thêm package đã được phê duyệt.
   - Không thay đổi cấu trúc Database mà không tạo file Migration chính thống.
4. **Quy tắc trao đổi**:
   - Khi có điểm chưa rõ hoặc cần sửa đổi từ 2 file core trở lên, **bắt buộc dùng popup hỏi người dùng (`ask_question`)**.
5. **Tài liệu tham khảo bắt buộc**:
   - Xem `ROADMAP.md` để theo dõi tiến độ các tính năng (đã làm / chưa làm).
   - Xem `BUSINESS.md` để nắm rõ các nghiệp vụ ẩn và quy tắc logic trong ứng dụng.
   - Xem `DESIGN.md` để tuân thủ bảng màu (tokens), kích thước, font chữ và các quy chuẩn giao diện.
6. **Quy chuẩn Thiết kế Responsive cho mọi kích thước màn hình**:
   - **Tương thích đa kích thước**: Đảm bảo giao diện hiển thị chuẩn xác, không vỡ layout từ thiết bị màn hình siêu hẹp (320dp – 360dp) đến kích thước chuẩn (390dp – 412dp) và tablet (>= 600dp).
   - **Bố cục Task Card 2 tầng**: Để tiêu đề công việc không bao giờ bị cắt cụt (`...`), luôn bố trí theo 2 tầng: Dòng trên dành trọn cho Checkbox và Tiêu đề; Dòng dưới dành cho Tag phân loại/Dự án và Badge thời hạn/Giờ hẹn.
   - **Chống tràn nội dung (Anti-overflow)**: Tuyệt đối không nhồi nhét chuỗi văn bản dài cùng số liệu động trong một `HorizontalStackLayout` chật hẹp; ưu tiên bố cục tách tầng, `Grid` linh hoạt (kết hợp `*` và `Auto`), hoặc `FlexLayout`.
   - **Vùng chạm tương tác (Touch Target)**: Mọi phần tử có thể chạm (Checkbox, Button, Floating Button, Action Icon) phải có vùng chạm tối thiểu `44x44dp` (hoặc tối thiểu `36x36dp` với padding/margin hỗ trợ) để tránh bấm trượt.
   - **Cắt góc ảnh chuẩn xác (Image Clipping)**: Với các hình ảnh hoặc logo có nền màu riêng, luôn sử dụng `Clip` hình học (`RoundRectangleGeometry`) tương ứng với góc bo của container để triệt tiêu các góc vuông nhọn tràn ra ngoài.
7. **Quy định chạy Test**:
   - Tuyệt đối **không** tự ý chạy test (unit test, automated test,...) trong quá trình thực hiện công việc.
   - **Chỉ chạy test khi người dùng có yêu cầu rõ ràng với lệnh `/grill-me`**.
8. **Quy định về Dữ liệu (Tuyệt đối không Mock Data)**:
   - Tuyệt đối **không** tự ý tạo hoặc sử dụng mock data, fake data, hardcoded data trong code, ViewModel hay Service.
   - Mọi dữ liệu phải được kết nối, xử lý và lưu trữ thông qua cơ chế dữ liệu thực tế (Database, Service thực tế, Persistent Storage,...) theo đúng kiến trúc của ứng dụng (trừ khi có yêu cầu cụ thể từ người dùng).


