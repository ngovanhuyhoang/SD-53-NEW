using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using QuanApi.Data;
using QuanApi.Dtos;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLyDonHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public QuanLyDonHangController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        // GET: Admin/QuanLyDonHang
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("HoaDons");
                if (response.IsSuccessStatusCode)
                {
                    var hoaDons = await response.Content.ReadFromJsonAsync<List<HoaDon>>();
                    return View(hoaDons ?? new List<HoaDon>());
                }
                return View(new List<HoaDon>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách đơn hàng: {ex.Message}";
                return View(new List<HoaDon>());
            }
        }

        // GET: Admin/QuanLyDonHang/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"HoaDons/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
                    if (hoaDon != null)
                    {
                        return View(hoaDon);
                    }
                }
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải chi tiết đơn hàng: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/QuanLyDonHang/CapNhatTrangThai/{id}
        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(Guid id, string trangThaiMoi)
        {
            try
            {
                var hoaDon = new
                {
                    trangThai = trangThaiMoi,
                    nguoiCapNhat = User.Identity?.Name ?? "Admin",
                    lanCapNhatCuoi = DateTime.UtcNow
                };

                var response = await _httpClient.PutAsJsonAsync($"HoaDons/{id}/trangthai", hoaDon);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn hàng thành '{trangThaiMoi}'";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật trạng thái: {error}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/QuanLyDonHang/XacNhanDonHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanDonHang(Guid id)
        {
            return await CapNhatTrangThai(id, "Chờ giao hàng");
        }

        // POST: Admin/QuanLyDonHang/XacNhanLayHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanLayHang(Guid id)
        {
            return await CapNhatTrangThai(id, "Đang vận chuyển");
        }

        // POST: Admin/QuanLyDonHang/XacNhanDangVanChuyen/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanDangVanChuyen(Guid id)
        {
            return await CapNhatTrangThai(id, "Đã giao hàng");
        }

        // POST: Admin/QuanLyDonHang/XacNhanDaGiaoHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanDaGiaoHang(Guid id)
        {
            return await CapNhatTrangThai(id, "Thành công");
        }

        // POST: Admin/QuanLyDonHang/HoanThanh/{id}
        [HttpPost]
        public async Task<IActionResult> HoanThanh(Guid id)
        {
            return await CapNhatTrangThai(id, "Hoàn thành");
        }
    }
} 