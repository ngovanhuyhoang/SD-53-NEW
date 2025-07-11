using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using BanQuanAu1.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangPhieuGiamController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public KhachHangPhieuGiamController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        // GET: api/KhachHangPhieuGiam
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhachHangPhieuGiam>>> GetAll()
        {
            return await _context.KhachHangPhieuGiams
                .AsNoTracking()
                .Include(k => k.KhachHang)
                .Include(k => k.PhieuGiamGia)
                .ToListAsync();
        }

        // GET: api/KhachHangPhieuGiam/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<KhachHangPhieuGiam>> GetById(Guid id)
        {
            var item = await _context.KhachHangPhieuGiams
                .Include(k => k.KhachHang)
                .Include(k => k.PhieuGiamGia)
                .FirstOrDefaultAsync(x => x.IDKhachHangPhieuGiam == id);

            return item == null ? NotFound() : item;
        }

        // GET: api/KhachHangPhieuGiam/by-voucher/{idPhieu}
        [HttpGet("by-voucher/{idPhieu:guid}")]
        public async Task<ActionResult<Guid?>> GetKhachHangByVoucher(Guid idPhieu)
        {
            var item = await _context.KhachHangPhieuGiams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IDPhieuGiamGia == idPhieu);

            return item?.IDKhachHang;
        }

        // POST: api/KhachHangPhieuGiam
        [HttpPost]
        public async Task<IActionResult> Create(KhachHangPhieuGiam model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.KhachHangPhieuGiams.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.IDKhachHangPhieuGiam }, model);
        }

        // PUT: api/KhachHangPhieuGiam/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, KhachHangPhieuGiam model)
        {
            if (id != model.IDKhachHangPhieuGiam)
                return BadRequest("ID không khớp.");

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.KhachHangPhieuGiams.Any(e => e.IDKhachHangPhieuGiam == id))
                    return NotFound();
                throw;
            }
        }

        // DELETE: api/KhachHangPhieuGiam/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _context.KhachHangPhieuGiams.FindAsync(id);
            if (entity == null) return NotFound();

            _context.KhachHangPhieuGiams.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
