using BanQuanAu1.Web.Data;
using QuanApi.Data;
using QuanApi.Dtos;
using QuanApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanApi.Repository
{
    public class DotGiamGiaRepository : DotGiamGiaIRepository
    {
        private readonly BanQuanAu1DbContext _context;

        public DotGiamGiaRepository(BanQuanAu1DbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultGeneric<DotGiamGia>> GetDotGiamGia(string maDot, string tenDot, int? phanTramGiam,
            DateTime? tuNgay, DateTime? denNgay, string trangThai, int page, int pageSize)
        {
            var query = _context.DotGiamGias.AsQueryable();

            if (!string.IsNullOrEmpty(maDot))
                query = query.Where(x => x.MaDot.Contains(maDot));

            if (!string.IsNullOrEmpty(tenDot))
                query = query.Where(x => x.TenDot.Contains(tenDot));

            if (phanTramGiam.HasValue)
                query = query.Where(x => x.PhanTramGiam == phanTramGiam.Value);

            if (tuNgay.HasValue)
                query = query.Where(x => x.NgayBatDau >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(x => x.NgayKetThuc <= denNgay.Value);

            if (!string.IsNullOrEmpty(trangThai))
            {
                bool tt = trangThai == "true";
                query = query.Where(x => x.TrangThai == tt);
            }

            var total = await query.CountAsync();
            var data = await query.OrderByDescending(x => x.NgayTao)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return new PagedResultGeneric<DotGiamGia>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<DotGiamGia> GetByIdAsync(Guid id) => await _context.DotGiamGias.FindAsync(id);

        public async Task<bool> CreateAsync(DotGiamGia dot, List<Guid> chiTietIds)
        {
            if (dot.NgayKetThuc < dot.NgayBatDau || dot.NgayBatDau < DateTime.Today)
                return false;

            dot.IDDotGiamGia = Guid.NewGuid();
            dot.NgayTao = DateTime.Now;
            dot.TrangThai = true;

            _context.DotGiamGias.Add(dot);

            if (chiTietIds?.Count > 0)
            {
                var chiTiets = await _context.SanPhamChiTiets
                                             .Where(x => chiTietIds.Contains(x.IDSanPhamChiTiet))
                                             .ToListAsync();

                foreach (var ct in chiTiets)
                {
                    _context.SanPhamDotGiams.Add(new SanPhamDotGiam
                    {
                        IDSanPhamDotGiam = Guid.NewGuid(),
                        IDDotGiamGia = dot.IDDotGiamGia,
                        IDSanPhamChiTiet = ct.IDSanPhamChiTiet,
                        MaSanPhamDotGiam = "SPDG_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                        GiaGoc = ct.GiaBan,
                        NgayTao = DateTime.Now,
                        TrangThai = true
                    });

                    ct.GiaBan = Math.Max(0, ct.GiaBan - ct.GiaBan * dot.PhanTramGiam / 100m);
                    ct.IDDotGiamGia = dot.IDDotGiamGia;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(DotGiamGia dot)
        {
            if (dot.NgayKetThuc < dot.NgayBatDau || dot.NgayBatDau < DateTime.Today)
                return false;

            dot.LanCapNhatCuoi = DateTime.Now;
            _context.DotGiamGias.Update(dot);

            var spGiams = await _context.SanPhamDotGiams
                                        .Where(x => x.IDDotGiamGia == dot.IDDotGiamGia)
                                        .ToListAsync();

            var chiTietIds = spGiams.Select(x => x.IDSanPhamChiTiet).ToList();
            var chiTiets = await _context.SanPhamChiTiets
                                         .Where(x => chiTietIds.Contains(x.IDSanPhamChiTiet))
                                         .ToDictionaryAsync(x => x.IDSanPhamChiTiet);

            foreach (var sp in spGiams)
            {
                if (chiTiets.TryGetValue(sp.IDSanPhamChiTiet, out var ct))
                {
                    ct.GiaBan = Math.Max(0, sp.GiaGoc - sp.GiaGoc * dot.PhanTramGiam / 100m);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var dot = await _context.DotGiamGias.FindAsync(id);
            if (dot == null) return false;

            var spGiams = await _context.SanPhamDotGiams
                                        .Where(x => x.IDDotGiamGia == id)
                                        .ToListAsync();

            var chiTietIds = spGiams.Select(x => x.IDSanPhamChiTiet).ToList();
            var chiTiets = await _context.SanPhamChiTiets
                                         .Where(x => chiTietIds.Contains(x.IDSanPhamChiTiet))
                                         .ToDictionaryAsync(x => x.IDSanPhamChiTiet);

            foreach (var sp in spGiams)
            {
                if (chiTiets.TryGetValue(sp.IDSanPhamChiTiet, out var ct))
                {
                    ct.GiaBan = sp.GiaGoc;
                    ct.IDDotGiamGia = null;
                }
            }

            _context.SanPhamDotGiams.RemoveRange(spGiams);
            _context.DotGiamGias.Remove(dot);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateTrangThaiAsync(Guid id, bool trangThai)
        {
            var dot = await _context.DotGiamGias.FindAsync(id);
            if (dot == null) return false;

            dot.TrangThai = trangThai;
            dot.LanCapNhatCuoi = DateTime.Now;

            var spGiams = await _context.SanPhamDotGiams
                                        .Where(x => x.IDDotGiamGia == id)
                                        .ToListAsync();

            var chiTietIds = spGiams.Select(x => x.IDSanPhamChiTiet).ToList();
            var chiTiets = await _context.SanPhamChiTiets
                                         .Where(x => chiTietIds.Contains(x.IDSanPhamChiTiet))
                                         .ToDictionaryAsync(x => x.IDSanPhamChiTiet);

            foreach (var sp in spGiams)
            {
                if (chiTiets.TryGetValue(sp.IDSanPhamChiTiet, out var ct))
                {
                    if (trangThai)
                    {
                        ct.GiaBan = Math.Max(0, sp.GiaGoc - sp.GiaGoc * dot.PhanTramGiam / 100m);
                        ct.IDDotGiamGia = id;
                    }
                    else
                    {
                        ct.GiaBan = sp.GiaGoc;
                        ct.IDDotGiamGia = null;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
