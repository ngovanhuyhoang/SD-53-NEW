using System.ComponentModel.DataAnnotations;

namespace QuanView.Areas.Admin.Models
{
    public class AddAnhSanPhamDto
    {
        [Required(ErrorMessage = "URL ảnh không được để trống")]
        public string UrlAnh { get; set; }
        
        public bool LaAnhChinh { get; set; } = false;
    }
} 