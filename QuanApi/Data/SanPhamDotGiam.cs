using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanApi.Data
{
    public class SanPhamDotGiam
    {
        [Key]
        public Guid IDSanPhamDotGiam { get; set; } = Guid.NewGuid();
        public string MaSanPhamDotGiam { get; set; }
        public Guid IDSanPhamChiTiet { get; set; }
        public Guid IDDotGiamGia { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal GiaGoc { get; set; }
        public string? NguoiTao { get; set; }
        public DateTime? LanCapNhatCuoi { get; set; }
        public string? NguoiCapNhat { get; set; }
        public bool TrangThai { get; set; }
        [ForeignKey("IDSanPhamChiTiet")]
        public virtual SanPhamChiTiet? SanPhamChiTiet { get; set; }
        [ForeignKey("IDDotGiamGia")]
        public virtual DotGiamGia? DotGiamGia { get; set; }
    }
}
