# Chức năng Quản lý Kích cỡ (KichCo)

## Tổng quan
Chức năng quản lý kích cỡ được xây dựng tương tự như ChatLieu với đầy đủ các tính năng CRUD, tìm kiếm, bộ lọc và phân trang sử dụng Ajax modal và partial view.

## Cấu trúc Files

### API Layer (QuanApi)
- **Controller**: `QuanApi/Controllers/KichCoController.cs`
  - `GET /api/KichCo` - Lấy tất cả kích cỡ
  - `GET /api/KichCo/{id}` - Lấy kích cỡ theo ID
  - `POST /api/KichCo/Create` - Tạo kích cỡ mới
  - `PUT /api/KichCo/{id}` - Cập nhật kích cỡ
  - `DELETE /api/KichCo/{id}` - Xóa kích cỡ
  - `PUT /api/KichCo/ToggleStatus/{id}` - Bật/tắt trạng thái
  - `GET /api/KichCo/paged` - Lấy danh sách có phân trang và lọc

### MVC Layer (QuanView)
- **Controller**: `QuanView/Areas/Admin/Controllers/KichCoController.cs`
- **Views**: `QuanView/Areas/Admin/Views/KichCo/`
  - `Index.cshtml` - Trang danh sách chính
  - `_CreatePartial.cshtml` - Form tạo mới
  - `_EditPartial.cshtml` - Form chỉnh sửa
  - `_DetailsPartial.cshtml` - Hiển thị chi tiết

### Data Layer
- **Model**: `QuanApi/Data/KichCo.cs`
- **DbContext**: `QuanApi/Data/BanQuanAu1DbContext.cs` (đã có DbSet KichCos)

## Tính năng

### 1. Hiển thị danh sách
- Bảng hiển thị với thông tin: Mã, Tên, Ngày tạo, Người tạo, Cập nhật, Người cập nhật, Trạng thái
- Phân trang với navigation
- Responsive design

### 2. Tìm kiếm và Bộ lọc
- Tìm kiếm theo từ khóa (Mã kích cỡ, Tên kích cỡ)
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
2. Vào menu "Sản Phẩm" > "Kích cỡ"
3. Hoặc truy cập trực tiếp: `/Admin/KichCo/Index`

### Thêm kích cỡ mới
1. Click nút "+ Thêm kích cỡ mới"
2. Điền thông tin: Mã kích cỡ, Tên kích cỡ
3. Chọn trạng thái (mặc định là kích hoạt)
4. Click "Thêm"

### Chỉnh sửa kích cỡ
1. Click nút "Sửa" (biểu tượng bút chì)
2. Chỉnh sửa thông tin trong modal
3. Click "Lưu"

### Xem chi tiết
1. Click nút "Chi tiết" (biểu tượng mắt)
2. Xem thông tin đầy đủ trong modal

### Xóa kích cỡ
1. Click nút "Xóa" (biểu tượng thùng rác)
2. Xác nhận trong dialog
3. Kích cỡ sẽ bị xóa khỏi hệ thống

### Bật/tắt trạng thái
1. Click vào switch button trong cột "Trạng thái"
2. Trạng thái sẽ thay đổi ngay lập tức

## API Endpoints

### Lấy danh sách có phân trang
```
GET /api/KichCo/paged?page=1&pageSize=10&keyword=S&trangThai=active
```

### Tạo mới
```
POST /api/KichCo/Create
Content-Type: application/json

{
  "maKichCo": "KC001",
  "tenKichCo": "Kích cỡ S",
  "trangThai": true
}
```

### Cập nhật
```
PUT /api/KichCo/{id}
Content-Type: application/json

{
  "maKichCo": "KC001",
  "tenKichCo": "Kích cỡ S Updated",
  "trangThai": true
}
```

### Xóa
```
DELETE /api/KichCo/{id}
```

### Toggle Status
```
PUT /api/KichCo/ToggleStatus/{id}
```

## Validation

### Client-side
- Mã kích cỡ: Required, MaxLength(50)
- Tên kích cỡ: Required, MaxLength(50)
- Trạng thái: Boolean

### Server-side
- Kiểm tra duplicate mã kích cỡ
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