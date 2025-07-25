using BanQuanAu1.Web.Data;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using QuanApi.Dtos;
using QuanApi.Repository.IRepository;
using System.Linq;

namespace QuanApi.Repository
{
    public class GioHangRepository : GioHangIRepository
    {
        BanQuanAu1DbContext _db;
        public GioHangRepository(BanQuanAu1DbContext db)
        {
            _db = db;
        }
        public List<SanPhamKhachHangViewModel> ListSPCT(int pageNumber, int pageSize)
        {
            var all = _db.SanPhamChiTiets
                .Include(spct => spct.SanPham)
                    .ThenInclude(sp => sp.DanhMuc)
                .Include(spct => spct.KichCo)
                .Include(spct => spct.MauSac)
                .Include(spct => spct.DotGiamGia)
                .Include(spct => spct.SanPham.AnhSanPhams)
                .Where(spct => spct.TrangThai)
                .ToList();

            var grouped = all
                .GroupBy(spct => spct.IDSanPham) 
                .OrderBy(g => g.Key)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new SanPhamKhachHangViewModel
                {
                    TenSanPham = g.First().SanPham.TenSanPham,
                    DanhMuc = g.First().SanPham.DanhMuc.TenDanhMuc,
                    UrlAnh = g.First().SanPham.AnhSanPhams.Where(a => a.LaAnhChinh).Select(a => a.UrlAnh).FirstOrDefault(),
                    BienThes = g.Select(spct => new BienTheSanPhamViewModel
                    {
                        IDSanPhamChiTiet = spct.IDSanPhamChiTiet,
                        Size = spct.KichCo.TenKichCo,
                        Mau = spct.MauSac.TenMauSac,
                        GiaGoc = spct.GiaBan,
                        GiaSauGiam = spct.DotGiamGia != null && spct.DotGiamGia.NgayBatDau <= DateTime.Now && spct.DotGiamGia.NgayKetThuc >= DateTime.Now
                            ? spct.GiaBan * (1 - spct.DotGiamGia.PhanTramGiam / 100m)
                            : spct.GiaBan,
                        SoLuong = spct.SoLuong
                    }).ToList()
                })
                .ToList();

            return grouped;
        }


        public SanPhamChiTietDto detailSpct(Guid id)
        {
            var spct = _db.SanPhamChiTiets
                .Include(ct => ct.SanPham)
                    .ThenInclude(sp => sp.DanhMuc)
                .Include(ct => ct.KichCo)
                .Include(ct => ct.MauSac)
                .Include(ct => ct.SanPham.AnhSanPhams)
                .Include(ct => ct.DotGiamGia)
                .FirstOrDefault(ct => ct.IDSanPhamChiTiet == id);

            if (spct == null)
                throw new KeyNotFoundException("Không tìm thấy sản phẩm chi tiết.");

            return new SanPhamChiTietDto
            {
                IdSanPhamChiTiet = spct.IDSanPhamChiTiet,
                IdSanPham = spct.IDSanPham,
                TenSanPham = spct.SanPham?.TenSanPham ?? "",
                TenDanhMuc = spct.SanPham?.DanhMuc?.TenDanhMuc ?? "",
                AnhDaiDien = spct.SanPham?.AnhSanPhams?.Where(a => a.LaAnhChinh).Select(a => a.UrlAnh).FirstOrDefault() ?? "",
                TenKichCo = spct.KichCo?.TenKichCo ?? "",
                TenMauSac = spct.MauSac?.TenMauSac ?? "",
                GiaBan = spct.GiaBan,
                price = (spct.DotGiamGia != null && spct.DotGiamGia.NgayBatDau <= DateTime.Now && spct.DotGiamGia.NgayKetThuc >= DateTime.Now)
                    ? spct.GiaBan * (1 - spct.DotGiamGia.PhanTramGiam / 100m)
                    : spct.GiaBan,
                SoLuong = spct.SoLuong,
                TrangThai = spct.TrangThai
            };
        }

        public void AddGioHang(Guid iduser, Guid idsp, int soluong)
        {
            var nguoidung = _db.GioHangs.FirstOrDefault(gt => gt.IDKhachHang == iduser);
            if (nguoidung == null)
            {
                nguoidung = new GioHang
                {
                    IDKhachHang = iduser,
                    MaGioHang = "GH" + DateTime.Now.Ticks,
                    NgayTao = DateTime.Now,
                    TrangThai = true,
                    ChiTietGioHangs = new List<ChiTietGioHang>()
                };
                _db.GioHangs.Add(nguoidung);
                _db.SaveChanges();
            }
            var sp = _db.SanPhamChiTiets.Find(idsp);
            if (sp == null || soluong <= 0)
            {
                throw new ArgumentException("Món ăn hoặc số lượng không hợp lệ.");
            }
            var giohang = nguoidung.ChiTietGioHangs.FirstOrDefault(ghct => ghct.IDSanPhamChiTiet == idsp);
            if (giohang != null)
            {
                giohang.SoLuong += soluong;
                giohang.GiaBan = sp.GiaBan;
            }
            else
            {
                nguoidung.ChiTietGioHangs.Add(new Data.ChiTietGioHang
                {
                    IDSanPhamChiTiet = idsp,
                    SoLuong = soluong,
                    GiaBan = sp.GiaBan,
                });
            }
            _db.SaveChanges();
        }
        public GioHang GetByUserId(Guid userId)
        {
            return _db.GioHangs.FirstOrDefault(gt => gt.IDKhachHang == userId);
        }

        public void XoaChiTietGioHang(Guid idgiohang)
        {
            var ghct = _db.ChiTietGioHangs.FirstOrDefault(ghct => ghct.IDChiTietGioHang == idgiohang);
            if (ghct == null)
            {
                throw new KeyNotFoundException("giỏ hàng không ton tai");
            }
            _db.ChiTietGioHangs.Remove(ghct);
            _db.SaveChanges();
        }
        private decimal TinhGiaSauGiam(SanPhamChiTiet spct)
        {
            if (spct.DotGiamGia != null &&
                spct.DotGiamGia.NgayBatDau <= DateTime.Now &&
                spct.DotGiamGia.NgayKetThuc >= DateTime.Now)
            {
                return spct.GiaBan * (1 - spct.DotGiamGia.PhanTramGiam / 100);
            }
            return spct.GiaBan;
        }

        public void UpdateChiTietGioHang(Guid idghct, int soluong)
        {
            var ghct = _db.ChiTietGioHangs
                          .Include(ct => ct.SanPhamChiTiet)
                          .ThenInclude(sp => sp.DotGiamGia)
                          .FirstOrDefault(ct => ct.IDChiTietGioHang == idghct);

            if (ghct == null)
                throw new KeyNotFoundException("Không tìm thấy giỏ hàng");

            if (soluong <= 0)
                throw new ArgumentException("Số lượng không hợp lệ");

            ghct.SoLuong = soluong;
            ghct.GiaBan = TinhGiaSauGiam(ghct.SanPhamChiTiet);

            _db.SaveChanges();
        }

    }
}
