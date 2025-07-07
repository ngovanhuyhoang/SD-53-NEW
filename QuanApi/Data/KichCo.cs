﻿using System.ComponentModel.DataAnnotations;

namespace QuanApi.Data
{
    public class KichCo
    {
        [Key]
        public Guid IDKichCo { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string MaKichCo { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenKichCo { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? NguoiTao { get; set; }

        public DateTime? LanCapNhatCuoi { get; set; }

        [MaxLength(100)]
        public string? NguoiCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;
        public virtual ICollection<SanPhamChiTiet>? SanPhamChiTiets { get; set; }
    }
}
