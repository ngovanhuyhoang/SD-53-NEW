using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;
using QuanApi.Data;
using QuanApi.Dtos;


namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamsController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public SanPhamsController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        // GET: api/SanPhams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamDto>>> GetSanPhams()
        {
            var result = await _context.SanPhams
                .Include(s => s.ChatLieu)
                .Include(s => s.DanhMuc)
                .Include(s => s.ThuongHieu)
                .Include(s => s.LoaiOng)
                .Include(s => s.KieuDang)
                .Include(s => s.LungQuan)
                .Include(s => s.SanPhamChiTiets)
                    .ThenInclude(ct => ct.KichCo)
                .Include(s => s.SanPhamChiTiets)
                    .ThenInclude(ct => ct.MauSac)
                .Include(s => s.SanPhamChiTiets)
                    .ThenInclude(ct => ct.HoaTiet)
                .Select(s => new SanPhamDto
                {
                    IDSanPham = s.IDSanPham,
                    MaSanPham = s.MaSanPham,
                    TenSanPham = s.TenSanPham,
                    IDChatLieu = s.IDChatLieu,
                    IDDanhMuc = s.IDDanhMuc,
                    IDThuongHieu = s.IDThuongHieu,
                    IDLoaiOng = s.IDLoaiOng,
                    IDKieuDang = s.IDKieuDang,
                    IDLungQuan = s.IDLungQuan,
                    CoXepLy = s.CoXepLy,
                    CoGian = s.CoGian,
                    TrangThai = s.TrangThai,
                    TenChatLieu = s.ChatLieu.TenChatLieu,
                    TenDanhMuc = s.DanhMuc.TenDanhMuc,
                    TenThuongHieu = s.ThuongHieu.TenThuongHieu,
                    TenLoaiOng = s.LoaiOng.TenLoaiOng,
                    TenKieuDang = s.KieuDang.TenKieuDang,
                    TenLungQuan = s.LungQuan.TenLungQuan,
                    ChiTietSanPhams = s.SanPhamChiTiets.Select(ct => new SanPhamChiTietDto
                    {
                        IdSanPhamChiTiet = ct.IDSanPhamChiTiet,
                        IdSanPham = ct.IDSanPham,
                        IdKichCo = ct.IDKichCo,
                        IdMauSac = ct.IDMauSac,
                        IdHoaTiet = ct.IDHoaTiet ?? Guid.Empty,
                        SoLuong = ct.SoLuong,
                        GiaBan = ct.GiaBan,
                        TenKichCo = ct.KichCo.TenKichCo,
                        TenMauSac = ct.MauSac.TenMauSac,
                        TenHoaTiet = ct.HoaTiet != null ? ct.HoaTiet.TenHoaTiet : null
                    }).ToList()
                })
                .ToListAsync();

            return result;
        }



        // GET: api/SanPhams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetSanPham(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return sanPham;
        }

        // PUT: api/SanPhams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSanPham(Guid id, SanPham sanPham)
        {
            if (id != sanPham.IDSanPham)
            {
                return BadRequest();
            }

            _context.Entry(sanPham).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanPhamExists(id))
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

        // POST: api/SanPhams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SanPham>> PostSanPham(SanPham sanPham)
        {
            if (sanPham.IDSanPham == Guid.Empty)
                sanPham.IDSanPham = Guid.NewGuid();

            // Tách biến thể ra trước khi thêm sản phẩm
            var chiTietList = sanPham.SanPhamChiTiets?.ToList();
            sanPham.SanPhamChiTiets = null;

            // ✅ Thêm sản phẩm trước
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            // ✅ Thêm biến thể sau
            if (chiTietList != null && chiTietList.Any())
            {
                foreach (var ct in chiTietList)
                {
                    ct.IDSanPhamChiTiet = Guid.NewGuid();
                    ct.IDSanPham = sanPham.IDSanPham;
                }

                await _context.SanPhamChiTiets.AddRangeAsync(chiTietList);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetSanPham", new { id = sanPham.IDSanPham }, sanPham);
        }





        // DELETE: api/SanPhams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanPham(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SanPhamExists(Guid id)
        {
            return _context.SanPhams.Any(e => e.IDSanPham == id);
        }
    }
}
