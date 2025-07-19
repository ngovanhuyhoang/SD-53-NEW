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
                .Include(s => s.AnhSanPhams.Where(a => a.TrangThai))
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
                    // Lấy ảnh chính hoặc ảnh đầu tiên
                    AnhChinh = s.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => a.UrlAnh)
                        .FirstOrDefault(),
                    // Lấy tất cả ảnh sản phẩm
                    DanhSachAnh = s.AnhSanPhams
                        .Where(a => a.TrangThai)
                        .OrderByDescending(a => a.LaAnhChinh)
                        .ThenBy(a => a.NgayTao)
                        .Select(a => new AnhSanPhamDto
                        {
                            IDAnhSanPham = a.IDAnhSanPham,
                            MaAnh = a.MaAnh,
                            UrlAnh = a.UrlAnh,
                            LaAnhChinh = a.LaAnhChinh,
                            NgayTao = a.NgayTao
                        }).ToList(),
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

        // Thêm ảnh cho sản phẩm
        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddProductImage(Guid id, [FromBody] AddAnhSanPhamDto dto)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
                return NotFound("Không tìm thấy sản phẩm.");

            var anhSanPham = new AnhSanPham
            {
                IDAnhSanPham = Guid.NewGuid(),
                MaAnh = $"IMG_{DateTime.Now:yyyyMMddHHmmssfff}",
                IDSanPham = id,
                UrlAnh = dto.UrlAnh,
                LaAnhChinh = dto.LaAnhChinh,
                NgayTao = DateTime.UtcNow,
                NguoiTao = User.Identity?.Name ?? "System",
                TrangThai = true
            };

            // Nếu đặt làm ảnh chính, bỏ ảnh chính cũ
            if (dto.LaAnhChinh)
            {
                var anhChinhCu = await _context.AnhSanPhams
                    .Where(a => a.IDSanPham == id && a.LaAnhChinh && a.TrangThai)
                    .FirstOrDefaultAsync();
                
                if (anhChinhCu != null)
                {
                    anhChinhCu.LaAnhChinh = false;
                    anhChinhCu.LanCapNhatCuoi = DateTime.UtcNow;
                    anhChinhCu.NguoiCapNhat = User.Identity?.Name ?? "System";
                }
            }

            _context.AnhSanPhams.Add(anhSanPham);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm ảnh sản phẩm thành công.",
                anhSanPham.IDAnhSanPham,
                anhSanPham.MaAnh,
                anhSanPham.UrlAnh,
                anhSanPham.LaAnhChinh
            });
        }

        // Xóa ảnh sản phẩm
        [HttpDelete("images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(Guid imageId)
        {
            var anhSanPham = await _context.AnhSanPhams.FindAsync(imageId);
            if (anhSanPham == null)
                return NotFound("Không tìm thấy ảnh sản phẩm.");

            // Không xóa thực sự, chỉ đánh dấu vô hiệu
            anhSanPham.TrangThai = false;
            anhSanPham.LanCapNhatCuoi = DateTime.UtcNow;
            anhSanPham.NguoiCapNhat = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return Ok("Đã xóa ảnh sản phẩm.");
        }

        // Đặt ảnh chính
        [HttpPut("images/{imageId}/set-main")]
        public async Task<IActionResult> SetMainImage(Guid imageId)
        {
            var anhSanPham = await _context.AnhSanPhams.FindAsync(imageId);
            if (anhSanPham == null)
                return NotFound("Không tìm thấy ảnh sản phẩm.");

            if (!anhSanPham.TrangThai)
                return BadRequest("Ảnh sản phẩm đã bị vô hiệu hóa.");

            // Bỏ ảnh chính cũ
            var anhChinhCu = await _context.AnhSanPhams
                .Where(a => a.IDSanPham == anhSanPham.IDSanPham && 
                           a.LaAnhChinh && 
                           a.TrangThai && 
                           a.IDAnhSanPham != imageId)
                .FirstOrDefaultAsync();
            
            if (anhChinhCu != null)
            {
                anhChinhCu.LaAnhChinh = false;
                anhChinhCu.LanCapNhatCuoi = DateTime.UtcNow;
                anhChinhCu.NguoiCapNhat = User.Identity?.Name ?? "System";
            }

            // Đặt ảnh mới làm chính
            anhSanPham.LaAnhChinh = true;
            anhSanPham.LanCapNhatCuoi = DateTime.UtcNow;
            anhSanPham.NguoiCapNhat = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            return Ok("Đã đặt ảnh làm ảnh chính.");
        }

        // Lấy danh sách ảnh của sản phẩm
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetProductImages(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
                return NotFound("Không tìm thấy sản phẩm.");

            var images = await _context.AnhSanPhams
                .Where(a => a.IDSanPham == id && a.TrangThai)
                .OrderByDescending(a => a.LaAnhChinh)
                .ThenBy(a => a.NgayTao)
                .Select(a => new AnhSanPhamDto
                {
                    IDAnhSanPham = a.IDAnhSanPham,
                    MaAnh = a.MaAnh,
                    IDSanPham = a.IDSanPham,
                    UrlAnh = a.UrlAnh,
                    LaAnhChinh = a.LaAnhChinh,
                    NgayTao = a.NgayTao,
                    NguoiTao = a.NguoiTao,
                    LanCapNhatCuoi = a.LanCapNhatCuoi,
                    NguoiCapNhat = a.NguoiCapNhat,
                    TrangThai = a.TrangThai
                })
                .ToListAsync();

            return Ok(images);
        }
    }
}
