using System;
using System.ComponentModel.DataAnnotations;

namespace QuanApi.Dtos
{
    public class NhanVienCreateDto
    {
        [Required(ErrorMessage = "Mã nhân viên là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Mã nhân viên không được vượt quá 50 ký tự.")]
        public string MaNhanVien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên nhân viên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên nhân viên không được vượt quá 100 ký tự.")]
        public string TenNhanVien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự và không quá 100 ký tự.")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [CustomValidation(typeof(NhanVienCreateDto), "ValidateNgaySinh")]
        public DateTime? NgaySinh { get; set; }

        public bool? GioiTinh { get; set; }

        [StringLength(100, ErrorMessage = "Quê quán không được vượt quá 100 ký tự.")]
        public string? QueQuan { get; set; }

        [StringLength(12, MinimumLength = 12, ErrorMessage = "CCCD phải có đúng 12 số.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CCCD không hợp lệ, phải là 12 chữ số.")]
        public string? CCCD { get; set; }

        [Required(ErrorMessage = "ID Vai trò là bắt buộc.")]
        public Guid IDVaiTro { get; set; }

        public bool TrangThai { get; set; } = true;
        public Guid IDNguoiTao { get; set; }

        public static ValidationResult? ValidateNgaySinh(DateTime? ngaySinh, ValidationContext context)
        {
            if (ngaySinh.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - ngaySinh.Value.Year;
                if (ngaySinh.Value.Date > today.AddYears(-age))
                {
                    age--;
                }

                if (age < 18)
                {
                    return new ValidationResult("Nhân viên phải đủ 18 tuổi trở lên.", new[] { nameof(NgaySinh) });
                }
            }
            return ValidationResult.Success;
        }
    }
}