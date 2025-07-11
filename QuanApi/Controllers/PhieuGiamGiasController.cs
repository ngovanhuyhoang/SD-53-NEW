using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using BanQuanAu1.Web.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using QuanApi.Dtos;

namespace QuanApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhieuGiamGiasController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public PhieuGiamGiasController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhieuGiamGia>>> GetAll()
        {
            return await _context.PhieuGiamGias
                                 .AsNoTracking()
                                 .OrderByDescending(p => p.NgayTao)
                                 .ToListAsync();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PhieuGiamGia>> GetById(Guid id)
        {
            var entity = await _context.PhieuGiamGias.FindAsync(id);
            return entity is null ? NotFound() : entity;
        }
        [HttpPost]
        public async Task<ActionResult<PhieuGiamGiaDto>> Create(
                [FromBody] CreatePhieuGiamGiaDto dto,
                [FromQuery] Guid? idKhachHang,
                [FromServices] IMapper mapper)
        {
            var model = mapper.Map<PhieuGiamGia>(dto);
            model.IDPhieuGiamGia = Guid.NewGuid();
            model.NgayTao = DateTime.UtcNow;

            if (idKhachHang.HasValue)
            {
                var kh = await _context.KhachHang.FindAsync(idKhachHang.Value);
                if (kh == null) return BadRequest("Khách hàng không tồn tại.");

                // 👉 Không thay model – chỉ ghim thông tin
                model.TenPhieu += $" (Áp dụng KH {kh.MaKhachHang})";
            }

            _context.PhieuGiamGias.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = model.IDPhieuGiamGia },
                mapper.Map<PhieuGiamGiaDto>(model));
        }

        public class UpdatePayload
        {
            public PhieuGiamGia Phieu { get; set; }
            public Guid? KhachHangId { get; set; }
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePayload payload)
        {
            var model = payload.Phieu;
            var khachHangId = payload.KhachHangId;

            if (id != model.IDPhieuGiamGia)
                return BadRequest("ID không khớp.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            // ✅ Làm sạch tên phiếu trước (xóa "Áp dụng KH")
            var idx = model.TenPhieu.IndexOf("(Áp dụng KH");
            if (idx >= 0)
                model.TenPhieu = model.TenPhieu.Substring(0, idx).Trim();

            if (khachHangId.HasValue)
            {
                var kh = await _context.KhachHang.FindAsync(khachHangId.Value);
                if (kh == null)
                    return BadRequest("Khách hàng không tồn tại.");

                // ✨ Gắn lại thông tin khách hàng duy nhất 1 lần
                model.TenPhieu += $" (Áp dụng KH {kh.MaKhachHang})";
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PhieuGiamGias.Any(x => x.IDPhieuGiamGia == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }



        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.PhieuGiamGias.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.PhieuGiamGias.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
