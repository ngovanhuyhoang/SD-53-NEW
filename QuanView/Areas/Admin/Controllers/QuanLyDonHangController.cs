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
        public async Task<IActionResult> Index(string trangThai, string tuNgay, string denNgay, string loaiDonHang, string khachHang, string maDonHang)
        {
            try
            {
                var response = await _httpClient.GetAsync("HoaDons");
                if (response.IsSuccessStatusCode)
                {
                    var hoaDons = await response.Content.ReadFromJsonAsync<List<HoaDon>>();
                    var filteredHoaDons = hoaDons ?? new List<HoaDon>();

                    // Lọc theo trạng thái
                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.TrangThai == trangThai).ToList();
                    }

                    // Lọc theo khoảng thời gian
                    if (!string.IsNullOrEmpty(tuNgay) && DateTime.TryParse(tuNgay, out var tuNgayDate))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.NgayTao.Date >= tuNgayDate.Date).ToList();
                    }

                    if (!string.IsNullOrEmpty(denNgay) && DateTime.TryParse(denNgay, out var denNgayDate))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.NgayTao.Date <= denNgayDate.Date).ToList();
                    }

                    // Lọc theo loại đơn hàng
                    if (!string.IsNullOrEmpty(loaiDonHang))
                    {
                        if (loaiDonHang == "online")
                        {
                            filteredHoaDons = filteredHoaDons.Where(h => !string.IsNullOrEmpty(h.DiaChiGiaoHang)).ToList();
                        }
                        else if (loaiDonHang == "taiquay")
                        {
                            filteredHoaDons = filteredHoaDons.Where(h => string.IsNullOrEmpty(h.DiaChiGiaoHang)).ToList();
                        }
                    }

                    // Lọc theo khách hàng
                    if (!string.IsNullOrEmpty(khachHang))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => 
                            (h.KhachHang != null && h.KhachHang.TenKhachHang.Contains(khachHang, StringComparison.OrdinalIgnoreCase)) ||
                            (h.TenNguoiNhan != null && h.TenNguoiNhan.Contains(khachHang, StringComparison.OrdinalIgnoreCase)) ||
                            (h.SoDienThoaiNguoiNhan != null && h.SoDienThoaiNguoiNhan.Contains(khachHang))
                        ).ToList();
                    }

                    // Lọc theo mã đơn hàng
                    if (!string.IsNullOrEmpty(maDonHang))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.MaHoaDon.Contains(maDonHang, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    return View(filteredHoaDons);
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
            return await CapNhatTrangThai(id, "Đã xác nhận");
        }

        // POST: Admin/QuanLyDonHang/XacNhanLayHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanLayHang(Guid id)
        {
            // Lấy trạng thái hiện tại để xác định trạng thái tiếp theo
            var response = await _httpClient.GetAsync($"HoaDons/{id}");
            if (response.IsSuccessStatusCode)
            {
                var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
                if (hoaDon != null)
                {
                    string trangThaiMoi = hoaDon.TrangThai switch
                    {
                        "Đã xác nhận" => "Chờ lấy hàng",
                        "Chờ lấy hàng" => "Đã lấy hàng",
                        _ => "Đã lấy hàng"
                    };
                    return await CapNhatTrangThai(id, trangThaiMoi);
                }
            }
            return await CapNhatTrangThai(id, "Đã lấy hàng");
        }

        // POST: Admin/QuanLyDonHang/XacNhanGiaoHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanGiaoHang(Guid id)
        {
            // Lấy trạng thái hiện tại để xác định trạng thái tiếp theo
            var response = await _httpClient.GetAsync($"HoaDons/{id}");
            if (response.IsSuccessStatusCode)
            {
                var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
                if (hoaDon != null)
                {
                    string trangThaiMoi = hoaDon.TrangThai switch
                    {
                        "Đã lấy hàng" => "Chờ giao hàng",
                        "Chờ giao hàng" => "Đang giao hàng",
                        _ => "Đang giao hàng"
                    };
                    return await CapNhatTrangThai(id, trangThaiMoi);
                }
            }
            return await CapNhatTrangThai(id, "Đang giao hàng");
        }

        // POST: Admin/QuanLyDonHang/XacNhanDaGiaoHang/{id}
        [HttpPost]
        public async Task<IActionResult> XacNhanDaGiaoHang(Guid id)
        {
            // Lấy trạng thái hiện tại để xác định trạng thái tiếp theo
            var response = await _httpClient.GetAsync($"HoaDons/{id}");
            if (response.IsSuccessStatusCode)
            {
                var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
                if (hoaDon != null)
                {
                    string trangThaiMoi = hoaDon.TrangThai switch
                    {
                        "Đang giao hàng" => "Đã giao",
                        "Đã giao" => "Giao hàng thành công",
                        _ => "Giao hàng thành công"
                    };
                    return await CapNhatTrangThai(id, trangThaiMoi);
                }
            }
            return await CapNhatTrangThai(id, "Giao hàng thành công");
        }
    }
} 