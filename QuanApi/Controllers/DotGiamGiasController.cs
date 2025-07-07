using Microsoft.AspNetCore.Mvc;
using QuanApi.Data;
using QuanApi.Dtos;
using QuanApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DotGiamGiasController : ControllerBase
    {
        private readonly DotGiamGiaIRepository _repository;

        public DotGiamGiasController(DotGiamGiaIRepository repository)
        {
            _repository = repository;
        }

        // GET: api/DotGiamGias
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DotGiamGiaFilterDto filter)
        {
            var result = await _repository.GetDotGiamGia(
                filter.MaDot,
                filter.TenDot,
                filter.PhanTramGiam,
                filter.TuNgay,
                filter.DenNgay,
                filter.TrangThai,
                filter.Page,
                filter.PageSize
            );
            
            var now = DateTime.Now;

            foreach (var item in result.Data)
            {
                if (item.TrangThai == true && item.NgayKetThuc < now)
                {
                    item.TrangThai = false;
                    await _repository.UpdateTrangThaiAsync(item.IDDotGiamGia, false);
                }
            }

            return Ok(result);
        }


        // GET: api/DotGiamGias/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dot = await _repository.GetByIdAsync(id);
            if (dot == null) return NotFound();
            return Ok(dot);
        }

        // POST: api/DotGiamGias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DotGiamGiaCreateDto dto)
        {
            if (dto == null || dto.Dot == null)
                return BadRequest("Dữ liệu không hợp lệ");

            var success = await _repository.CreateAsync(dto.Dot, dto.ChiTietIds ?? new List<Guid>());
            return success ? Ok(new { Success = true }) : BadRequest("Tạo thất bại (có thể do ngày không hợp lệ)");
        }

        // PUT: api/DotGiamGias/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DotGiamGia model)
        {
            if (id != model.IDDotGiamGia) return BadRequest("ID không khớp");

            var success = await _repository.UpdateAsync(model);
            return success ? Ok(new { Success = true }) : BadRequest("Cập nhật thất bại (có thể do ngày không hợp lệ)");
        }

        // DELETE: api/DotGiamGias/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _repository.DeleteAsync(id);
            return success ? Ok(new { Success = true }) : NotFound();
        }

        // PUT: api/DotGiamGias/UpdateTrangThai?id=GUID&trangThai=true
        [HttpPut("UpdateTrangThai")]
        public async Task<IActionResult> UpdateTrangThai([FromQuery] Guid id, [FromQuery] bool trangThai)
        {
            var success = await _repository.UpdateTrangThaiAsync(id, trangThai);
            return success ? Ok(new { Success = true }) : NotFound();
        }

    }
}
