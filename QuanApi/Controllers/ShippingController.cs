using Microsoft.AspNetCore.Mvc;
using QuanApi.Dtos;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        // Tạm thời sử dụng service inline để tránh lỗi DI
        private readonly Dictionary<string, decimal> _regionBaseFees = new()
        {
            { "Hà Nội", 20000 }, { "Hải Phòng", 25000 }, { "Quảng Ninh", 30000 },
            { "Thái Nguyên", 25000 }, { "Vĩnh Phúc", 25000 }, { "Bắc Ninh", 22000 },
            { "Hưng Yên", 25000 }, { "Hà Nam", 25000 }, { "Nam Định", 30000 },
            { "Ninh Bình", 30000 }, { "Thanh Hóa", 40000 }, { "Nghệ An", 45000 },
            { "Hà Tĩnh", 45000 }, { "Quảng Bình", 50000 }, { "Quảng Trị", 50000 },
            { "Thừa Thiên Huế", 50000 }, { "Đà Nẵng", 55000 }, { "Quảng Nam", 55000 },
            { "Quảng Ngãi", 60000 }, { "Bình Định", 60000 }, { "Phú Yên", 65000 },
            { "Khánh Hòa", 65000 }, { "Ninh Thuận", 70000 }, { "Bình Thuận", 70000 },
            { "Hồ Chí Minh", 75000 }, { "Bình Dương", 75000 }, { "Đồng Nai", 80000 },
            { "Bà Rịa - Vũng Tàu", 85000 }, { "Long An", 80000 }, { "Tiền Giang", 85000 },
            { "Bến Tre", 90000 }, { "Vĩnh Long", 90000 }, { "Trà Vinh", 95000 },
            { "Cần Thơ", 90000 }, { "Đồng Tháp", 90000 }, { "An Giang", 95000 },
            { "Kiên Giang", 100000 }, { "Cà Mau", 110000 }, { "Bạc Liêu", 105000 },
            { "Sóc Trăng", 95000 }, { "Hậu Giang", 95000 }, { "Tây Ninh", 85000 },
            { "Bình Phước", 85000 }, { "Đắk Lắk", 90000 }, { "Đắk Nông", 95000 },
            { "Lâm Đồng", 85000 }, { "Gia Lai", 95000 }, { "Kon Tum", 100000 }
        };

        private readonly Dictionary<string, decimal> _districtSurcharge = new()
        {
            { "Quận 1", -5000 }, { "Quận 3", -3000 }, { "Quận Hai Bà Trưng", -3000 },
            { "Quận Hoàn Kiếm", -5000 }, { "Quận Ba Đình", -3000 },
            { "Huyện Cần Giờ", 15000 }, { "Huyện Củ Chi", 10000 },
            { "Huyện Nhà Bè", 8000 }, { "Huyện Bình Chánh", 8000 }
        };

        [HttpPost("calculate")]
        public ActionResult<ShippingInfoDto> CalculateShipping([FromBody] CalculateShippingRequest request)
        {
            try
            {
                // Log request để debug
                Console.WriteLine($"Shipping calculation request: Province={request?.Province}, District={request?.District}, OrderValue={request?.OrderValue}");

                if (request == null)
                {
                    return BadRequest("Request không được null");
                }

                if (string.IsNullOrEmpty(request.Province))
                {
                    return BadRequest("Tỉnh/thành phố không được để trống");
                }

                if (request.OrderValue < 0)
                {
                    return BadRequest("Giá trị đơn hàng không được âm");
                }

                var shippingInfo = CalculateShippingInfo(
                    request.Province, 
                    request.District ?? "", 
                    request.OrderValue
                );

                Console.WriteLine($"Shipping calculation result: OriginalFee={shippingInfo.OriginalFee}, FinalFee={shippingInfo.FinalFee}");

                return Ok(shippingInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Shipping calculation error: {ex}");
                return StatusCode(500, $"Lỗi tính phí vận chuyển: {ex.Message}");
            }
        }

        private ShippingInfoDto CalculateShippingInfo(string province, string district, decimal orderValue)
        {
            // Lấy phí cơ bản theo tỉnh
            decimal baseFee = _regionBaseFees.GetValueOrDefault(province, 50000);

            // Áp dụng phụ phí theo quận/huyện
            decimal districtFee = _districtSurcharge.GetValueOrDefault(district, 0);

            decimal totalFee = baseFee + districtFee;
            if (totalFee < 0) totalFee = 0;

            // Áp dụng giảm giá theo giá trị đơn hàng
            decimal discount = 0;
            string discountMessage = "";

            if (orderValue >= 500000)
            {
                discount = totalFee; // Miễn phí 100%
                discountMessage = "Miễn phí vận chuyển cho đơn hàng từ 500.000đ";
            }
            else if (orderValue >= 300000)
            {
                discount = totalFee * 0.5m; // Giảm 50%
                discountMessage = "Giảm 50% phí vận chuyển cho đơn hàng từ 300.000đ";
            }
            else if (orderValue >= 200000)
            {
                discount = totalFee * 0.2m; // Giảm 20%
                discountMessage = "Giảm 20% phí vận chuyển cho đơn hàng từ 200.000đ";
            }

            decimal finalFee = totalFee - discount;
            if (finalFee < 0) finalFee = 0;

            // Thời gian giao hàng ước tính
            int estimatedDays = GetEstimatedDeliveryDays(province);

            return new ShippingInfoDto
            {
                Province = province,
                District = district,
                OriginalFee = totalFee,
                DiscountAmount = discount,
                FinalFee = finalFee,
                DiscountMessage = discountMessage,
                EstimatedDeliveryDays = estimatedDays
            };
        }

        private int GetEstimatedDeliveryDays(string province)
        {
            var majorCities = new[] { "Hồ Chí Minh", "Hà Nội", "Đà Nẵng", "Cần Thơ", "Hải Phòng" };
            
            if (majorCities.Contains(province))
            {
                return 2;
            }
            
            var nearbyProvinces = new[] { 
                "Bình Dương", "Đồng Nai", "Long An", "Bắc Ninh", "Hưng Yên", 
                "Vĩnh Phúc", "Hà Nam", "Quảng Nam", "Bình Thuận" 
            };
            
            if (nearbyProvinces.Contains(province))
            {
                return 3;
            }
            
            return 5;
        }

        [HttpGet("test")]
        public ActionResult<object> TestShipping()
        {
            try
            {
                var testResult = CalculateShippingInfo("Hà Nội", "Ba Đình", 100000);
                return Ok(new { 
                    message = "Shipping service hoạt động bình thường", 
                    testResult = testResult 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi test shipping service: {ex.Message}");
            }
        }

        [HttpGet("fee")]
        public ActionResult<decimal> GetShippingFee(
            [FromQuery] string province, 
            [FromQuery] string? district = null, 
            [FromQuery] decimal orderValue = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(province))
                {
                    return BadRequest("Tỉnh/thành phố không được để trống");
                }

                var shippingInfo = CalculateShippingInfo(province, district ?? "", orderValue);
                return Ok(shippingInfo.FinalFee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi tính phí vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("provinces")]
        public ActionResult<IEnumerable<string>> GetSupportedProvinces()
        {
            var provinces = new[]
            {
                // Miền Bắc
                "Hà Nội", "Hải Phòng", "Quảng Ninh", "Thái Nguyên", "Vĩnh Phúc", 
                "Bắc Ninh", "Hưng Yên", "Hà Nam", "Nam Định", "Ninh Bình",
                
                // Miền Trung
                "Thanh Hóa", "Nghệ An", "Hà Tĩnh", "Quảng Bình", "Quảng Trị", 
                "Thừa Thiên Huế", "Đà Nẵng", "Quảng Nam", "Quảng Ngãi", "Bình Định", 
                "Phú Yên", "Khánh Hòa", "Ninh Thuận", "Bình Thuận",
                
                // Miền Nam
                "Hồ Chí Minh", "Bình Dương", "Đồng Nai", "Bà Rịa - Vũng Tàu", 
                "Long An", "Tiền Giang", "Bến Tre", "Vĩnh Long", "Trà Vinh", 
                "Cần Thơ", "Đồng Tháp", "An Giang", "Kiên Giang", "Cà Mau", 
                "Bạc Liêu", "Sóc Trăng", "Hậu Giang", "Tây Ninh", "Bình Phước", 
                "Đắk Lắk", "Đắk Nông", "Lâm Đồng", "Gia Lai", "Kon Tum"
            };

            return Ok(provinces.OrderBy(p => p));
        }

        [HttpGet("districts")]
        public ActionResult<IEnumerable<string>> GetDistricts([FromQuery] string province)
        {
            if (string.IsNullOrEmpty(province))
            {
                return BadRequest("Tỉnh/thành phố không được để trống");
            }

            var districts = GetDistrictsByProvince(province);
            return Ok(districts.OrderBy(d => d));
        }

        [HttpGet("wards")]
        public ActionResult<IEnumerable<string>> GetWards([FromQuery] string province, [FromQuery] string district)
        {
            if (string.IsNullOrEmpty(province) || string.IsNullOrEmpty(district))
            {
                return BadRequest("Tỉnh/thành phố và quận/huyện không được để trống");
            }

            var wards = GetWardsByDistrict(province, district);
            return Ok(wards.OrderBy(w => w));
        }

        private IEnumerable<string> GetDistrictsByProvince(string province)
        {
            return province switch
            {
                "Hà Nội" => new[] { "Ba Đình", "Hoàn Kiếm", "Tây Hồ", "Long Biên", "Cầu Giấy", "Đống Đa", "Hai Bà Trưng", "Hoàng Mai", "Thanh Xuân", "Sóc Sơn", "Đông Anh", "Gia Lâm", "Nam Từ Liêm", "Bắc Từ Liêm", "Mê Linh", "Hà Đông", "Sơn Tây", "Ba Vì", "Phúc Thọ", "Đan Phượng", "Hoài Đức", "Quốc Oai", "Thạch Thất", "Chương Mỹ", "Thanh Oai", "Thường Tín", "Phú Xuyên", "Ứng Hòa", "Mỹ Đức" },
                "Hồ Chí Minh" => new[] { "Quận 1", "Quận 2", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8", "Quận 9", "Quận 10", "Quận 11", "Quận 12", "Bình Thạnh", "Gò Vấp", "Phú Nhuận", "Tân Bình", "Tân Phú", "Thủ Đức", "Bình Tân", "Hóc Môn", "Củ Chi", "Nhà Bè", "Cần Giờ" },
                "Đà Nẵng" => new[] { "Hải Châu", "Thanh Khê", "Sơn Trà", "Ngũ Hành Sơn", "Liên Chiểu", "Cẩm Lệ", "Hòa Vang", "Hoàng Sa" },
                "Hải Phòng" => new[] { "Hồng Bàng", "Ngô Quyền", "Lê Chân", "Hải An", "Kiến An", "Đồ Sơn", "Dương Kinh", "Thuỷ Nguyên", "An Dương", "An Lão", "Kiến Thuỵ", "Tiên Lãng", "Vĩnh Bảo", "Cát Hải", "Bạch Long Vĩ" },
                "Cần Thơ" => new[] { "Ninh Kiều", "Ô Môn", "Bình Thuỷ", "Cái Răng", "Thốt Nốt", "Vĩnh Thạnh", "Cờ Đỏ", "Phong Điền", "Thới Lai" },
                _ => new[] { "Trung tâm", "Ngoại thành" }
            };
        }

        private IEnumerable<string> GetWardsByDistrict(string province, string district)
        {
            // Simplified ward data - in real application, this would come from a comprehensive database
            return district switch
            {
                "Ba Đình" => new[] { "Phúc Xá", "Trúc Bạch", "Vĩnh Phúc", "Cống Vị", "Liễu Giai", "Nguyễn Trung Trực", "Quán Thánh", "Ngọc Hà", "Điện Biên", "Đội Cấn", "Ngọc Khánh", "Kim Mã", "Giảng Võ", "Thành Công" },
                "Hoàn Kiếm" => new[] { "Phúc Tấn", "Đồng Xuân", "Hàng Mã", "Hàng Buồm", "Hàng Đào", "Hàng Bồ", "Cửa Đông", "Lý Thái Tổ", "Hàng Bạc", "Hàng Gai", "Chương Dương Độ", "Hàng Trống", "Cửa Nam", "Hàng Bông", "Tràng Tiền", "Trần Hưng Đạo", "Phan Chu Trinh" },
                "Quận 1" => new[] { "Tân Định", "Đa Kao", "Bến Nghé", "Bến Thành", "Nguyễn Thái Bình", "Phạm Ngũ Lão", "Cầu Ông Lãnh", "Cô Giang", "Nguyễn Cư Trinh", "Cầu Kho" },
                "Quận 2" => new[] { "Thảo Điền", "An Phú", "Bình An", "Bình Trưng Đông", "Bình Trưng Tây", "Bình Khánh", "An Lợi Đông", "Thạnh Mỹ Lợi", "Cát Lái", "Thủ Thiêm" },
                _ => new[] { "Phường 1", "Phường 2", "Phường 3", "Phường 4", "Phường 5" }
            };
        }

        [HttpGet("discount-info")]
        public ActionResult<object> GetDiscountInfo()
        {
            var discountTiers = new[]
            {
                new { MinOrderValue = 500000, DiscountPercent = 100, Description = "Miễn phí vận chuyển cho đơn hàng từ 500.000đ" },
                new { MinOrderValue = 300000, DiscountPercent = 50, Description = "Giảm 50% phí vận chuyển cho đơn hàng từ 300.000đ" },
                new { MinOrderValue = 200000, DiscountPercent = 20, Description = "Giảm 20% phí vận chuyển cho đơn hàng từ 200.000đ" }
            };

            return Ok(discountTiers);
        }
    }
}
