using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BanQuanAu1.Web.Data;
using QuanApi.Data;

namespace QuanView.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanViensController : Controller
    {
        private readonly BanQuanAu1DbContext _context;

        public NhanViensController(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        // GET: Admin/NhanViens
        public async Task<IActionResult> Index()
        {
            var banQuanAu1DbContext = _context.NhanViens.Include(n => n.VaiTro);
            return View(await banQuanAu1DbContext.ToListAsync());
        }

        // GET: Admin/NhanViens/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.VaiTro)
                .FirstOrDefaultAsync(m => m.IDNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Create
        public IActionResult Create()
        {
            ViewData["IDVaiTro"] = new SelectList(_context.Set<VaiTro>(), "IDVaiTro", "MaVaiTro");
            return View();
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDNhanVien,MaNhanVien,TenNhanVien,Email,MatKhau,NgaySinh,GioiTinh,QueQuan,CCCD,SoDienThoai,NgayTao,NguoiTao,LanCapNhatCuoi,NguoiCapNhat,TrangThai,IDVaiTro")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                nhanVien.IDNhanVien = Guid.NewGuid();
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDVaiTro"] = new SelectList(_context.Set<VaiTro>(), "IDVaiTro", "MaVaiTro", nhanVien.IDVaiTro);
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            ViewData["IDVaiTro"] = new SelectList(_context.Set<VaiTro>(), "IDVaiTro", "MaVaiTro", nhanVien.IDVaiTro);
            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IDNhanVien,MaNhanVien,TenNhanVien,Email,MatKhau,NgaySinh,GioiTinh,QueQuan,CCCD,SoDienThoai,NgayTao,NguoiTao,LanCapNhatCuoi,NguoiCapNhat,TrangThai,IDVaiTro")] NhanVien nhanVien)
        {
            if (id != nhanVien.IDNhanVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.IDNhanVien))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IDVaiTro"] = new SelectList(_context.Set<VaiTro>(), "IDVaiTro", "MaVaiTro", nhanVien.IDVaiTro);
            return View(nhanVien);
        }

        // GET: Admin/NhanViens/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .Include(n => n.VaiTro)
                .FirstOrDefaultAsync(m => m.IDNhanVien == id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(Guid id)
        {
            return _context.NhanViens.Any(e => e.IDNhanVien == id);
        }
    }
}
