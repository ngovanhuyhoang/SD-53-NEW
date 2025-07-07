using System.ComponentModel.DataAnnotations;

namespace QuanApi.Dtos
{
    public class CreateKhachHangDto
    {
        [Required(ErrorMessage = "Mã khách hàng là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Mã khách hàng không được vượt quá 50 ký tự.")]
        public string MaKhachHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự.")]
        public string TenKhachHang { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")] 
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 255 ký tự.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string SoDienThoai { get; set; } = string.Empty;

        public bool TrangThai { get; set; } = true; 
    }
}