using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using QuanView.Controllers;
using Microsoft.AspNetCore.Http;
using QuanApi.Dtos;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using QuanView.Models;

namespace QuanView.Controllers
{
    // SessionExtensions để xử lý session
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }

    //[Authorize]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        private bool ValidateVietnamesePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            phoneNumber = phoneNumber.Trim();

            string pattern = @"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$";
            
            return Regex.IsMatch(phoneNumber, pattern);
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            string cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");
            
            if (cleaned.StartsWith("+84"))
            {
                cleaned = "0" + cleaned.Substring(3);
            }
            
            if (cleaned.StartsWith("84"))
            {
                cleaned = "0" + cleaned.Substring(2);
            }
            
            return cleaned;
        }

        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();

            if (!cart.Any())
            {
                return RedirectToAction("Index", "GioHang");
            }

            // Cập nhật thông tin sản phẩm cho từng item
            foreach (var item in cart)
            {
                var responseSpct = await _httpClient.GetAsync($"SanPhamChiTiets/{item.IDSanPhamChiTiet}");
                if (responseSpct.IsSuccessStatusCode)
                {
                    var spct = await responseSpct.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
                    if (spct != null)
                    {
                        item.GiaBan = spct.price;
                        item.SanPhamChiTiet = new SanPhamChiTiet
                        {
                            SanPham = new SanPham
                            {
                                TenSanPham = spct.TenSanPham
                            },
                            GiaBan = spct.price,
                            AnhSanPhams = new List<AnhSanPham>
                            {
                                new AnhSanPham
                                {
                                    UrlAnh = spct.AnhDaiDien ?? "/img/default-product.jpg",
                                    LaAnhChinh = true
                                }
                            }
                        };
                    }
                }
            }

            // Tính tổng tiền
            var tongTien = cart.Sum(item => item.GiaBan * item.SoLuong);
            
            ViewBag.CartItems = cart;
            ViewBag.TongTien = tongTien;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] CheckoutDto checkoutData)
        {
            try
            {
                // Validate số điện thoại người nhận
                if (string.IsNullOrWhiteSpace(checkoutData.SoDienThoaiNguoiNhan))
                {
                    return Json(new { success = false, message = "Số điện thoại người nhận không được để trống" });
                }

                string formattedPhone = FormatPhoneNumber(checkoutData.SoDienThoaiNguoiNhan);
                
                if (!ValidateVietnamesePhoneNumber(formattedPhone))
                {
                    return Json(new { success = false, message = "Số điện thoại người nhận không hợp lệ" });
                }

                // Cập nhật số điện thoại đã được format
                checkoutData.SoDienThoaiNguoiNhan = formattedPhone;

                // Lấy giỏ hàng từ session
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();

                if (!cart.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Cập nhật lại thông tin sản phẩm và giá trước khi tạo hóa đơn
                foreach (var item in cart)
                {
                    var responseSpct = await _httpClient.GetAsync($"SanPhamChiTiets/{item.IDSanPhamChiTiet}");
                    if (responseSpct.IsSuccessStatusCode)
                    {
                        var spct = await responseSpct.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
                        if (spct != null && spct.price > 0)
                        {
                            item.GiaBan = spct.price;
                        }
                        else
                        {
                            return Json(new { success = false, message = $"Không thể lấy giá sản phẩm cho ID: {item.IDSanPhamChiTiet}" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Không thể lấy thông tin sản phẩm cho ID: {item.IDSanPhamChiTiet}" });
                    }
                }

                // Tạo danh sách chi tiết hóa đơn với giá đã được cập nhật
                var chiTietHoaDons = cart.Select(item => new
                {
                    idSanPhamChiTiet = item.IDSanPhamChiTiet,
                    soLuong = item.SoLuong,
                    donGia = item.GiaBan,
                    thanhTien = Math.Round(item.GiaBan * item.SoLuong, 2)
                }).ToList();

                // Log để debug
                Console.WriteLine($"Số lượng chi tiết hóa đơn: {chiTietHoaDons.Count()}");
                foreach (var ct in chiTietHoaDons)
                {
                    Console.WriteLine($"SPCT ID: {ct.idSanPhamChiTiet}, SL: {ct.soLuong}, Đơn giá: {ct.donGia}, Thành tiền: {ct.thanhTien}");
                }

                // Lấy KhachHangId từ claims nếu user đã đăng nhập
                Guid? khachHangId = null;
                var customerIdClaim = User.FindFirst("custom:id_khachhang");
                if (customerIdClaim != null && Guid.TryParse(customerIdClaim.Value, out var parsedCustomerId))
                {
                    khachHangId = parsedCustomerId;
                }
                
                // Xử lý phiếu giảm giá nếu có
                Guid? phieuGiamGiaId = null;
                if (!string.IsNullOrEmpty(checkoutData.MaGiamGia))
                {
                    try
                    {
                        // Tìm phiếu giảm giá theo mã
                        var responsePhieu = await _httpClient.GetAsync($"PhieuGiamGias/kiem-tra?code={Uri.EscapeDataString(checkoutData.MaGiamGia)}");
                        if (responsePhieu.IsSuccessStatusCode)
                        {
                            var phieuResult = await responsePhieu.Content.ReadFromJsonAsync<PhieuGiamGiaResponse>();
                            if (phieuResult != null && phieuResult.Success && phieuResult.IdPhieuGiamGia.HasValue)
                            {
                                phieuGiamGiaId = phieuResult.IdPhieuGiamGia.Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi xử lý phiếu giảm giá: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"KhachHangId from claims: {khachHangId}");
                Console.WriteLine($"PhieuGiamGiaId: {phieuGiamGiaId}");
                Console.WriteLine($"MaGiamGia: {checkoutData.MaGiamGia}");
                
                // Tạo dữ liệu hóa đơn
                var hoaDonData = new
                {
                    khachHangId = khachHangId, // Sử dụng từ claims thay vì checkoutData
                    nhanVienId = (Guid?)null,
                    phieuGiamGiaId = phieuGiamGiaId, // Sử dụng từ API thay vì checkoutData
                    phuongThucThanhToanId = checkoutData.PhuongThucThanhToanId,
                    tongTien = checkoutData.TongTien,
                    tienGiam = checkoutData.TienGiam,
                    phiVanChuyen = checkoutData.PhiVanChuyen, // Thêm phí vận chuyển
                    tenNguoiNhan = checkoutData.TenNguoiNhan,
                    soDienThoaiNguoiNhan = checkoutData.SoDienThoaiNguoiNhan,
                    diaChiGiaoHang = checkoutData.DiaChiGiaoHang,
                    ghiChu = checkoutData.GhiChu,
                    chiTietHoaDons = chiTietHoaDons
                };

                // Xử lý đặt hàng
                var response = await _httpClient.PostAsJsonAsync("HoaDons", hoaDonData);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("Cart"); 
                    return Json(new { success = true, message = "Đặt hàng thành công!" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Lỗi: {error}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var response = await _httpClient.GetAsync("PhuongThucThanhToans");
                if (response.IsSuccessStatusCode)
                {
                    var methods = await response.Content.ReadFromJsonAsync<List<PhuongThucThanhToan>>();
                    return Json(methods);
                }
                return Json(new List<PhuongThucThanhToan>());
            }
            catch
            {
                return Json(new List<PhuongThucThanhToan>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> KiemTraMaGiamGia(string code)
        {
            try
            {
                var response = await _httpClient.GetAsync($"PhieuGiamGias/kiem-tra?code={Uri.EscapeDataString(code)}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<object>();
                    return Json(result);
                }
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ" });
            }
            catch
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi kiểm tra mã giảm giá" });
            }
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentCustomer()
        {
            try
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                var response = await _httpClient.GetAsync($"KhachHang/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var customer = await response.Content.ReadFromJsonAsync<object>();
                    return Json(new { success = true, customer = customer });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể lấy thông tin khách hàng" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerAddresses()
        {
            try
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                var response = await _httpClient.GetAsync($"KhachHang/{customerId}/addresses");
                if (response.IsSuccessStatusCode)
                {
                    var addresses = await response.Content.ReadFromJsonAsync<List<AddressDto>>();
                    return Json(new { success = true, addresses = addresses ?? new List<AddressDto>() });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể lấy danh sách địa chỉ" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // Thêm action để lấy địa chỉ mặc định của khách hàng
        [HttpGet]
        public async Task<IActionResult> GetDefaultAddress()
        {
            try
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                // Sử dụng API endpoint mới để lấy địa chỉ mặc định
                var response = await _httpClient.GetAsync($"KhachHang/{customerId}/default-address");
                if (response.IsSuccessStatusCode)
                {
                    var address = await response.Content.ReadFromJsonAsync<AddressDto>();
                    return Json(new { success = true, address = address });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Json(new { success = false, message = "Không tìm thấy địa chỉ mặc định" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể lấy địa chỉ mặc định" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // Thêm action để lấy dữ liệu giỏ hàng cho checkout
        [HttpGet]
        public async Task<IActionResult> GetCheckoutCart()
        {
            try
            {
                var customerIdClaim = User.FindFirst("custom:id_khachhang");
                if (customerIdClaim == null || !Guid.TryParse(customerIdClaim.Value, out var customerId))
                {
                    // Khách hàng - trả về giỏ hàng session với thông tin sản phẩm đầy đủ
                    var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                    
                    // Cập nhật thông tin sản phẩm cho từng item (giống như trong GioHangController)
                    foreach (var item in cart)
                    {
                        var responseSpct = await _httpClient.GetAsync($"SanPhamChiTiets/{item.IDSanPhamChiTiet}");
                        if (responseSpct.IsSuccessStatusCode)
                        {
                            var spct = await responseSpct.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
                            if (spct != null)
                            {
                                item.GiaBan = spct.price;
                                item.SanPhamChiTiet = new SanPhamChiTiet
                                {
                                    SanPham = new SanPham
                                    {
                                        TenSanPham = spct.TenSanPham
                                    },
                                    GiaBan = spct.price,
                                    AnhSanPhams = new List<AnhSanPham>
                                    {
                                        new AnhSanPham
                                        {
                                            UrlAnh = spct.AnhDaiDien ?? "/img/default-product.jpg",
                                            LaAnhChinh = true
                                        }
                                    }
                                };
                            }
                        }
                    }
                    
                    return Json(new { success = true, chiTietGioHangs = cart });
                }

                // Người dùng đã đăng nhập - trả về giỏ hàng từ database
                var response = await _httpClient.GetAsync($"GioHangs/getbyuser?iduser={customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var gioHang = await response.Content.ReadFromJsonAsync<QuanApi.Data.GioHang>();
                    var chiTietGioHangs = gioHang?.ChiTietGioHangs ?? new List<QuanApi.Data.ChiTietGioHang>();
                    
                    // Cập nhật thông tin sản phẩm cho từng item
                    foreach (var item in chiTietGioHangs)
                    {
                        var responseSpct = await _httpClient.GetAsync($"SanPhamChiTiets/{item.IDSanPhamChiTiet}");
                        if (responseSpct.IsSuccessStatusCode)
                        {
                            var spct = await responseSpct.Content.ReadFromJsonAsync<SanPhamChiTietDto>();
                            if (spct != null)
                            {
                                item.GiaBan = spct.price;
                                item.SanPhamChiTiet = new SanPhamChiTiet
                                {
                                    SanPham = new SanPham
                                    {
                                        TenSanPham = spct.TenSanPham
                                    },
                                    GiaBan = spct.price,
                                    AnhSanPhams = new List<AnhSanPham>
                                    {
                                        new AnhSanPham
                                        {
                                            UrlAnh = spct.AnhDaiDien ?? "/img/default-product.jpg",
                                            LaAnhChinh = true
                                        }
                                    }
                                };
                            }
                        }
                    }
                    
                    return Json(new { success = true, chiTietGioHangs = chiTietGioHangs });
                }

                return Json(new { success = true, chiTietGioHangs = new List<QuanApi.Data.ChiTietGioHang>() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // Test endpoint để kiểm tra session cart
        [HttpGet]
        public IActionResult TestSessionCart()
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart");
                return Json(new { 
                    success = true, 
                    cartCount = cart?.Count ?? 0,
                    cart = cart,
                    sessionKeys = HttpContext.Session.Keys.ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerVouchers()
        {
            try
            {
                // Lấy phiếu giảm giá công khai từ KhachHangPhieuGiamsController
                var response = await _httpClient.GetAsync("KhachHangPhieuGiam/phieu-giam-gia-cong-khai");
                if (response.IsSuccessStatusCode)
                {
                    var vouchers = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(vouchers);
                }
                return StatusCode((int)response.StatusCode, "Lỗi khi lấy danh sách phiếu giảm giá công khai");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerPersonalVouchers()
        {
            try
            {
                // Lấy ID khách hàng từ claims
                var customerIdClaim = User.FindFirst("custom:id_khachhang")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out Guid customerId))
                {
                    return Ok(new List<object>()); // Trả về danh sách rỗng nếu chưa đăng nhập
                }

                // Lấy phiếu giảm giá riêng của khách hàng
                var response = await _httpClient.GetAsync($"KhachHangPhieuGiam/phieu-giam-gia-cua-khach-hang/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    var vouchers = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(vouchers);
                }
                return StatusCode((int)response.StatusCode, "Lỗi khi lấy danh sách phiếu giảm giá của khách hàng");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        // Tạo địa chỉ mới cho khách hàng
        [HttpPost]
        public async Task<IActionResult> TaoDiaChi([FromBody] TaoDiaChiDto dto)
        {
            try
            {
                // Kiểm tra user có đăng nhập không
                var customerIdClaim = User.FindFirst("custom:id_khachhang")?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out Guid customerId))
                {
                    return BadRequest("Vui lòng đăng nhập để lưu địa chỉ!");
                }

                // Gán ID khách hàng từ claims
                dto.IDKhachHang = customerId;

                // Gọi API tạo địa chỉ
                var response = await _httpClient.PostAsJsonAsync("BanHangTaiQuay/tao-dia-chi", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(result);
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }

    public class CheckoutDto
    {
        public Guid? KhachHangId { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoDienThoaiNguoiNhan { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public Guid PhuongThucThanhToanId { get; set; }
        public Guid? PhieuGiamGiaId { get; set; }
        public decimal TongTien { get; set; }
        public decimal? TienGiam { get; set; }
        public string GhiChu { get; set; }
        public string MaGiamGia { get; set; } 
        public decimal PhiVanChuyen { get; set; } = 50000; 
    }

    public class PhieuGiamGiaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? IdPhieuGiamGia { get; set; }
        public decimal? GiaTriGiam { get; set; }
        public decimal? GiaTriGiamToiDa { get; set; }
        public decimal? DonToiThieu { get; set; }
    }

    public class AddressDto
    {
        public Guid IDDiaChi { get; set; }
        public string MaDiaChi { get; set; }
        public string DiaChiChiTiet { get; set; }
        public Guid IDKhachHang { get; set; }
        public bool LaMacDinh { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SdtNguoiNhan { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? LanCapNhatCuoi { get; set; }
        public string NguoiCapNhat { get; set; }
        public bool TrangThai { get; set; }
    }
}