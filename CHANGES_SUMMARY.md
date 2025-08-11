# Tóm tắt thay đổi - Bán hàng tại quầy

## Các thay đổi đã thực hiện

### 1. Logic trạng thái hóa đơn
- **Trước**: Tất cả đơn hàng đều có trạng thái "DaThanhToan"
- **Sau**: 
  - Nếu có giao hàng + có địa chỉ + thanh toán bằng tiền mặt → Trạng thái "DaXacNhan"
  - Các trường hợp khác → Trạng thái "DaThanhToan"

### 2. Phí vận chuyển cố định
- **Trước**: Phí vận chuyển có thể nhập tùy ý
- **Sau**: Phí vận chuyển cố định 50.000 VNĐ khi bật giao hàng

### 3. Các file đã sửa đổi

#### API Controller (`SD-53/QuanApi/Controllers/BanHangTaiQuayController.cs`)
- Thêm logic xác định trạng thái hóa đơn dựa trên điều kiện
- Thêm trường `ShippingFee` vào `InvoiceDto`
- Thêm logic lưu phí vận chuyển vào trường `PhiVanChuyen`

#### Model (`SD-53/QuanApi/Data/HoaDon.cs`)
- Thêm trường `PhiVanChuyen` để lưu phí vận chuyển riêng biệt

#### Frontend (`SD-53/QuanView/Areas/Admin/Views/ClientBanHangTaiQuay/Index.cshtml`)
- Thay đổi input phí vận chuyển thành readonly
- Thêm ghi chú "Phí vận chuyển cố định: 50.000 VNĐ"
- Cập nhật hàm `toggleShipping()` để tự động set phí vận chuyển 50.000 VNĐ
- Cập nhật hàm `updatePaymentInfo()` để sử dụng phí vận chuyển cố định
- Cập nhật dữ liệu gửi lên API để bao gồm phí vận chuyển

### 4. Database
- Đã có migration `20250726100840_phivanchuyen.cs` để thêm trường `PhiVanChuyen` vào bảng `HoaDons`

## Cách hoạt động

1. **Khi bật giao hàng**: 
   - Phí vận chuyển tự động set thành 50.000 VNĐ
   - Hiển thị form nhập địa chỉ giao hàng

2. **Khi thanh toán**:
   - Nếu có giao hàng + có địa chỉ + thanh toán tiền mặt → Trạng thái "DaXacNhan"
   - Nếu không → Trạng thái "DaThanhToan"
   - Phí vận chuyển được lưu vào trường `PhiVanChuyen` và cộng vào `TongTien`

3. **Giao diện**:
   - Phí vận chuyển hiển thị cố định 50.000 VNĐ
   - Không thể thay đổi giá trị phí vận chuyển
