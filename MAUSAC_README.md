# Chức năng Quản lý Màu sắc (MauSac)

## Tổng quan
Chức năng quản lý màu sắc được xây dựng tương tự như ChatLieu và KichCo với đầy đủ các tính năng CRUD, tìm kiếm, bộ lọc và phân trang sử dụng Ajax modal và partial view.

## Cấu trúc Files

### API Layer (QuanApi)
- **Controller**: `QuanApi/Controllers/MauSacController.cs`
  - `GET /api/MauSac` - Lấy tất cả màu sắc
  - `GET /api/MauSac/{id}` - Lấy màu sắc theo ID
  - `POST /api/MauSac/Create` - Tạo màu sắc mới
  - `PUT /api/MauSac/{id}` - Cập nhật màu sắc
  - `DELETE /api/MauSac/{id}` - Xóa màu sắc
  - `PUT /api/MauSac/ToggleStatus/{id}` - Bật/tắt trạng thái
  - `GET /api/MauSac/paged` - Lấy danh sách có phân trang và lọc

### MVC Layer (QuanView)
- **Controller**: `QuanView/Areas/Admin/Controllers/MauSacController.cs`
- **Views**: `QuanView/Areas/Admin/Views/MauSac/`
  - `Index.cshtml` - Trang danh sách chính
  - `_CreatePartial.cshtml` - Form tạo mới
  - `_EditPartial.cshtml` - Form chỉnh sửa
  - `_DetailsPartial.cshtml` - Hiển thị chi tiết

### Data Layer
- **Model**: `QuanApi/Data/MauSac.cs`
- **DbContext**: `QuanApi/Data/BanQuanAu1DbContext.cs` (đã có DbSet MauSacs)

## Tính năng

### 1. Hiển thị danh sách
- Bảng hiển thị với thông tin: Mã, Tên, Ngày tạo, Người tạo, Cập nhật, Người cập nhật, Trạng thái
- Phân trang với navigation
- Responsive design

### 2. Tìm kiếm và Bộ lọc
- Tìm kiếm theo từ khóa (Mã màu sắc, Tên màu sắc)
- Bộ lọc theo trạng thái (Kích hoạt/Ẩn)
- Nút "Xóa lọc" để reset về trạng thái ban đầu

### 3. Thêm mới (Create)
- Modal popup với form validation
- Ajax submit không reload trang
- Hiển thị loading spinner
- Thông báo thành công/thất bại

### 4. Chỉnh sửa (Edit)
- Modal popup với dữ liệu được load
- Ajax submit
- Validation client-side và server-side
- Cập nhật thông tin người sửa và thời gian

### 5. Xem chi tiết (Details)
- Modal popup hiển thị đầy đủ thông tin
- Format ngày giờ đẹp
- Hiển thị trạng thái với badge màu

### 6. Xóa (Delete)
- Confirm dialog trước khi xóa
- Ajax delete
- Thông báo kết quả

### 7. Toggle Status
- Switch button để bật/tắt trạng thái
- Ajax update không reload trang
- Hiển thị trạng thái mới ngay lập tức

## Cách sử dụng

### Truy cập
1. Đăng nhập vào hệ thống admin
2. Vào menu "Sản Phẩm" > "Màu sắc"
3. Hoặc truy cập trực tiếp: `/Admin/MauSac/Index`

### Thêm màu sắc mới
1. Click nút "+ Thêm màu sắc mới"
2. Điền thông tin: Mã màu sắc, Tên màu sắc
3. Chọn trạng thái (mặc định là kích hoạt)
4. Click "Thêm"

### Chỉnh sửa màu sắc
1. Click nút "Sửa" (biểu tượng bút chì)
2. Chỉnh sửa thông tin trong modal
3. Click "Lưu"

### Xem chi tiết
1. Click nút "Chi tiết" (biểu tượng mắt)
2. Xem thông tin đầy đủ trong modal

### Xóa màu sắc
1. Click nút "Xóa" (biểu tượng thùng rác)
2. Xác nhận trong dialog
3. Màu sắc sẽ bị xóa khỏi hệ thống

### Bật/tắt trạng thái
1. Click vào switch button trong cột "Trạng thái"
2. Trạng thái sẽ thay đổi ngay lập tức

## API Endpoints

### Lấy danh sách có phân trang
```
GET /api/MauSac/paged?page=1&pageSize=10&keyword=đỏ&trangThai=active
```

### Tạo mới
```
POST /api/MauSac/Create
Content-Type: application/json

{
  "maMauSac": "MS001",
  "tenMauSac": "Màu đỏ",
  "trangThai": true
}
```

### Cập nhật
```
PUT /api/MauSac/{id}
Content-Type: application/json

{
  "maMauSac": "MS001",
  "tenMauSac": "Màu đỏ Updated",
  "trangThai": true
}
```

### Xóa
```
DELETE /api/MauSac/{id}
```

### Toggle Status
```
PUT /api/MauSac/ToggleStatus/{id}
```

## Validation

### Client-side
- Mã màu sắc: Required, MaxLength(50)
- Tên màu sắc: Required, MaxLength(50)
- Trạng thái: Boolean

### Server-side
- Kiểm tra duplicate mã màu sắc
- Validation model state
- Logging cho debugging

## Styling
- Sử dụng Bootstrap 5
- Custom CSS cho table và buttons
- Responsive design
- Loading spinners
- Status badges với màu sắc

## Error Handling
- Try-catch blocks trong controllers
- JSON error responses
- User-friendly error messages
- Console logging cho debugging

## Security
- CSRF token protection
- Input validation
- SQL injection prevention (Entity Framework)
- XSS prevention (HTML encoding)

## Performance
- Lazy loading với Include()
- Pagination để giảm tải
- Ajax requests để tránh reload
- Caching với Entity Framework

## Testing
File test API: `QuanApi/QuanApi.http`
- Test tất cả CRUD operations
- Test pagination và filtering
- Test error scenarios 