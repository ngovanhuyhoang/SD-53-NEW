using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace QuanView.Controllers
{
    public class DonHangController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(HttpClient httpClient, IConfiguration configuration, ILogger<DonHangController> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: DonHang/Index
        public async Task<IActionResult> Index(string trangThai = "TatCa", string tuNgay = "", string denNgay = "", string search = "", string phoneNumber = "", int page = 1)
        {
            try
            {
                var customerIdClaim = User.FindFirst("custom:id_khachhang");
                string customerId = customerIdClaim?.Value;
                
                var baseUrl = _configuration["ApiSettings:KhachHangApiBaseUrl"];
                
                string apiUrl;
                if (!string.IsNullOrEmpty(customerId))
                {
                    apiUrl = $"{baseUrl}/HoaDons/customer/{customerId}";
                }
                else
                {
                    apiUrl = $"{baseUrl}/HoaDons/guest";
                }
                
                var parameters = new List<string>();
                
                if (trangThai != "TatCa")
                {
                    parameters.Add($"trangThai={Uri.EscapeDataString(trangThai)}");
                }
                
                if (!string.IsNullOrEmpty(search))
                {
                    parameters.Add($"search={Uri.EscapeDataString(search)}");
                }
                
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    parameters.Add($"phoneNumber={Uri.EscapeDataString(phoneNumber)}");
                }
                
                // Thêm tham số phân trang
                parameters.Add($"page={page}");
                parameters.Add("pageSize=10");

                if (parameters.Count > 0)
                {
                    apiUrl += "?" + string.Join("&", parameters);
                }

                var response = await _httpClient.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var hoaDons = await response.Content.ReadFromJsonAsync<List<HoaDon>>();
                    var filteredHoaDons = hoaDons ?? new List<HoaDon>();

                    // Lọc theo ngày (nếu có)
                    if (!string.IsNullOrEmpty(tuNgay) && DateTime.TryParse(tuNgay, out var tuNgayDate))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.NgayTao.Date >= tuNgayDate.Date).ToList();
                    }

                    if (!string.IsNullOrEmpty(denNgay) && DateTime.TryParse(denNgay, out var denNgayDate))
                    {
                        filteredHoaDons = filteredHoaDons.Where(h => h.NgayTao.Date <= denNgayDate.Date).ToList();
                    }
                    
                    // Lấy thông tin phân trang từ header
                    var totalCount = 0;
                    var totalPages = 0;
                    var currentPage = page;
                    var pageSize = 10;
                    
                    if (response.Headers.Contains("X-Total-Count"))
                        int.TryParse(response.Headers.GetValues("X-Total-Count").FirstOrDefault(), out totalCount);
                    if (response.Headers.Contains("X-Total-Pages"))
                        int.TryParse(response.Headers.GetValues("X-Total-Pages").FirstOrDefault(), out totalPages);
                    
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
                    
                    ViewBag.TrangThai = trangThai;
                    ViewBag.TuNgay = tuNgay;
                    ViewBag.DenNgay = denNgay;
                    ViewBag.Search = search;
                    ViewBag.PhoneNumber = phoneNumber;
                    return View(viewModel);
                }
                else
                {
                    ViewBag.TrangThai = trangThai;
                    ViewBag.TuNgay = tuNgay;
                    ViewBag.DenNgay = denNgay;
                    ViewBag.Search = search;
                    ViewBag.PhoneNumber = phoneNumber;
                    
                    // Trả về ViewModel rỗng với phân trang
                    var emptyViewModel = new
                    {
                        HoaDons = new List<HoaDon>(),
                        Pagination = new
                        {
                            CurrentPage = page,
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
            catch (Exception ex)
            {
                _logger.LogError($"Exception in Index: {ex.Message}");
                
                ViewBag.TrangThai = trangThai;
                ViewBag.TuNgay = tuNgay;
                ViewBag.DenNgay = denNgay;
                ViewBag.Search = search;
                ViewBag.PhoneNumber = phoneNumber;
                
                // Trả về ViewModel rỗng với phân trang
                var emptyViewModel = new
                {
                    HoaDons = new List<HoaDon>(),
                    Pagination = new
                    {
                        CurrentPage = page,
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

        // GET: DonHang/ChiTiet/{id}
        public async Task<IActionResult> ChiTiet(Guid id)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:KhachHangApiBaseUrl"];
                var response = await _httpClient.GetAsync($"{baseUrl}/HoaDons/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var hoaDon = await response.Content.ReadFromJsonAsync<HoaDon>();
                    
                    var html = $@"
                        <div class='row'>
                            <div class='col-md-6'>
                                <h6>Thông tin đơn hàng</h6>
                                <table class='table table-sm'>
                                    <tr><td><strong>Mã đơn hàng:</strong></td><td>{hoaDon.MaHoaDon}</td></tr>
                                    <tr><td><strong>Ngày đặt:</strong></td><td>{hoaDon.NgayTao:dd/MM/yyyy HH:mm}</td></tr>
                                    <tr><td><strong>Trạng thái:</strong></td><td>{hoaDon.TrangThai}</td></tr>
                                    <tr><td><strong>Tổng tiền:</strong></td><td>{hoaDon.TongTien:N0} ₫</td></tr>
                                    <tr><td><strong>Tiền giảm:</strong></td><td>{(hoaDon.TienGiam?.ToString("N0") ?? "0")} ₫</td></tr>
                                </table>
                            </div>
                            <div class='col-md-6'>
                                <h6>Địa chỉ giao hàng</h6>
                                <p>{hoaDon.DiaChiGiaoHang ?? "Không có"}</p>
                            </div>
                        </div>
                        <div class='row mt-3'>
                            <div class='col-12'>
                                <h6>Chi tiết sản phẩm</h6>
                                <table class='table table-sm'>
                                    <thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead>
                                    <tbody>";
                    
                    if (hoaDon.ChiTietHoaDons != null)
                    {
                        foreach (var chiTiet in hoaDon.ChiTietHoaDons)
                        {
                            html += $@"
                                <tr>
                                    <td>{chiTiet.SanPhamChiTiet?.SanPham?.TenSanPham ?? "N/A"}</td>
                                    <td>{chiTiet.SoLuong}</td>
                                    <td>{chiTiet.DonGia:N0} ₫</td>
                                    <td>{chiTiet.ThanhTien:N0} ₫</td>
                                </tr>";
                        }
                    }
                    
                    html += @"
                                    </tbody>
                                </table>
                            </div>
                        </div>";
                    
                    return Content(html, "text/html");
                }
                else
                {
                    return Content("<div class='alert alert-danger'>Không thể tải thông tin đơn hàng</div>", "text/html");
                }
            }
            catch (Exception ex)
            {
                return Content("<div class='alert alert-danger'>Có lỗi xảy ra khi tải thông tin đơn hàng</div>", "text/html");
            }
        }

        // POST: DonHang/HuyDon/{id}
        [HttpPost]
        public async Task<IActionResult> HuyDon(Guid id)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:KhachHangApiBaseUrl"];
                var response = await _httpClient.PutAsync($"{baseUrl}/HoaDons/{id}/trangthai", 
                    new StringContent(JsonSerializer.Serialize(new { 
                        TrangThai = "Đã hủy",
                        NguoiCapNhat = "Customer",
                        LanCapNhatCuoi = DateTime.UtcNow
                    }), Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Đã hủy đơn hàng thành công" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error canceling order: {response.StatusCode} - {errorContent}");
                    return Json(new { success = false, message = "Không thể hủy đơn hàng" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception canceling order: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng" });
            }
        }
    }
}
