using BanQuanAu1.Web.Data; // Namespace chứa DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using QuanApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamChiTietsController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public SanPhamChiTietsController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        // GET: api/sanphamchitiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamChiTietDto>>> GetAll()
        {
            try
            {
                var list = await _context.SanPhamChiTiets
                    .Include(ct => ct.KichCo)
                    .Include(ct => ct.MauSac)
                    .Include(ct => ct.HoaTiet)
                    .Include(ct => ct.SanPham)

                    .ToListAsync();

                var result = list.Select(ct => new SanPhamChiTietDto
                {
                    IdSanPhamChiTiet = ct.IDSanPhamChiTiet,
                    IdSanPham = ct.IDSanPham,
                    IdKichCo = ct.IDKichCo,
                    IdMauSac = ct.IDMauSac,
                    IdHoaTiet = ct.IDHoaTiet ?? Guid.Empty,
                    SoLuong = ct.SoLuong,
                    GiaBan = ct.GiaBan,
                    MaSPChiTiet = ct.MaSPChiTiet,
                    TenKichCo = ct.KichCo?.TenKichCo ?? "N/A",
                    TenMauSac = ct.MauSac?.TenMauSac ?? "N/A",
                    TenHoaTiet = ct.HoaTiet?.TenHoaTiet ?? "N/A",
                    TenSanPham = ct.SanPham?.TenSanPham ?? "Không xác định",
                    TrangThai = ct.SanPham?.TrangThai ?? true,


                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: api/sanphamchitiets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamChiTietDto>> GetById(Guid id)
        {
            try
            {
                var ct = await _context.SanPhamChiTiets
                    .Include(ct => ct.KichCo)
                    .Include(ct => ct.MauSac)
                    .Include(ct => ct.HoaTiet)
                    .Include(ct => ct.SanPham)

                    .FirstOrDefaultAsync(ct => ct.IDSanPhamChiTiet == id);

                if (ct == null)
                    return NotFound("Không tìm thấy sản phẩm chi tiết.");

                var result = new SanPhamChiTietDto
                {
                    IdSanPhamChiTiet = ct.IDSanPhamChiTiet,
                    IdSanPham = ct.IDSanPham,
                    IdKichCo = ct.IDKichCo,
                    IdMauSac = ct.IDMauSac,
                    IdHoaTiet = ct.IDHoaTiet ?? Guid.Empty,
                    SoLuong = ct.SoLuong,
                    GiaBan = ct.GiaBan,
                    MaSPChiTiet = ct.MaSPChiTiet,
                    TenKichCo = ct.KichCo?.TenKichCo ?? "N/A",
                    TenMauSac = ct.MauSac?.TenMauSac ?? "N/A",
                    TenHoaTiet = ct.HoaTiet?.TenHoaTiet ?? "N/A",
                    TenSanPham = ct.SanPham?.TenSanPham ?? "Không xác định",
                    TrangThai = ct.SanPham?.TrangThai ?? true,

                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // GET: api/sanphamchitiets/bysanpham?idsanpham=...
        [HttpGet("bysanpham")]
        public async Task<ActionResult<IEnumerable<SanPhamChiTietDto>>> GetBySanPham([FromQuery] Guid idsanpham)
        {
            try
            {
                if (idsanpham == Guid.Empty)
                    return BadRequest("ID sản phẩm không hợp lệ.");

                var list = await _context.SanPhamChiTiets
                    .Where(ct => ct.IDSanPham == idsanpham)
                    .Include(ct => ct.KichCo)
                    .Include(ct => ct.MauSac)
                    .Include(ct => ct.HoaTiet)
                    .Include(ct => ct.SanPham)

                    .ToListAsync();

                if (!list.Any())
                    return NotFound("Không tìm thấy chi tiết sản phẩm cho ID sản phẩm này.");

                var result = list.Select(ct => new SanPhamChiTietDto
                {
                    IdSanPhamChiTiet = ct.IDSanPhamChiTiet,
                    IdSanPham = ct.IDSanPham,
                    IdKichCo = ct.IDKichCo,
                    IdMauSac = ct.IDMauSac,
                    IdHoaTiet = ct.IDHoaTiet ?? Guid.Empty,
                    SoLuong = ct.SoLuong,
                    GiaBan = ct.GiaBan,
                    MaSPChiTiet = ct.MaSPChiTiet,
                    TenKichCo = ct.KichCo?.TenKichCo ?? "N/A",
                    TenMauSac = ct.MauSac?.TenMauSac ?? "N/A",
                    TenHoaTiet = ct.HoaTiet?.TenHoaTiet ?? "N/A",
                    TenSanPham = ct.SanPham?.TenSanPham ?? "Không xác định",
                    TrangThai = ct.SanPham?.TrangThai ?? true,

                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // POST: api/sanphamchitiets
        [HttpPost]
        public async Task<IActionResult> Post(SanPhamChiTietDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto.IdSanPham == Guid.Empty || dto.IdKichCo == Guid.Empty || dto.IdMauSac == Guid.Empty)
                    return BadRequest("ID sản phẩm, kích cỡ hoặc màu sắc không hợp lệ.");

                var entity = new SanPhamChiTiet
                {
                    IDSanPhamChiTiet = dto.IdSanPhamChiTiet == Guid.Empty ? Guid.NewGuid() : dto.IdSanPhamChiTiet,
                    IDSanPham = dto.IdSanPham,
                    IDKichCo = dto.IdKichCo,
                    IDMauSac = dto.IdMauSac,
                    IDHoaTiet = dto.IdHoaTiet == Guid.Empty ? null : dto.IdHoaTiet,
                    SoLuong = dto.SoLuong,
                    GiaBan = dto.GiaBan,
                    MaSPChiTiet = dto.MaSPChiTiet ?? $"CT_{DateTime.UtcNow.Ticks.ToString()[^6..]}"
                };

                _context.SanPhamChiTiets.Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IDSanPhamChiTiet }, entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // PUT: api/sanphamchitiets/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, SanPhamChiTietDto dto)
        {
            try
            {
                if (id != dto.IdSanPhamChiTiet)
                    return BadRequest("ID sản phẩm chi tiết không khớp.");

                var ct = await _context.SanPhamChiTiets.FindAsync(id);
                if (ct == null)
                    return NotFound("Không tìm thấy sản phẩm chi tiết.");

                ct.IDSanPham = dto.IdSanPham;
                ct.IDKichCo = dto.IdKichCo;
                ct.IDMauSac = dto.IdMauSac;
                ct.IDHoaTiet = dto.IdHoaTiet == Guid.Empty ? null : dto.IdHoaTiet;
                ct.SoLuong = dto.SoLuong;
                ct.GiaBan = dto.GiaBan;
                ct.MaSPChiTiet = dto.MaSPChiTiet ?? ct.MaSPChiTiet;

                _context.Entry(ct).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // DELETE: api/sanphamchitiets/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ct = await _context.SanPhamChiTiets.FindAsync(id);
                if (ct == null)
                    return NotFound("Không tìm thấy sản phẩm chi tiết.");

                _context.SanPhamChiTiets.Remove(ct);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
        



    }
}