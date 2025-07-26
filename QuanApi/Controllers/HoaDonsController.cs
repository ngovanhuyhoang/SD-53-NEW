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

        // GET: api/HoaDons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDons()
        {
            return await _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.PhieuGiamGia)
                .Include(h => h.PhuongThucThanhToan)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.SanPham)
                .ToListAsync();
        }

        // GET: api/HoaDons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDon>> GetHoaDon(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.PhieuGiamGia)
                .Include(h => h.PhuongThucThanhToan)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.SanPhamChiTiet)
                        .ThenInclude(spct => spct.SanPham)
                .FirstOrDefaultAsync(h => h.IDHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return hoaDon;
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

                hoaDon.TrangThai = dto.TrangThai;
                hoaDon.NguoiCapNhat = dto.NguoiCapNhat;
                hoaDon.LanCapNhatCuoi = dto.LanCapNhatCuoi;

                await _context.SaveChangesAsync();

                return Ok(new { message = $"Đã cập nhật trạng thái đơn hàng thành '{dto.TrangThai}'" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái hóa đơn: {Message}", ex.Message);
                return StatusCode(500, $"Lỗi server: {ex.Message}");
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