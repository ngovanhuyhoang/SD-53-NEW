using Microsoft.AspNetCore.Mvc;
using QuanApi.Repository.IRepository;
using QuanApi.Dtos;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamNguoiDungsController : ControllerBase
    {
        private readonly GioHangIRepository _gioHangRepo;

        public SanPhamNguoiDungsController(GioHangIRepository gioHangRepo)
        {
            _gioHangRepo = gioHangRepo;
        }

        // GET: api/SanPhamNguoiDungs?pageNumber=1&pageSize=10
        [HttpGet]
        public IActionResult GetSanPhamChiTiets(
            int pageNumber = 1, int pageSize = 10,
            string search = null, int? priceFrom = null, int? priceTo = null)
        {
            var query = _gioHangRepo.ListSPCT(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.TenSanPham.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (priceFrom.HasValue)
                query = query.Where(x => x.BienThes.Any(b => b.GiaSauGiam >= priceFrom.Value)).ToList();

            if (priceTo.HasValue)
                query = query.Where(x => x.BienThes.Any(b => b.GiaSauGiam <= priceTo.Value)).ToList();

            return Ok(query);
        }

        [HttpGet("{id}")]
        public IActionResult GetDetail(Guid id)
        {
            var result = _gioHangRepo.detailSpct(id);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            return Ok(result);
        }

    }
}
