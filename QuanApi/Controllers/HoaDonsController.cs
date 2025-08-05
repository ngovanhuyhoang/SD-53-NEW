using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using QuanApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
            public class HoaDonsController : ControllerBase
        {
            private readonly BanQuanAu1DbContext _context;
            private readonly ILogger<HoaDonsController> _logger;

            public HoaDonsController(BanQuanAu1DbContext context, ILogger<HoaDonsController> logger)
            {
                _context = context;
                _logger = logger;
            }

        // GET: api/HoaDons - Cho admin (hiển thị tất cả đơn hàng)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHoaDons()
        {
            try
            {
                var hoaDons = await _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.NhanVien)
                    .Include(h => h.PhieuGiamGia)
                    .Include(h => h.PhuongThucThanhToan)
                    .Select(h => new
                    {
                        IDHoaDon = h.IDHoaDon,
                        MaHoaDon = h.MaHoaDon,
                        TongTien = h.TongTien,
                        TienGiam = h.TienGiam,
                        TrangThai = h.TrangThai,
                        NgayTao = h.NgayTao,
                        TenNguoiNhan = h.TenNguoiNhan,
                        SoDienThoaiNguoiNhan = h.SoDienThoaiNguoiNhan,
                        DiaChiGiaoHang = h.DiaChiGiaoHang,
                        KhachHang = h.KhachHang != null ? new
                        {
                            IDKhachHang = h.KhachHang.IDKhachHang,
                            TenKhachHang = h.KhachHang.TenKhachHang,
                            SoDienThoai = h.KhachHang.SoDienThoai
                        } : null,
                        NhanVien = h.NhanVien != null ? new
                        {
                            IDNhanVien = h.NhanVien.IDNhanVien,
                            TenNhanVien = h.NhanVien.TenNhanVien
                        } : null,
                        SoLuongSanPham = h.ChiTietHoaDons.Count
                    })
                    .ToListAsync();

                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hóa đơn");
                return StatusCode(500, "Lỗi server khi lấy danh sách hóa đơn");
            }
        }

        // GET: api/HoaDons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetHoaDon(Guid id)
        {
            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.NhanVien)
                    .Include(h => h.PhieuGiamGia)
                    .Include(h => h.PhuongThucThanhToan)
                    .Include(h => h.ChiTietHoaDons)
                    .Where(h => h.IDHoaDon == id)
                    .Select(h => new
                    {
                        IDHoaDon = h.IDHoaDon,
                        MaHoaDon = h.MaHoaDon,
                        TongTien = h.TongTien,
                        TienGiam = h.TienGiam,
                        TrangThai = h.TrangThai,
                        NgayTao = h.NgayTao,
                        TenNguoiNhan = h.TenNguoiNhan,
                        SoDienThoaiNguoiNhan = h.SoDienThoaiNguoiNhan,
                        DiaChiGiaoHang = h.DiaChiGiaoHang,
                        KhachHang = h.KhachHang != null ? new
                        {
                            IDKhachHang = h.KhachHang.IDKhachHang,
                            TenKhachHang = h.KhachHang.TenKhachHang,
                            SoDienThoai = h.KhachHang.SoDienThoai,
                            Email = h.KhachHang.Email
                        } : null,
                        NhanVien = h.NhanVien != null ? new
                        {
                            IDNhanVien = h.NhanVien.IDNhanVien,
                            TenNhanVien = h.NhanVien.TenNhanVien,
                            SoDienThoai = h.NhanVien.SoDienThoai
                        } : null,
                        PhieuGiamGia = h.PhieuGiamGia != null ? new
                        {
                            IDPhieuGiamGia = h.PhieuGiamGia.IDPhieuGiamGia,
                            MaPhieu = h.PhieuGiamGia.MaCode,
                            TenPhieu = h.PhieuGiamGia.TenPhieu
                        } : null,
                        PhuongThucThanhToan = h.PhuongThucThanhToan != null ? new
                        {
                            IDPhuongThucThanhToan = h.PhuongThucThanhToan.IDPhuongThucThanhToan,
                            TenPhuongThuc = h.PhuongThucThanhToan.TenPhuongThuc
                        } : null,
                        ChiTietHoaDons = h.ChiTietHoaDons.Select(ct => new
                        {
                            IDChiTietHoaDon = ct.IDChiTietHoaDon,
                            MaChiTietHoaDon = ct.MaChiTietHoaDon,
                            SoLuong = ct.SoLuong,
                            DonGia = ct.DonGia,
                            ThanhTien = ct.ThanhTien,
                            SanPhamChiTiet = new
                            {
                                IDSanPhamChiTiet = ct.SanPhamChiTiet.IDSanPhamChiTiet,
                                MaSPChiTiet = ct.SanPhamChiTiet.MaSPChiTiet,
                                GiaBan = ct.SanPhamChiTiet.GiaBan,
                                SanPham = new
                                {
                                    IDSanPham = ct.SanPhamChiTiet.SanPham.IDSanPham,
                                    TenSanPham = ct.SanPhamChiTiet.SanPham.TenSanPham,
                                    MaSanPham = ct.SanPhamChiTiet.SanPham.MaSanPham
                                }
                            }
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (hoaDon == null)
                {
                    return NotFound();
                }

                return Ok(hoaDon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết hóa đơn: {Message}", ex.Message);
                return StatusCode(500, "Lỗi khi tải chi tiết đơn hàng");
            }
        }

        // POST: api/HoaDons
        [HttpPost]
        public async Task<ActionResult<HoaDon>> CreateHoaDon([FromBody] CreateHoaDonDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Kiểm tra dữ liệu đầu vào
                if (dto.ChiTietHoaDons == null || !dto.ChiTietHoaDons.Any())
                {
                    return BadRequest("Không có sản phẩm nào trong đơn hàng");
                }

                if (dto.TongTien <= 0)
                {
                    return BadRequest("Tổng tiền phải lớn hơn 0");
                }

                // Tạo hóa đơn mới
                var hoaDon = new HoaDon
                {
                    IDHoaDon = Guid.NewGuid(),
                    MaHoaDon = $"HD_{DateTime.Now:yyyyMMddHHmmss}",
                    IDKhachHang = dto.KhachHangId,
                    IDNhanVien = dto.NhanVienId,
                    IDPhieuGiamGia = dto.PhieuGiamGiaId,
                    IDPhuongThucThanhToan = dto.PhuongThucThanhToanId,
                    TongTien = dto.TongTien,
                    TienGiam = dto.TienGiam ?? 0,
                    TrangThai = "Chờ xác nhận",
                    TenNguoiNhan = dto.TenNguoiNhan ?? "",
                    SoDienThoaiNguoiNhan = dto.SoDienThoaiNguoiNhan ?? "",
                    DiaChiGiaoHang = dto.DiaChiGiaoHang ?? "",
                    NgayTao = DateTime.UtcNow,
                    NguoiTao = "Customer",
                    TrangThaiHoaDon = true
                };

                _context.HoaDons.Add(hoaDon);

                // Tạo chi tiết hóa đơn từ dữ liệu được gửi lên
                foreach (var chiTiet in dto.ChiTietHoaDons)
                {
                    var chiTietHoaDon = new ChiTietHoaDon
                    {
                        IDChiTietHoaDon = Guid.NewGuid(),
                        MaChiTietHoaDon = $"CTHD_{DateTime.Now:yyyyMMddHHmmss}_{chiTiet.IDSanPhamChiTiet.ToString().Substring(0, 8)}",
                        IDHoaDon = hoaDon.IDHoaDon,
                        IDSanPhamChiTiet = chiTiet.IDSanPhamChiTiet,
                        SoLuong = chiTiet.SoLuong,
                        DonGia = chiTiet.DonGia,
                        ThanhTien = chiTiet.ThanhTien,
                        NgayTao = DateTime.UtcNow,
                        NguoiTao = "Customer",
                        TrangThai = true
                    };

                    _context.ChiTietHoaDons.Add(chiTietHoaDon);

                    // Cập nhật số lượng sản phẩm
                    var sanPhamChiTiet = await _context.SanPhamChiTiets.FindAsync(chiTiet.IDSanPhamChiTiet);
                    if (sanPhamChiTiet != null)
                    {
                        sanPhamChiTiet.SoLuong -= chiTiet.SoLuong;
                        if (sanPhamChiTiet.SoLuong < 0)
                        {
                            return BadRequest($"Sản phẩm {sanPhamChiTiet.MaSPChiTiet} không đủ số lượng");
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHoaDon), new { id = hoaDon.IDHoaDon }, hoaDon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hóa đơn: {Message}", ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);
                }
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // PUT: api/HoaDons/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoaDon(Guid id, HoaDon hoaDon)
        {
            if (id != hoaDon.IDHoaDon)
            {
                return BadRequest();
            }

            _context.Entry(hoaDon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoaDonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/HoaDons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoaDon(Guid id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HoaDonExists(Guid id)
        {
            return _context.HoaDons.Any(e => e.IDHoaDon == id);
        }

        // PUT: api/HoaDons/{id}/trangthai
        [HttpPut("{id}/trangthai")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] UpdateTrangThaiDto dto)
        {
            try
            {
                var hoaDon = await _context.HoaDons.FindAsync(id);
                if (hoaDon == null)
                {
                    return NotFound("Không tìm thấy hóa đơn");
                }

                // Cập nhật trạng thái
                hoaDon.TrangThai = dto.TrangThai;
                hoaDon.NguoiCapNhat = dto.NguoiCapNhat;
                hoaDon.LanCapNhatCuoi = dto.LanCapNhatCuoi;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Updated order {id} status to {dto.TrangThai}");
                
                return Ok(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order status: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/HoaDons/don-hang-can-xac-nhan
        [HttpGet("don-hang-can-xac-nhan")]
        public async Task<ActionResult<object>> GetDonHangCanXacNhan()
        {
            try
            {
                var donHangCanXacNhan = await _context.HoaDons
                    .Where(h => h.TrangThai == "Chờ xác nhận" && !string.IsNullOrEmpty(h.DiaChiGiaoHang))
                    .Include(h => h.KhachHang)
                    .OrderByDescending(h => h.NgayTao)
                    .Take(5)
                    .Select(h => new
                    {
                        id = h.IDHoaDon,
                        maHoaDon = h.MaHoaDon,
                        tenKhachHang = h.KhachHang != null ? h.KhachHang.TenKhachHang : h.TenNguoiNhan,
                        ngayTao = h.NgayTao,
                        tongTien = h.TongTien
                    })
                    .ToListAsync();

                var tongSo = await _context.HoaDons
                    .CountAsync(h => h.TrangThai == "Chờ xác nhận" && !string.IsNullOrEmpty(h.DiaChiGiaoHang));

                return Ok(new
                {
                    tongSo = tongSo,
                    danhSach = donHangCanXacNhan
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đơn hàng cần xác nhận: {Message}", ex.Message);
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: api/HoaDons/guest 
        [HttpGet("guest")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDonsByGuest(
            string? phoneNumber = null, string? search = null, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return BadRequest("Số điện thoại là bắt buộc để tìm đơn hàng");
                }

                if (phoneNumber.Length < 10 || phoneNumber.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có 10-11 số");
                }

                var query = _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.NhanVien)
                    .Include(h => h.PhieuGiamGia)
                    .Include(h => h.PhuongThucThanhToan)
                    .Include(h => h.ChiTietHoaDons)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                            .ThenInclude(spct => spct.SanPham)
                    .Where(h => h.TrangThaiHoaDon);

                query = query.Where(h => h.SoDienThoaiNguoiNhan == phoneNumber);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(h => h.MaHoaDon.Contains(search));
                }

                query = query.OrderByDescending(h => h.NgayTao);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var hoaDons = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                Response.Headers.Append("X-Total-Pages", totalPages.ToString());
                Response.Headers.Append("X-Current-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());

                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm đơn hàng cho khách vãng lai với số điện thoại {PhoneNumber}", phoneNumber);
                return StatusCode(500, "Lỗi nội bộ server");
            }
        }

        // GET: api/HoaDons/customer/{customerId} - Cho người dùng đã đăng nhập
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDonsByCustomer(
            Guid customerId, string? search = null, string? fromDate = null, string? toDate = null, 
            int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.HoaDons
                    .Include(h => h.KhachHang)
                    .Include(h => h.NhanVien)
                    .Include(h => h.PhieuGiamGia)
                    .Include(h => h.PhuongThucThanhToan)
                    .Include(h => h.ChiTietHoaDons)
                        .ThenInclude(ct => ct.SanPhamChiTiet)
                            .ThenInclude(spct => spct.SanPham)
                    .Where(h => h.IDKhachHang == customerId && h.TrangThaiHoaDon)
                    .AsQueryable();

                // Tìm kiếm theo mã đơn hàng
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(h => h.MaHoaDon.Contains(search));
                }

                // Lọc theo ngày từ
                if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fromDateParsed))
                {
                    query = query.Where(h => h.NgayTao.Date >= fromDateParsed.Date);
                }

                // Lọc theo ngày đến
                if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var toDateParsed))
                {
                    query = query.Where(h => h.NgayTao.Date <= toDateParsed.Date);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var hoaDons = await query
                    .OrderByDescending(h => h.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                Response.Headers.Append("X-Total-Pages", totalPages.ToString());
                Response.Headers.Append("X-Current-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());

                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy đơn hàng cho khách hàng {CustomerId}", customerId);
                return StatusCode(500, "Lỗi nội bộ server");
            }
        }
    }

    public class CreateHoaDonDto
    {
        public Guid? KhachHangId { get; set; }
        public Guid? NhanVienId { get; set; }
        public Guid? PhieuGiamGiaId { get; set; }
        public Guid PhuongThucThanhToanId { get; set; }
        public decimal TongTien { get; set; }
        public decimal? TienGiam { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoDienThoaiNguoiNhan { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string GhiChu { get; set; }
        public List<ChiTietHoaDonDto> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDonDto>();
    }

    public class ChiTietHoaDonDto
    {
        public Guid IDSanPhamChiTiet { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class UpdateTrangThaiDto
    {
        public string TrangThai { get; set; }
        public string NguoiCapNhat { get; set; }
        public DateTime LanCapNhatCuoi { get; set; }
    }
} 