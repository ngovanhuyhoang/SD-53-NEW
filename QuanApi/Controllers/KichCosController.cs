using BanQuanAu1.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KichCosController : ControllerBase
    {
        private readonly BanQuanAu1DbContext _context;

        public KichCosController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KichCo>>> GetAll()
        {
            return await _context.KichCos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KichCo>> GetById(Guid id)
        {
            var item = await _context.KichCos.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Post(KichCo model)
        {
            model.IDKichCo = Guid.NewGuid();
            _context.KichCos.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = model.IDKichCo }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, KichCo model)
        {
            if (id != model.IDKichCo) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.KichCos.FindAsync(id);
            if (item == null) return NotFound();
            _context.KichCos.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
