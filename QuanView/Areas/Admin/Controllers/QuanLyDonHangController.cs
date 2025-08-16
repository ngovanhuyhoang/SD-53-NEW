using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using QuanApi.Data;
using QuanApi.Dtos;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Policy = "AdminPolicy")]
    public class QuanLyDonHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public QuanLyDonHangController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

        // DTO classes for API response
        public class HoaDonDto
        {
            public Guid IDHoaDon { get; set; }
            public string MaHoaDon { get; set; }
            public decimal TongTien { get; set; }
            public decimal? TienGiam { get; set; }
            public string TrangThai { get; set; }
            public DateTime NgayTao { get; set; }
            public string TenNguoiNhan { get; set; }
            public string SoDienThoaiNguoiNhan { get; set; }
            public string DiaChiGiaoHang { get; set; }
            public KhachHangDto KhachHang { get; set; }
            public NhanVienDto NhanVien { get; set; }
            public int SoLuongSanPham { get; set; }
        }

        public class KhachHangDto
        {
            public Guid IDKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string SoDienThoai { get; set; }
        }

        public class NhanVienDto
        {
            public Guid IDNhanVien { get; set; }
            public string TenNhanVien { get; set; }
        }

        // DTO classes for detailed invoice
        public class HoaDonDetailDto
        {
            public Guid IDHoaDon { get; set; }
            public string MaHoaDon { get; set; }
            public decimal TongTien { get; set; }
            public decimal? TienGiam { get; set; }
            public decimal? PhiVanChuyen { get; set; }
            public string TrangThai { get; set; }
            public DateTime NgayTao { get; set; }
            public string TenNguoiNhan { get; set; }
            public string SoDienThoaiNguoiNhan { get; set; }
            public string DiaChiGiaoHang { get; set; }
            public string GhiChu { get; set; }
            public KhachHangDetailDto KhachHang { get; set; }
            public NhanVienDetailDto NhanVien { get; set; }
            public PhieuGiamGiaDto PhieuGiamGia { get; set; }
            public PhuongThucThanhToanDto PhuongThucThanhToan { get; set; }
            public List<ChiTietHoaDonDetailDto> ChiTietHoaDons { get; set; }
        }

        public class KhachHangDetailDto
        {
            public Guid IDKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string SoDienThoai { get; set; }
            public string Email { get; set; }
        }

        public class NhanVienDetailDto
        {
            public Guid IDNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
        }

        public class PhieuGiamGiaDto
        {
            public Guid IDPhieuGiamGia { get; set; }
            public string MaPhieu { get; set; }
            public string TenPhieu { get; set; }
        }

        public class PhuongThucThanhToanDto
        {
            public Guid IDPhuongThucThanhToan { get; set; }
            public string TenPhuongThuc { get; set; }
        }

        public class ChiTietHoaDonDetailDto
        {
            public Guid IDChiTietHoaDon { get; set; }
            public string MaChiTietHoaDon { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public SanPhamChiTietDetailDto SanPhamChiTiet { get; set; }
        }

        public class SanPhamChiTietDetailDto
        {
            public Guid IDSanPhamChiTiet { get; set; }
            public string MaSPChiTiet { get; set; }
            public decimal GiaBan { get; set; }
            public KichCoDto KichCo { get; set; }
            public MauSacDto MauSac { get; set; }
            public SanPhamDetailDto SanPham { get; set; }
        }

        public class KichCoDto { public string TenKichCo { get; set; } }
        public class MauSacDto { public string TenMauSac { get; set; } }

        public class SanPhamDetailDto
        {
            public Guid IDSanPham { get; set; }
            public string TenSanPham { get; set; }
            public string MaSanPham { get; set; }
        }

        // GET: Admin/QuanLyDonHang
        public async Task<IActionResult> Index(string trangThai, string tuNgay, string denNgay, string loaiDonHang, string khachHang, string maDonHang)
        {
            try
            {
                var response = await _httpClient.GetAsync("HoaDons");
                if (response.IsSuccessStatusCode)
                {
                    var hoaDonsData = await response.Content.ReadFromJsonAsync<List<HoaDonDto>>();
                    var filteredHoaDons = new List<HoaDon>();

                    // Convert DTO data to HoaDon objects for View compatibility
                    if (hoaDonsData != null)
                    {
                        foreach (var hoaDonData in hoaDonsData)
                        {
                            var hoaDon = new HoaDon
                            {
                                IDHoaDon = hoaDonData.IDHoaDon,
                                MaHoaDon = hoaDonData.MaHoaDon,
                                TongTien = hoaDonData.TongTien,
                                TienGiam = hoaDonData.TienGiam ?? 0,
                                TrangThai = hoaDonData.TrangThai,
                                NgayTao = hoaDonData.NgayTao,
                                TenNguoiNhan = hoaDonData.TenNguoiNhan,
                                SoDienThoaiNguoiNhan = hoaDonData.SoDienThoaiNguoiNhan,
                                DiaChiGiaoHang = hoaDonData.DiaChiGiaoHang
                            };

                            // Add KhachHang if exists
                            if (hoaDonData.KhachHang != null)
                            {
                                hoaDon.KhachHang = new KhachHang
                                {
                                    IDKhachHang = hoaDonData.KhachHang.IDKhachHang,
                                    TenKhachHang = hoaDonData.KhachHang.TenKhachHang,
                                    SoDienThoai = hoaDonData.KhachHang.SoDienThoai
                                };
                            }

                            // Add NhanVien if exists
                            if (hoaDonData.NhanVien != null)
                            {
                                hoaDon.NhanVien = new NhanVien
                                {
                                    IDNhanVien = hoaDonData.NhanVien.IDNhanVien,
                                    TenNhanVien = hoaDonData.NhanVien.TenNhanVien
                                };
                            }

                            filteredHoaDons.Add(hoaDon);
                        }
                    }

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
                    var hoaDonData = await response.Content.ReadFromJsonAsync<HoaDonDetailDto>();
                    if (hoaDonData != null)
                    {
                        var hoaDon = new HoaDon
                        {
                            IDHoaDon = hoaDonData.IDHoaDon,
                            MaHoaDon = hoaDonData.MaHoaDon,
                            TongTien = hoaDonData.TongTien,
                            TienGiam = hoaDonData.TienGiam ?? 0,
                            PhiVanChuyen = hoaDonData.PhiVanChuyen ?? 0,
                            TrangThai = hoaDonData.TrangThai,
                            NgayTao = hoaDonData.NgayTao,
                            TenNguoiNhan = hoaDonData.TenNguoiNhan,
                            SoDienThoaiNguoiNhan = hoaDonData.SoDienThoaiNguoiNhan,
                            DiaChiGiaoHang = hoaDonData.DiaChiGiaoHang,
                        };

                        // Add related data if exists
                        if (hoaDonData.KhachHang != null)
                        {
                            hoaDon.KhachHang = new KhachHang
                            {
                                IDKhachHang = hoaDonData.KhachHang.IDKhachHang,
                                TenKhachHang = hoaDonData.KhachHang.TenKhachHang,
                                SoDienThoai = hoaDonData.KhachHang.SoDienThoai,
                                Email = hoaDonData.KhachHang.Email
                            };
                        }

                        if (hoaDonData.NhanVien != null)
                        {
                            hoaDon.NhanVien = new NhanVien
                            {
                                IDNhanVien = hoaDonData.NhanVien.IDNhanVien,
                                TenNhanVien = hoaDonData.NhanVien.TenNhanVien,
                                SoDienThoai = hoaDonData.NhanVien.SoDienThoai
                            };
                        }

                        if (hoaDonData.PhuongThucThanhToan != null)
                        {
                            hoaDon.PhuongThucThanhToan = new PhuongThucThanhToan
                            {
                                IDPhuongThucThanhToan = hoaDonData.PhuongThucThanhToan.IDPhuongThucThanhToan,
                                TenPhuongThuc = hoaDonData.PhuongThucThanhToan.TenPhuongThuc
                            };
                        }

                        // Add ChiTietHoaDons if exists
                        if (hoaDonData.ChiTietHoaDons != null)
                        {
                            hoaDon.ChiTietHoaDons = new List<ChiTietHoaDon>();
                            foreach (var ct in hoaDonData.ChiTietHoaDons)
                            {
                                var chiTiet = new ChiTietHoaDon
                                {
                                    IDChiTietHoaDon = ct.IDChiTietHoaDon,
                                    MaChiTietHoaDon = ct.MaChiTietHoaDon,
                                    SoLuong = ct.SoLuong,
                                    DonGia = ct.DonGia,
                                    ThanhTien = ct.ThanhTien
                                };

                                if (ct.SanPhamChiTiet != null)
                                {
                                    chiTiet.SanPhamChiTiet = new SanPhamChiTiet
                                    {
                                        IDSanPhamChiTiet = ct.SanPhamChiTiet.IDSanPhamChiTiet,
                                        MaSPChiTiet = ct.SanPhamChiTiet.MaSPChiTiet,
                                        GiaBan = ct.SanPhamChiTiet.GiaBan,
                                        KichCo = ct.SanPhamChiTiet.KichCo != null ? new KichCo
                                        {
                                            TenKichCo = ct.SanPhamChiTiet.KichCo.TenKichCo
                                        } : null,
                                        MauSac = ct.SanPhamChiTiet.MauSac != null ? new MauSac
                                        {
                                            TenMauSac = ct.SanPhamChiTiet.MauSac.TenMauSac
                                        } : null
                                    };

                                    if (ct.SanPhamChiTiet.SanPham != null)
                                    {
                                        chiTiet.SanPhamChiTiet.SanPham = new SanPham
                                        {
                                            IDSanPham = ct.SanPhamChiTiet.SanPham.IDSanPham,
                                            TenSanPham = ct.SanPhamChiTiet.SanPham.TenSanPham,
                                            MaSanPham = ct.SanPhamChiTiet.SanPham.MaSanPham
                                        };
                                    }
                                }

                                hoaDon.ChiTietHoaDons.Add(chiTiet);
                            }
                        }

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

        // GET: Admin/QuanLyDonHang/DonHangCanXacNhan
        [HttpGet]
        public async Task<IActionResult> DonHangCanXacNhan()
        {
            try
            {
                var response = await _httpClient.GetAsync("HoaDons/don-hang-can-xac-nhan");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(result);
                }
                return StatusCode((int)response.StatusCode, "Lỗi khi lấy đơn hàng cần xác nhận");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
                // GET: Admin/QuanLyDonHang/XuatHoaDon/{id}
        [HttpGet]
        public async Task<IActionResult> XuatHoaDon(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"HoaDons/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var hoaDonData = await response.Content.ReadFromJsonAsync<HoaDonDetailDto>();
                if (hoaDonData == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var hoaDon = new HoaDon
                {
                    IDHoaDon = hoaDonData.IDHoaDon,
                    MaHoaDon = hoaDonData.MaHoaDon,
                    TongTien = hoaDonData.TongTien,
                    TienGiam = hoaDonData.TienGiam ?? 0,
                    PhiVanChuyen = hoaDonData.PhiVanChuyen ?? 0,
                    TrangThai = hoaDonData.TrangThai,
                    NgayTao = hoaDonData.NgayTao,
                    TenNguoiNhan = hoaDonData.TenNguoiNhan,
                    SoDienThoaiNguoiNhan = hoaDonData.SoDienThoaiNguoiNhan,
                    DiaChiGiaoHang = hoaDonData.DiaChiGiaoHang,
                };

                if (hoaDonData.KhachHang != null)
                {
                    hoaDon.KhachHang = new KhachHang
                    {
                        IDKhachHang = hoaDonData.KhachHang.IDKhachHang,
                        TenKhachHang = hoaDonData.KhachHang.TenKhachHang,
                        SoDienThoai = hoaDonData.KhachHang.SoDienThoai,
                        Email = hoaDonData.KhachHang.Email
                    };
                }

                if (hoaDonData.PhuongThucThanhToan != null)
                {
                    hoaDon.PhuongThucThanhToan = new PhuongThucThanhToan
                    {
                        IDPhuongThucThanhToan = hoaDonData.PhuongThucThanhToan.IDPhuongThucThanhToan,
                        TenPhuongThuc = hoaDonData.PhuongThucThanhToan.TenPhuongThuc
                    };
                }

                if (hoaDonData.ChiTietHoaDons != null)
                {
                    hoaDon.ChiTietHoaDons = new List<ChiTietHoaDon>();
                    foreach (var ct in hoaDonData.ChiTietHoaDons)
                    {
                        var chiTiet = new ChiTietHoaDon
                        {
                            IDChiTietHoaDon = ct.IDChiTietHoaDon,
                            MaChiTietHoaDon = ct.MaChiTietHoaDon,
                            SoLuong = ct.SoLuong,
                            DonGia = ct.DonGia,
                            ThanhTien = ct.ThanhTien
                        };

                        if (ct.SanPhamChiTiet != null)
                        {
                            chiTiet.SanPhamChiTiet = new SanPhamChiTiet
                            {
                                IDSanPhamChiTiet = ct.SanPhamChiTiet.IDSanPhamChiTiet,
                                MaSPChiTiet = ct.SanPhamChiTiet.MaSPChiTiet,
                                GiaBan = ct.SanPhamChiTiet.GiaBan,
                                KichCo = ct.SanPhamChiTiet.KichCo != null ? new KichCo
                                {
                                    TenKichCo = ct.SanPhamChiTiet.KichCo.TenKichCo
                                } : null,
                                MauSac = ct.SanPhamChiTiet.MauSac != null ? new MauSac
                                {
                                    TenMauSac = ct.SanPhamChiTiet.MauSac.TenMauSac
                                } : null,
                                SanPham = ct.SanPhamChiTiet.SanPham != null ? new SanPham
                                {
                                    IDSanPham = ct.SanPhamChiTiet.SanPham.IDSanPham,
                                    TenSanPham = ct.SanPhamChiTiet.SanPham.TenSanPham,
                                    MaSanPham = ct.SanPhamChiTiet.SanPham.MaSanPham
                                } : null
                            };
                        }

                        hoaDon.ChiTietHoaDons.Add(chiTiet);
                    }
                }

                return View("Invoice", hoaDon);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xuất hóa đơn: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}