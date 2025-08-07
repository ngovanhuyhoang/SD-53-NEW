using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanApi.Data;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BanQuanAu1.Web.Data
{
    public class BanQuanAu1DbContext : DbContext
    {
        public BanQuanAu1DbContext(DbContextOptions<BanQuanAu1DbContext> options)
            : base(options)
        {
        }

        // DbSet cho tất cả các bảng
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<ChatLieu> ChatLieus { get; set; }
        public DbSet<LoaiOng> LoaiOngs { get; set; }
        public DbSet<KieuDang> KieuDangs { get; set; }
        public DbSet<LungQuan> LungQuans { get; set; }
        public DbSet<KichCo> KichCos { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<DotGiamGia> DotGiamGias { get; set; }
        public DbSet<PhieuGiamGia> PhieuGiamGias { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<DiaChi> DiaChis { get; set; }
        public DbSet<KhachHangPhieuGiam> KhachHangPhieuGiams { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public DbSet<AnhSanPham> AnhSanPhams { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<LichSuHoaDon> LichSuHoaDons { get; set; }
        public DbSet<PhongTroChuyen> PhongTroChuyens { get; set; }
        public DbSet<TinNhan> TinNhans { get; set; }
        public DbSet<SanPhamDotGiam> SanPhamDotGiams { get; set; }
        public DbSet<Banner> Banners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.VaiTro)
                .WithMany(vt => vt.NhanViens)
                .HasForeignKey(nv => nv.IDVaiTro);

            // Seed data
        }
        public DbSet<QuanApi.Data.HoaTiet> HoaTiet { get; set; } = default!;
        public DbSet<QuanApi.Data.VaiTro> VaiTro { get; set; } = default!;
    }
}