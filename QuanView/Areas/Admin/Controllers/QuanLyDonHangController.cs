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
        public async Task<IActionResult> Index(string trangThai, string tuNgay, string denNgay, string loaiDonHang, string khachHang, string maDonHang, int page = 1)
        {
            try
            {
                // Xây dựng URL với các tham số
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(trangThai)) queryParams.Add($"trangThai={Uri.EscapeDataString(trangThai)}");
                if (!string.IsNullOrEmpty(tuNgay)) queryParams.Add($"tuNgay={Uri.EscapeDataString(tuNgay)}");
                if (!string.IsNullOrEmpty(denNgay)) queryParams.Add($"denNgay={Uri.EscapeDataString(denNgay)}");
                if (!string.IsNullOrEmpty(loaiDonHang)) queryParams.Add($"loaiDonHang={Uri.EscapeDataString(loaiDonHang)}");
                if (!string.IsNullOrEmpty(khachHang)) queryParams.Add($"search={Uri.EscapeDataString(khachHang)}");
                if (!string.IsNullOrEmpty(maDonHang)) queryParams.Add($"search={Uri.EscapeDataString(maDonHang)}");
                
                queryParams.Add($"page={page}");
                queryParams.Add("pageSize=10");
                
                var apiUrl = $"HoaDons?{string.Join("&", queryParams)}";
                var response = await _httpClient.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var hoaDons = await response.Content.ReadFromJsonAsync<List<HoaDon>>();
                    var filteredHoaDons = hoaDons ?? new List<HoaDon>();
                    
                    // Lấy thông tin phân trang từ header
                    var totalCount = 0;
                    var totalPages = 0;
                    var currentPage = page;
                    var pageSize = 10;
                    
                    if (response.Headers.Contains("X-Total-Count"))
                        int.TryParse(response.Headers.GetValues("X-Total-Count").FirstOrDefault(), out totalCount);
                    if (response.Headers.Contains("X-Total-Pages"))
                        int.TryParse(response.Headers.GetValues("X-Total-Pages").FirstOrDefault(), out totalPages);

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

                    // Tạo ViewModel với thông tin phân trang
                    var viewModel = new
                    {
                        HoaDons = filteredHoaDons,
                        Pagination = new
                        {
                            CurrentPage = currentPage,
                            TotalPages = totalPages,
                            TotalCount = totalCount,
                            PageSize = pageSize,
                            HasPreviousPage = currentPage > 1,
                            HasNextPage = currentPage < totalPages
                        }
                    };

                    return View(viewModel);
                }
                // Trả về ViewModel với danh sách rỗng
                var emptyViewModel = new
                {
                    HoaDons = new List<HoaDon>(),
                    Pagination = new
                    {
                        CurrentPage = 1,
                        TotalPages = 0,
                        TotalCount = 0,
                        PageSize = 10,
                        HasPreviousPage = false,
                        HasNextPage = false
                    }
                };
                return View(emptyViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách đơn hàng: {ex.Message}";
                // Trả về ViewModel với danh sách rỗng
                var emptyViewModel = new
                {
                    HoaDons = new List<HoaDon>(),
                    Pagination = new
                    {
                        CurrentPage = 1,
                        TotalPages = 0,
                        TotalCount = 0,
                        PageSize = 10,
                        HasPreviousPage = false,
                        HasNextPage = false
                    }
                };
                return View(emptyViewModel);
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