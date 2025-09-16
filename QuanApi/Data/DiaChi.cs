using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanApi.Data
{
    public class DiaChi
    {
        [Key]
        public Guid IDDiaChi { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string MaDiaChi { get; set; }

        [Required]
        public Guid IDKhachHang { get; set; }

        [Required]
        [MaxLength(255)]
        public string DiaChiChiTiet { get; set; }

        [Required]
        public bool LaMacDinh { get; set; } = false;

        // Thêm các thuộc tính mới cho địa chỉ
        [MaxLength(100)]
        public string? TinhThanh { get; set; }

        [MaxLength(100)]
        public string? QuanHuyen { get; set; }

        [MaxLength(100)]
        public string? PhuongXa { get; set; }

        [MaxLength(100)]
        public string? TenNguoiNhan { get; set; }

        [MaxLength(20)]
        public string? SdtNguoiNhan { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? NguoiTao { get; set; }

        public DateTime? LanCapNhatCuoi { get; set; }

        [MaxLength(100)]
        public string? NguoiCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;

        [ForeignKey("IDKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        /// Thêm thuộc tính này để hiển thị địa chỉ đầy đủ, không cần lưu vào DB
        [NotMapped]
        public string DiaChiDayDu
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(DiaChiChiTiet)) parts.Add(DiaChiChiTiet);
                if (!string.IsNullOrEmpty(PhuongXa)) parts.Add(PhuongXa);
                if (!string.IsNullOrEmpty(QuanHuyen)) parts.Add(QuanHuyen);
                if (!string.IsNullOrEmpty(TinhThanh)) parts.Add(TinhThanh);
                return string.Join(", ", parts);
            }
        }
    }
}