using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaTietsController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public HoaTietsController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaTiet>>> GetAll()
        {
            return await _context.HoaTiet.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HoaTiet>> GetById(Guid id)
        {
            var item = await _context.HoaTiet.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Post(HoaTiet model)
        {
            model.IDHoaTiet = Guid.NewGuid();
            _context.HoaTiet.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = model.IDHoaTiet }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, HoaTiet model)
        {
            if (id != model.IDHoaTiet) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.HoaTiet.FindAsync(id);
            if (item == null) return NotFound();
            _context.HoaTiet.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
