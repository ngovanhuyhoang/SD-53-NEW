using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanApi.Data
{
    public class KhachHangPhieuGiam
    {
        [Key]
        public Guid IDKhachHangPhieuGiam { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string MaKhachHangPhieuGiam { get; set; }

        [Required]
        public Guid IDKhachHang { get; set; }

        [Required]
        public Guid IDPhieuGiamGia { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? NguoiTao { get; set; }

        public DateTime? LanCapNhatCuoi { get; set; }

        [MaxLength(100)]
        public string? NguoiCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;
        [ForeignKey("IDKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }
        // 1-n: PhieuGiamGia -> KhachHangPhieuGiam
        [ForeignKey("IDPhieuGiamGia")]
        public virtual PhieuGiamGia? PhieuGiamGia { get; set; }
    }
}
