using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MauSacsController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public MauSacsController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MauSac>>> GetAll()
        {
            return await _context.MauSacs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MauSac>> GetById(Guid id)
        {
            var item = await _context.MauSacs.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Post(MauSac model)
        {
            model.IDMauSac = Guid.NewGuid();
            _context.MauSacs.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = model.IDMauSac }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, MauSac model)
        {
            if (id != model.IDMauSac) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.MauSacs.FindAsync(id);
            if (item == null) return NotFound();
            _context.MauSacs.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
