using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanApi.Dtos
{
    public class NhanVienUpdateDto
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

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự và không quá 100 ký tự.")]
        public string? MatKhau { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
        [CustomValidation(typeof(NhanVienUpdateDto), nameof(ValidateAge))]
        public DateTime? NgaySinh { get; set; }

        public bool? GioiTinh { get; set; }

        [StringLength(100, ErrorMessage = "Quê quán không được vượt quá 100 ký tự.")]
        public string? QueQuan { get; set; }

        [StringLength(20, ErrorMessage = "CCCD không được vượt quá 20 ký tự.")]
        public string? CCCD { get; set; }

        [Required(ErrorMessage = "ID Vai trò là bắt buộc.")]
        public Guid IDVaiTro { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        public bool TrangThai { get; set; }


        [Required]
        public Guid IDNguoiCapNhat { get; set; }

        public Guid IDNhanVienCanCapNhat { get; set; }

        public static ValidationResult? ValidateAge(DateTime? ngaySinh, ValidationContext context)
        {
            if (ngaySinh.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - ngaySinh.Value.Year;
                if (ngaySinh.Value.Date > today.AddYears(-age)) age--;

                if (age < 18)
                {
                    return new ValidationResult("Nhân viên phải trên 18 tuổi.", new[] { nameof(NgaySinh) });
                }
            }
            return ValidationResult.Success;
        }
    }
}