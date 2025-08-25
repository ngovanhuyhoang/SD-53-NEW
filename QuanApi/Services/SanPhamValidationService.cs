using QuanApi.Data;
using QuanApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace QuanApi.Services
{
    public interface ISanPhamValidationService
    {
        Task<ValidationResult> ValidateSanPhamChiTietAsync(SanPhamChiTietDto dto);
        Task<ValidationResult> ValidateQuantityUpdateAsync(Guid sanPhamChiTietId, int newQuantity);
        Task<ValidationResult> ValidatePriceUpdateAsync(Guid sanPhamChiTietId, decimal newPrice);
    }

    public class SanPhamValidationService : ISanPhamValidationService
    {
        private readonly BanQuanAu1DbContext _context;

        public SanPhamValidationService(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public async Task<ValidationResult> ValidateSanPhamChiTietAsync(SanPhamChiTietDto dto)
        {
            var result = new ValidationResult();

            // Validate quantity
            if (dto.SoLuong < 0)
            {
                result.AddError("SoLuong", "Số lượng không thể âm");
            }

            if (dto.SoLuong > 10000)
            {
                result.AddError("SoLuong", "Số lượng không thể vượt quá 10,000");
            }

            // Validate price
            if (dto.GiaBan <= 0)
            {
                result.AddError("GiaBan", "Giá bán phải lớn hơn 0");
            }

            if (dto.GiaBan > 100000000) // 100 million VND
            {
                result.AddError("GiaBan", "Giá bán không thể vượt quá 100,000,000 VND");
            }

            // Check if product exists
            if (dto.IdSanPham != Guid.Empty)
            {
                var productExists = await _context.SanPhams
                    .AnyAsync(sp => sp.IDSanPham == dto.IdSanPham && sp.TrangThai);
                
                if (!productExists)
                {
                    result.AddError("IdSanPham", "Sản phẩm không tồn tại hoặc đã bị vô hiệu hóa");
                }
            }

            // Check if size exists
            if (dto.IdKichCo != Guid.Empty)
            {
                var sizeExists = await _context.KichCos
                    .AnyAsync(kc => kc.IDKichCo == dto.IdKichCo && kc.TrangThai);
                
                if (!sizeExists)
                {
                    result.AddError("IdKichCo", "Kích cỡ không tồn tại hoặc đã bị vô hiệu hóa");
                }
            }

            // Check if color exists
            if (dto.IdMauSac != Guid.Empty)
            {
                var colorExists = await _context.MauSacs
                    .AnyAsync(ms => ms.IDMauSac == dto.IdMauSac && ms.TrangThai);
                
                if (!colorExists)
                {
                    result.AddError("IdMauSac", "Màu sắc không tồn tại hoặc đã bị vô hiệu hóa");
                }
            }

            // Check for duplicate product variant
            var duplicateExists = await _context.SanPhamChiTiets
                .AnyAsync(spct => spct.IDSanPham == dto.IdSanPham 
                    && spct.IDKichCo == dto.IdKichCo 
                    && spct.IDMauSac == dto.IdMauSac
                    && spct.IDHoaTiet == dto.IdHoaTiet
                    && spct.IDSanPhamChiTiet != dto.IdSanPhamChiTiet
                    && spct.TrangThai);

            if (duplicateExists)
            {
                result.AddError("Duplicate", "Biến thể sản phẩm này đã tồn tại (cùng sản phẩm, kích cỡ, màu sắc và họa tiết)");
            }

            return result;
        }

        public async Task<ValidationResult> ValidateQuantityUpdateAsync(Guid sanPhamChiTietId, int newQuantity)
        {
            var result = new ValidationResult();

            if (newQuantity < 0)
            {
                result.AddError("SoLuong", "Số lượng không thể âm");
                return result;
            }

            if (newQuantity > 10000)
            {
                result.AddError("SoLuong", "Số lượng không thể vượt quá 10,000");
                return result;
            }

            // Check if there are pending orders that would be affected
            var pendingOrderQuantity = await _context.ChiTietHoaDons
                .Where(cth => cth.IDSanPhamChiTiet == sanPhamChiTietId 
                    && cth.HoaDon.TrangThai == 0) // Assuming 0 is pending status
                .SumAsync(cth => cth.SoLuong);

            if (newQuantity < pendingOrderQuantity)
            {
                result.AddError("SoLuong", $"Không thể giảm số lượng xuống dưới {pendingOrderQuantity} do có đơn hàng đang chờ xử lý");
            }

            return result;
        }

        public async Task<ValidationResult> ValidatePriceUpdateAsync(Guid sanPhamChiTietId, decimal newPrice)
        {
            var result = new ValidationResult();

            if (newPrice <= 0)
            {
                result.AddError("GiaBan", "Giá bán phải lớn hơn 0");
                return result;
            }

            if (newPrice > 100000000)
            {
                result.AddError("GiaBan", "Giá bán không thể vượt quá 100,000,000 VND");
                return result;
            }

            // Get current price to check for significant changes
            var currentProduct = await _context.SanPhamChiTiets
                .FindAsync(sanPhamChiTietId);

            if (currentProduct != null)
            {
                var priceChangePercent = Math.Abs((newPrice - currentProduct.GiaBan) / currentProduct.GiaBan) * 100;
                
                if (priceChangePercent > 50) // More than 50% change
                {
                    result.AddWarning("GiaBan", $"Thay đổi giá lớn: {priceChangePercent:F1}% so với giá hiện tại ({currentProduct.GiaBan:N0} VND)");
                }
            }

            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Warnings { get; } = new Dictionary<string, List<string>>();

        public void AddError(string field, string message)
        {
            if (!Errors.ContainsKey(field))
                Errors[field] = new List<string>();
            
            Errors[field].Add(message);
        }

        public void AddWarning(string field, string message)
        {
            if (!Warnings.ContainsKey(field))
                Warnings[field] = new List<string>();
            
            Warnings[field].Add(message);
        }

        public string GetErrorMessages()
        {
            var messages = new List<string>();
            foreach (var error in Errors)
            {
                messages.AddRange(error.Value.Select(msg => $"{error.Key}: {msg}"));
            }
            return string.Join("; ", messages);
        }

        public string GetWarningMessages()
        {
            var messages = new List<string>();
            foreach (var warning in Warnings)
            {
                messages.AddRange(warning.Value.Select(msg => $"{warning.Key}: {msg}"));
            }
            return string.Join("; ", messages);
        }
    }
}