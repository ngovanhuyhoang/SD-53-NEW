using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using static QuanApi.Repository.HoaDonRepository;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanHangTaiQuayController : Controller
    {
        private readonly HttpClient _httpClient;

        public BanHangTaiQuayController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/BanHangTaiQuays");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy danh sách hoá đơn.";
                return View((HoaDon?)null);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                ViewBag.Error = "Dữ liệu từ API rỗng.";
                return View((HoaDon?)null);
            }

            var hoaDons = JsonSerializer.Deserialize<List<HoaDon>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var hoaDonHienTai = hoaDons?.FirstOrDefault(h => h.TrangThai == TrangThaiHoaDon.ChuaThanhToan.ToString());

            return View(hoaDonHienTai);
        }

        [HttpPost]
        public async Task<IActionResult> TaoHoaDon()
        {
            var response = await _httpClient.PostAsync("/api/BanHangTaiQuays/TaoHoaDon", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Không thể tạo hóa đơn. " + error;
                return RedirectToAction("Index");
            }

            var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
            return RedirectToAction("ThemSanPham", new { id = hoaDon?.IDHoaDon });
        }




        [HttpPost]
        public async Task<IActionResult> ThemSanPham(Guid idHoaDon, Guid idSanPhamChiTiet, int soLuong, decimal donGia)
        {
            var chiTiet = new ChiTietHoaDon
            {
                IDSanPhamChiTiet = idSanPhamChiTiet,
                SoLuong = soLuong,
                DonGia = donGia,
                ThanhTien = donGia * soLuong
            };

            var response = await _httpClient.PostAsJsonAsync($"api/BanHangTaiQuays/ThemSanPham?hoaDonId={idHoaDon}", chiTiet);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Không thể thêm sản phẩm");
        }

        [HttpPost]
        public async Task<IActionResult> XoaSanPham(Guid idHoaDon, Guid idChiTiet)
        {
            var response = await _httpClient.DeleteAsync($"api/BanHangTaiQuays/XoaSanPham?hoaDonId={idHoaDon}&chiTietId={idChiTiet}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Không thể xoá sản phẩm");
        }

        [HttpPost]
        public async Task<IActionResult> ChonKhachHang(Guid idHoaDon, Guid idKhachHang)
        {
            var response = await _httpClient.PutAsync($"api/BanHangTaiQuays/CapNhatKhachHang?hoaDonId={idHoaDon}&khachHangId={idKhachHang}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Không thể gán khách hàng");
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToan(Guid idHoaDon)
        {
            var response = await _httpClient.PutAsync($"api/BanHangTaiQuays/ThanhToan?hoaDonId={idHoaDon}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return BadRequest("Không thể thanh toán");
        }
    }
}
