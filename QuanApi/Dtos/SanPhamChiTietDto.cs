using System.ComponentModel.DataAnnotations;

namespace QuanApi.Dtos
{
    public class SanPhamChiTietDto
    {
        public Guid IdSanPhamChiTiet { get; set; }
        public Guid IdSanPham { get; set; }
        public Guid IdKichCo { get; set; }
        public Guid IdMauSac { get; set; }
        public Guid IdHoaTiet { get; set; }
        
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SoLuong { get; set; }
        
        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal GiaBan { get; set; }
        public decimal originalPrice { get; set; }
        public decimal price { get; set; }
        public string? TenKichCo { get; set; }
        public string? TenMauSac { get; set; }
        public string? TenHoaTiet { get; set; }
        public string? TenSanPham { get; set; } // Thêm trường này
        public string? TenDanhMuc { get; set; } // Thêm trường này cho view khách hàng
        public string? AnhDaiDien { get; set; } // Thêm trường này cho view khách hàng
        public bool TrangThai { get; set; } // Thêm trường này
        public string? MaSPChiTiet { get; set; } // Thêm trường này nếu cần
    }
}
