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
