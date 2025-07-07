    namespace QuanApi.Dtos
{
    public class KhachHangDto
    {
        public Guid IDKhachHang { get; set; }
        public string MaKhachHang { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public string? NguoiTao { get; set; }
        public DateTime? LanCapNhatCuoi { get; set; }
        public string? NguoiCapNhat { get; set; }
        public bool TrangThai { get; set; }
    }
}
