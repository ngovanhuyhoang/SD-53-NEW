using AutoMapper;
using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using QuanApi.Dtos;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace QuanApi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;
        private readonly IMapper _mapper;

        public KhachHangController(BanQuanAu1DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KhachHangDto>>> GetKhachHang()
        {
            var khachHangs = await _context.KhachHang.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<KhachHangDto>>(khachHangs));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KhachHangDto>> GetKhachHang(Guid id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<KhachHangDto>(khachHang));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<KhachHangDto>> PostKhachHang([FromBody] CreateKhachHangDto createKhachHangDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.KhachHang.AnyAsync(kh => kh.MaKhachHang == createKhachHangDto.MaKhachHang))
            {
                ModelState.AddModelError("MaKhachHang", "Mã khách hàng đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }
            if (!string.IsNullOrEmpty(createKhachHangDto.Email) && await _context.KhachHang.AnyAsync(kh => kh.Email == createKhachHangDto.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }
            if (!string.IsNullOrEmpty(createKhachHangDto.SoDienThoai) && await _context.KhachHang.AnyAsync(kh => kh.SoDienThoai == createKhachHangDto.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }

            var khachHang = _mapper.Map<KhachHang>(createKhachHangDto);

            khachHang.IDKhachHang = Guid.NewGuid();
            khachHang.NgayTao = DateTime.UtcNow;
            khachHang.NguoiTao = User.Identity?.Name ?? "System";
            khachHang.LanCapNhatCuoi = null;
            khachHang.NguoiCapNhat = null;
            khachHang.TrangThai = true;

            if (!string.IsNullOrEmpty(createKhachHangDto.MatKhau))
            {
                khachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(createKhachHangDto.MatKhau);
            }
            else
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu là bắt buộc khi tạo mới khách hàng.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.KhachHang.Add(khachHang);
            await _context.SaveChangesAsync();

            var khachHangDto = _mapper.Map<KhachHangDto>(khachHang);

            return CreatedAtAction(nameof(GetKhachHang), new { id = khachHang.IDKhachHang }, khachHangDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutKhachHang(Guid id, [FromBody] UpdateKhachHangDto updateKhachHangDto)
        {
            if (id != updateKhachHangDto.IDKhachHang)
            {
                return BadRequest("ID trong URL không khớp với ID khách hàng trong dữ liệu.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var originalKhachHang = await _context.KhachHang.FindAsync(id);
            if (originalKhachHang == null)
            {
                return NotFound("Không tìm thấy khách hàng để cập nhật.");
            }

            if (await _context.KhachHang.AnyAsync(kh => kh.MaKhachHang == updateKhachHangDto.MaKhachHang && kh.IDKhachHang != id))
            {
                ModelState.AddModelError("MaKhachHang", "Mã khách hàng đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }
            if (!string.IsNullOrEmpty(updateKhachHangDto.Email) && await _context.KhachHang.AnyAsync(kh => kh.Email == updateKhachHangDto.Email && kh.IDKhachHang != id))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }
            if (!string.IsNullOrEmpty(updateKhachHangDto.SoDienThoai) && await _context.KhachHang.AnyAsync(kh => kh.SoDienThoai == updateKhachHangDto.SoDienThoai && kh.IDKhachHang != id))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại.");
                return Conflict(new ValidationProblemDetails(ModelState));
            }

            _mapper.Map(updateKhachHangDto, originalKhachHang);

            originalKhachHang.LanCapNhatCuoi = DateTime.UtcNow;
            originalKhachHang.NguoiCapNhat = User.Identity?.Name ?? "System";

            if (!string.IsNullOrEmpty(updateKhachHangDto.MatKhau))
            {
                originalKhachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(updateKhachHangDto.MatKhau);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KhachHangExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Lỗi cơ sở dữ liệu khi cập nhật: {ex.InnerException?.Message ?? ex.Message}");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKhachHang(Guid id)
        {
            var khachHang = await _context.KhachHang.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            _context.KhachHang.Remove(khachHang);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KhachHangExists(Guid id)
        {
            return _context.KhachHang.Any(e => e.IDKhachHang == id);
        }
    }
}
