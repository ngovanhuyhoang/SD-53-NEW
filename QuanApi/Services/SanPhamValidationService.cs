using QuanApi.Data;
using System.ComponentModel.DataAnnotations;

namespace QuanApi.Services
{
    public class SanPhamValidationService
    {
        public List<string> ValidateSanPham(SanPham sanPham)
        {
            var errors = new List<string>();

            // Validate basic product info
            if (string.IsNullOrWhiteSpace(sanPham.MaSanPham))
                errors.Add("Mã sản phẩm không được để trống.");

            if (string.IsNullOrWhiteSpace(sanPham.TenSanPham))
                errors.Add("Tên sản phẩm không được để trống.");

            // Validate product details if they exist
            if (sanPham.SanPhamChiTiets != null && sanPham.SanPhamChiTiets.Any())
            {
                foreach (var chiTiet in sanPham.SanPhamChiTiets)
                {
                    var chiTietErrors = ValidateSanPhamChiTiet(chiTiet);
                    errors.AddRange(chiTietErrors);
                }
            }

            return errors;
        }

        public List<string> ValidateSanPhamChiTiet(SanPhamChiTiet chiTiet)
        {
            var errors = new List<string>();

            // Validate quantity - must not be negative
            if (chiTiet.SoLuong < 0)
                errors.Add("Số lượng không được là số âm.");

            // Validate price - must not be negative
            if (chiTiet.GiaBan < 0)
                errors.Add("Giá bán không được là số âm.");

            // Additional business rules
            if (chiTiet.GiaBan == 0)
                errors.Add("Giá bán phải lớn hơn 0.");

            return errors;
        }

        public List<string> ValidateModelState(object model)
        {
            var errors = new List<string>();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                errors.AddRange(results.Select(r => r.ErrorMessage ?? "Lỗi validation không xác định."));
            }

            return errors;
        }
    }
}
