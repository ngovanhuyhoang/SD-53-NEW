using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using QuanView.Controllers;

namespace QuanView.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin giỏ hàng từ session
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
                                TenSanPham = spct.TenSanPham,
                                AnhSanPhams = new List<AnhSanPham> {
                                    new AnhSanPham { UrlAnh = spct.AnhDaiDien ?? "/img/default-product.jpg", LaAnhChinh = true }
                                }
                            },
                            GiaBan = spct.price
                        };
                    }
                }
            }

            ViewBag.CartItems = cart;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] CheckoutDto checkoutData)
        {
            try
            {
                // Lấy giỏ hàng từ session
                var cart = HttpContext.Session.GetObjectFromJson<List<QuanApi.Data.ChiTietGioHang>>("Cart") ?? new List<QuanApi.Data.ChiTietGioHang>();
                
                if (!cart.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Tạo danh sách chi tiết hóa đơn
                var chiTietHoaDons = cart.Select(item => new
                {
                    idSanPhamChiTiet = item.IDSanPhamChiTiet,
                    soLuong = item.SoLuong,
                    donGia = item.GiaBan,
                    thanhTien = item.GiaBan * item.SoLuong
                }).ToList();

                // Tạo dữ liệu hóa đơn
                var hoaDonData = new
                {
                    khachHangId = checkoutData.KhachHangId,
                    nhanVienId = (Guid?)null,
                    phieuGiamGiaId = checkoutData.PhieuGiamGiaId,
                    phuongThucThanhToanId = checkoutData.PhuongThucThanhToanId,
                    tongTien = checkoutData.TongTien,
                    tienGiam = checkoutData.TienGiam,
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
                    HttpContext.Session.Remove("Cart"); // Xóa giỏ hàng ngay tại server
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
    }
} 