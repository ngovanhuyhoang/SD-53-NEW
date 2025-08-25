using System;

namespace QuanView.Areas.Admin.Models
{
    public class ThemVaoGioHangDto
    {
        public Guid IDGioHang { get; set; }
        public Guid IDSanPhamChiTiet { get; set; }
        public int SoLuong { get; set; }
        public string? NguoiCapNhat { get; set; }
    }
}
