using System;

namespace QuanApi.Dtos
{
    /// <summary>
    /// DTO dùng để trả về thông tin vai trò.
    /// </summary>
    public class VaiTroDto
    {
        public Guid IDVaiTro { get; set; }
        public string MaVaiTro { get; set; } = string.Empty;
        public string TenVaiTro { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
    }
}
