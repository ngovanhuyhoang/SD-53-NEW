﻿using System;
using System.ComponentModel.DataAnnotations;

namespace QuanApi.Dtos
{
    public class CreatePhieuGiamGiaDto
    {
        [Required, MaxLength(50)]
        public string MaCode { get; set; }

        [Required, MaxLength(300)]
        public string TenPhieu { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn hoặc bằng 0.")]
        public decimal GiaTriGiam { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm tối đa phải lớn hơn hoặc bằng 0.")]
        public decimal? GiaTriGiamToiDa { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn tối thiểu phải lớn hơn hoặc bằng 0.")]
        public decimal? DonToiThieu { get; set; }

        [Required, Range(0, 32767, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        public short SoLuong { get; set; }

        [Required]
        public bool LaCongKhai { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;

        [MaxLength(100)]
        public string? NguoiTao { get; set; }
    }
}
