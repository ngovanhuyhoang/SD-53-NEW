using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class jnjn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhSanPhams_SanPhams_SanPhamIDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietGioHangs_GioHangs_GioHangIDGioHang",
                table: "ChiTietGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietGioHangs_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_HoaDonIDHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDons_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_KhachHang_KhachHangIDKhachHang",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_KhachHang_KhachHangIDKhachHang",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienIDNhanVien",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_PhieuGiamGias_PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_PhuongThucThanhToans_PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangPhieuGiams_KhachHang_KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangPhieuGiams_PhieuGiamGias_PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_LichSuHoaDons_HoaDons_HoaDonIDHoaDon",
                table: "LichSuHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_PhongTroChuyens_KhachHang_KhachHangIDKhachHang",
                table: "PhongTroChuyens");

            migrationBuilder.DropForeignKey(
                name: "FK_PhongTroChuyens_NhanViens_NhanVienIDNhanVien",
                table: "PhongTroChuyens");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_DotGiamGias_DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_KichCos_KichCoIDKichCo",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_MauSacs_MauSacIDMauSac",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_SanPhams_SanPhamIDSanPham",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamDotGiams_DotGiamGias_DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamDotGiams_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_ChatLieus_ChatLieuIDChatLieu",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_DanhMucs_DanhMucIDDanhMuc",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_KieuDangs_KieuDangIDKieuDang",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_LoaiOngs_LoaiOngIDLoaiOng",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_LungQuans_LungQuanIDLungQuan",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_ThuongHieus_ThuongHieuIDThuongHieu",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_KhachHang_KhachHangIDKhachHang",
                table: "TinNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_NhanViens_NhanVienIDNhanVien",
                table: "TinNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_PhongTroChuyens_PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_KhachHangIDKhachHang",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_NhanVienIDNhanVien",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_ChatLieuIDChatLieu",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_DanhMucIDDanhMuc",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_KieuDangIDKieuDang",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_LoaiOngIDLoaiOng",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_LungQuanIDLungQuan",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_ThuongHieuIDThuongHieu",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamDotGiams_DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamDotGiams_SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_KichCoIDKichCo",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_MauSacIDMauSac",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_SanPhamIDSanPham",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_PhongTroChuyens_KhachHangIDKhachHang",
                table: "PhongTroChuyens");

            migrationBuilder.DropIndex(
                name: "IX_PhongTroChuyens_NhanVienIDNhanVien",
                table: "PhongTroChuyens");

            migrationBuilder.DropIndex(
                name: "IX_LichSuHoaDons_HoaDonIDHoaDon",
                table: "LichSuHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangPhieuGiams_KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangPhieuGiams_PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_KhachHangIDKhachHang",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_NhanVienIDNhanVien",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_GioHangs_KhachHangIDKhachHang",
                table: "GioHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDons_HoaDonIDHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDons_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietGioHangs_GioHangIDGioHang",
                table: "ChiTietGioHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietGioHangs_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs");

            migrationBuilder.DropIndex(
                name: "IX_AnhSanPhams_SanPhamIDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.DropColumn(
                name: "KhachHangIDKhachHang",
                table: "TinNhans");

            migrationBuilder.DropColumn(
                name: "NhanVienIDNhanVien",
                table: "TinNhans");

            migrationBuilder.DropColumn(
                name: "PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans");

            migrationBuilder.DropColumn(
                name: "ChatLieuIDChatLieu",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "DanhMucIDDanhMuc",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "KieuDangIDKieuDang",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "LoaiOngIDLoaiOng",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "LungQuanIDLungQuan",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "ThuongHieuIDThuongHieu",
                table: "SanPhams");

            migrationBuilder.DropColumn(
                name: "DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams");

            migrationBuilder.DropColumn(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams");

            migrationBuilder.DropColumn(
                name: "DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets");

            migrationBuilder.DropColumn(
                name: "KichCoIDKichCo",
                table: "SanPhamChiTiets");

            migrationBuilder.DropColumn(
                name: "MauSacIDMauSac",
                table: "SanPhamChiTiets");

            migrationBuilder.DropColumn(
                name: "SanPhamIDSanPham",
                table: "SanPhamChiTiets");

            migrationBuilder.DropColumn(
                name: "KhachHangIDKhachHang",
                table: "PhongTroChuyens");

            migrationBuilder.DropColumn(
                name: "NhanVienIDNhanVien",
                table: "PhongTroChuyens");

            migrationBuilder.DropColumn(
                name: "HoaDonIDHoaDon",
                table: "LichSuHoaDons");

            migrationBuilder.DropColumn(
                name: "KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropColumn(
                name: "PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropColumn(
                name: "KhachHangIDKhachHang",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "NhanVienIDNhanVien",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "KhachHangIDKhachHang",
                table: "GioHangs");

            migrationBuilder.DropColumn(
                name: "HoaDonIDHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons");

            migrationBuilder.DropColumn(
                name: "GioHangIDGioHang",
                table: "ChiTietGioHangs");

            migrationBuilder.DropColumn(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs");

            migrationBuilder.DropColumn(
                name: "SanPhamIDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_IDKhachHang",
                table: "TinNhans",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_IDNhanVien",
                table: "TinNhans",
                column: "IDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_IDPhongTroChuyen",
                table: "TinNhans",
                column: "IDPhongTroChuyen");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDChatLieu",
                table: "SanPhams",
                column: "IDChatLieu");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDDanhMuc",
                table: "SanPhams",
                column: "IDDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDKieuDang",
                table: "SanPhams",
                column: "IDKieuDang");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDLoaiOng",
                table: "SanPhams",
                column: "IDLoaiOng");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDLungQuan",
                table: "SanPhams",
                column: "IDLungQuan");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_IDThuongHieu",
                table: "SanPhams",
                column: "IDThuongHieu");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamDotGiams_IDDotGiamGia",
                table: "SanPhamDotGiams",
                column: "IDDotGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamDotGiams_IDSanPhamChiTiet",
                table: "SanPhamDotGiams",
                column: "IDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_IDDotGiamGia",
                table: "SanPhamChiTiets",
                column: "IDDotGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_IDKichCo",
                table: "SanPhamChiTiets",
                column: "IDKichCo");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_IDMauSac",
                table: "SanPhamChiTiets",
                column: "IDMauSac");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_IDSanPham",
                table: "SanPhamChiTiets",
                column: "IDSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTroChuyens_IDKhachHang",
                table: "PhongTroChuyens",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTroChuyens_IDNhanVien",
                table: "PhongTroChuyens",
                column: "IDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoaDons_IDHoaDon",
                table: "LichSuHoaDons",
                column: "IDHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangPhieuGiams_IDKhachHang",
                table: "KhachHangPhieuGiams",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangPhieuGiams_IDPhieuGiamGia",
                table: "KhachHangPhieuGiams",
                column: "IDPhieuGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_IDKhachHang",
                table: "HoaDons",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_IDNhanVien",
                table: "HoaDons",
                column: "IDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_IDPhieuGiamGia",
                table: "HoaDons",
                column: "IDPhieuGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_IDPhuongThucThanhToan",
                table: "HoaDons",
                column: "IDPhuongThucThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_IDKhachHang",
                table: "GioHangs",
                column: "IDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_IDHoaDon",
                table: "ChiTietHoaDons",
                column: "IDHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_IDSanPhamChiTiet",
                table: "ChiTietHoaDons",
                column: "IDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_IDGioHang",
                table: "ChiTietGioHangs",
                column: "IDGioHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_IDSanPhamChiTiet",
                table: "ChiTietGioHangs",
                column: "IDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_AnhSanPhams_IDSanPham",
                table: "AnhSanPhams",
                column: "IDSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhSanPhams_SanPhams_IDSanPham",
                table: "AnhSanPhams",
                column: "IDSanPham",
                principalTable: "SanPhams",
                principalColumn: "IDSanPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietGioHangs_GioHangs_IDGioHang",
                table: "ChiTietGioHangs",
                column: "IDGioHang",
                principalTable: "GioHangs",
                principalColumn: "IDGioHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietGioHangs_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "ChiTietGioHangs",
                column: "IDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_IDHoaDon",
                table: "ChiTietHoaDons",
                column: "IDHoaDon",
                principalTable: "HoaDons",
                principalColumn: "IDHoaDon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDons_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "ChiTietHoaDons",
                column: "IDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_KhachHang_IDKhachHang",
                table: "HoaDons",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_IDNhanVien",
                table: "HoaDons",
                column: "IDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_PhieuGiamGias_IDPhieuGiamGia",
                table: "HoaDons",
                column: "IDPhieuGiamGia",
                principalTable: "PhieuGiamGias",
                principalColumn: "IDPhieuGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_PhuongThucThanhToans_IDPhuongThucThanhToan",
                table: "HoaDons",
                column: "IDPhuongThucThanhToan",
                principalTable: "PhuongThucThanhToans",
                principalColumn: "IDPhuongThucThanhToan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangPhieuGiams_KhachHang_IDKhachHang",
                table: "KhachHangPhieuGiams",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangPhieuGiams_PhieuGiamGias_IDPhieuGiamGia",
                table: "KhachHangPhieuGiams",
                column: "IDPhieuGiamGia",
                principalTable: "PhieuGiamGias",
                principalColumn: "IDPhieuGiamGia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuHoaDons_HoaDons_IDHoaDon",
                table: "LichSuHoaDons",
                column: "IDHoaDon",
                principalTable: "HoaDons",
                principalColumn: "IDHoaDon",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTroChuyens_KhachHang_IDKhachHang",
                table: "PhongTroChuyens",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTroChuyens_NhanViens_IDNhanVien",
                table: "PhongTroChuyens",
                column: "IDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_DotGiamGias_IDDotGiamGia",
                table: "SanPhamChiTiets",
                column: "IDDotGiamGia",
                principalTable: "DotGiamGias",
                principalColumn: "IDDotGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_KichCos_IDKichCo",
                table: "SanPhamChiTiets",
                column: "IDKichCo",
                principalTable: "KichCos",
                principalColumn: "IDKichCo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_MauSacs_IDMauSac",
                table: "SanPhamChiTiets",
                column: "IDMauSac",
                principalTable: "MauSacs",
                principalColumn: "IDMauSac",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_SanPhams_IDSanPham",
                table: "SanPhamChiTiets",
                column: "IDSanPham",
                principalTable: "SanPhams",
                principalColumn: "IDSanPham",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamDotGiams_DotGiamGias_IDDotGiamGia",
                table: "SanPhamDotGiams",
                column: "IDDotGiamGia",
                principalTable: "DotGiamGias",
                principalColumn: "IDDotGiamGia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamDotGiams_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "SanPhamDotGiams",
                column: "IDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_ChatLieus_IDChatLieu",
                table: "SanPhams",
                column: "IDChatLieu",
                principalTable: "ChatLieus",
                principalColumn: "IDChatLieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_DanhMucs_IDDanhMuc",
                table: "SanPhams",
                column: "IDDanhMuc",
                principalTable: "DanhMucs",
                principalColumn: "IDDanhMuc",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_KieuDangs_IDKieuDang",
                table: "SanPhams",
                column: "IDKieuDang",
                principalTable: "KieuDangs",
                principalColumn: "IDKieuDang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_LoaiOngs_IDLoaiOng",
                table: "SanPhams",
                column: "IDLoaiOng",
                principalTable: "LoaiOngs",
                principalColumn: "IDLoaiOng",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_LungQuans_IDLungQuan",
                table: "SanPhams",
                column: "IDLungQuan",
                principalTable: "LungQuans",
                principalColumn: "IDLungQuan",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_ThuongHieus_IDThuongHieu",
                table: "SanPhams",
                column: "IDThuongHieu",
                principalTable: "ThuongHieus",
                principalColumn: "IDThuongHieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_KhachHang_IDKhachHang",
                table: "TinNhans",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_NhanViens_IDNhanVien",
                table: "TinNhans",
                column: "IDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_PhongTroChuyens_IDPhongTroChuyen",
                table: "TinNhans",
                column: "IDPhongTroChuyen",
                principalTable: "PhongTroChuyens",
                principalColumn: "IDPhongTroChuyen",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhSanPhams_SanPhams_IDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietGioHangs_GioHangs_IDGioHang",
                table: "ChiTietGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietGioHangs_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "ChiTietGioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_IDHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHoaDons_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "ChiTietHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_KhachHang_IDKhachHang",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_IDNhanVien",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_PhieuGiamGias_IDPhieuGiamGia",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_PhuongThucThanhToans_IDPhuongThucThanhToan",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangPhieuGiams_KhachHang_IDKhachHang",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangPhieuGiams_PhieuGiamGias_IDPhieuGiamGia",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_LichSuHoaDons_HoaDons_IDHoaDon",
                table: "LichSuHoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_PhongTroChuyens_KhachHang_IDKhachHang",
                table: "PhongTroChuyens");

            migrationBuilder.DropForeignKey(
                name: "FK_PhongTroChuyens_NhanViens_IDNhanVien",
                table: "PhongTroChuyens");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_DotGiamGias_IDDotGiamGia",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_KichCos_IDKichCo",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_MauSacs_IDMauSac",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_SanPhams_IDSanPham",
                table: "SanPhamChiTiets");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamDotGiams_DotGiamGias_IDDotGiamGia",
                table: "SanPhamDotGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamDotGiams_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "SanPhamDotGiams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_ChatLieus_IDChatLieu",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_DanhMucs_IDDanhMuc",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_KieuDangs_IDKieuDang",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_LoaiOngs_IDLoaiOng",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_LungQuans_IDLungQuan",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_SanPhams_ThuongHieus_IDThuongHieu",
                table: "SanPhams");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_KhachHang_IDKhachHang",
                table: "TinNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_NhanViens_IDNhanVien",
                table: "TinNhans");

            migrationBuilder.DropForeignKey(
                name: "FK_TinNhans_PhongTroChuyens_IDPhongTroChuyen",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_IDKhachHang",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_IDNhanVien",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_TinNhans_IDPhongTroChuyen",
                table: "TinNhans");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDChatLieu",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDDanhMuc",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDKieuDang",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDLoaiOng",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDLungQuan",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhams_IDThuongHieu",
                table: "SanPhams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamDotGiams_IDDotGiamGia",
                table: "SanPhamDotGiams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamDotGiams_IDSanPhamChiTiet",
                table: "SanPhamDotGiams");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_IDDotGiamGia",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_IDKichCo",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_IDMauSac",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_IDSanPham",
                table: "SanPhamChiTiets");

            migrationBuilder.DropIndex(
                name: "IX_PhongTroChuyens_IDKhachHang",
                table: "PhongTroChuyens");

            migrationBuilder.DropIndex(
                name: "IX_PhongTroChuyens_IDNhanVien",
                table: "PhongTroChuyens");

            migrationBuilder.DropIndex(
                name: "IX_LichSuHoaDons_IDHoaDon",
                table: "LichSuHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangPhieuGiams_IDKhachHang",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangPhieuGiams_IDPhieuGiamGia",
                table: "KhachHangPhieuGiams");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_IDKhachHang",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_IDNhanVien",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_IDPhieuGiamGia",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_IDPhuongThucThanhToan",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_GioHangs_IDKhachHang",
                table: "GioHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDons_IDHoaDon",
                table: "ChiTietHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHoaDons_IDSanPhamChiTiet",
                table: "ChiTietHoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietGioHangs_IDGioHang",
                table: "ChiTietGioHangs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietGioHangs_IDSanPhamChiTiet",
                table: "ChiTietGioHangs");

            migrationBuilder.DropIndex(
                name: "IX_AnhSanPhams_IDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangIDKhachHang",
                table: "TinNhans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NhanVienIDNhanVien",
                table: "TinNhans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChatLieuIDChatLieu",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DanhMucIDDanhMuc",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KieuDangIDKieuDang",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoaiOngIDLoaiOng",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LungQuanIDLungQuan",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThuongHieuIDThuongHieu",
                table: "SanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KichCoIDKichCo",
                table: "SanPhamChiTiets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MauSacIDMauSac",
                table: "SanPhamChiTiets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamIDSanPham",
                table: "SanPhamChiTiets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangIDKhachHang",
                table: "PhongTroChuyens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NhanVienIDNhanVien",
                table: "PhongTroChuyens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HoaDonIDHoaDon",
                table: "LichSuHoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangIDKhachHang",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NhanVienIDNhanVien",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhachHangIDKhachHang",
                table: "GioHangs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HoaDonIDHoaDon",
                table: "ChiTietHoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GioHangIDGioHang",
                table: "ChiTietGioHangs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SanPhamIDSanPham",
                table: "AnhSanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_KhachHangIDKhachHang",
                table: "TinNhans",
                column: "KhachHangIDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_NhanVienIDNhanVien",
                table: "TinNhans",
                column: "NhanVienIDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans",
                column: "PhongTroChuyenIDPhongTroChuyen");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_ChatLieuIDChatLieu",
                table: "SanPhams",
                column: "ChatLieuIDChatLieu");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_DanhMucIDDanhMuc",
                table: "SanPhams",
                column: "DanhMucIDDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_KieuDangIDKieuDang",
                table: "SanPhams",
                column: "KieuDangIDKieuDang");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_LoaiOngIDLoaiOng",
                table: "SanPhams",
                column: "LoaiOngIDLoaiOng");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_LungQuanIDLungQuan",
                table: "SanPhams",
                column: "LungQuanIDLungQuan");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_ThuongHieuIDThuongHieu",
                table: "SanPhams",
                column: "ThuongHieuIDThuongHieu");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamDotGiams_DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams",
                column: "DotGiamGiaIDDotGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamDotGiams_SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams",
                column: "SanPhamChiTietIDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets",
                column: "DotGiamGiaIDDotGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_KichCoIDKichCo",
                table: "SanPhamChiTiets",
                column: "KichCoIDKichCo");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_MauSacIDMauSac",
                table: "SanPhamChiTiets",
                column: "MauSacIDMauSac");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_SanPhamIDSanPham",
                table: "SanPhamChiTiets",
                column: "SanPhamIDSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTroChuyens_KhachHangIDKhachHang",
                table: "PhongTroChuyens",
                column: "KhachHangIDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_PhongTroChuyens_NhanVienIDNhanVien",
                table: "PhongTroChuyens",
                column: "NhanVienIDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuHoaDons_HoaDonIDHoaDon",
                table: "LichSuHoaDons",
                column: "HoaDonIDHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangPhieuGiams_KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams",
                column: "KhachHangIDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangPhieuGiams_PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams",
                column: "PhieuGiamGiaIDPhieuGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_KhachHangIDKhachHang",
                table: "HoaDons",
                column: "KhachHangIDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_NhanVienIDNhanVien",
                table: "HoaDons",
                column: "NhanVienIDNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons",
                column: "PhieuGiamGiaIDPhieuGiamGia");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons",
                column: "PhuongThucThanhToanIDPhuongThucThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_KhachHangIDKhachHang",
                table: "GioHangs",
                column: "KhachHangIDKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_HoaDonIDHoaDon",
                table: "ChiTietHoaDons",
                column: "HoaDonIDHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHoaDons_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons",
                column: "SanPhamChiTietIDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_GioHangIDGioHang",
                table: "ChiTietGioHangs",
                column: "GioHangIDGioHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs",
                column: "SanPhamChiTietIDSanPhamChiTiet");

            migrationBuilder.CreateIndex(
                name: "IX_AnhSanPhams_SanPhamIDSanPham",
                table: "AnhSanPhams",
                column: "SanPhamIDSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhSanPhams_SanPhams_SanPhamIDSanPham",
                table: "AnhSanPhams",
                column: "SanPhamIDSanPham",
                principalTable: "SanPhams",
                principalColumn: "IDSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietGioHangs_GioHangs_GioHangIDGioHang",
                table: "ChiTietGioHangs",
                column: "GioHangIDGioHang",
                principalTable: "GioHangs",
                principalColumn: "IDGioHang");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietGioHangs_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietGioHangs",
                column: "SanPhamChiTietIDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDons_HoaDons_HoaDonIDHoaDon",
                table: "ChiTietHoaDons",
                column: "HoaDonIDHoaDon",
                principalTable: "HoaDons",
                principalColumn: "IDHoaDon");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHoaDons_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "ChiTietHoaDons",
                column: "SanPhamChiTietIDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_KhachHang_KhachHangIDKhachHang",
                table: "GioHangs",
                column: "KhachHangIDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_KhachHang_KhachHangIDKhachHang",
                table: "HoaDons",
                column: "KhachHangIDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienIDNhanVien",
                table: "HoaDons",
                column: "NhanVienIDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_PhieuGiamGias_PhieuGiamGiaIDPhieuGiamGia",
                table: "HoaDons",
                column: "PhieuGiamGiaIDPhieuGiamGia",
                principalTable: "PhieuGiamGias",
                principalColumn: "IDPhieuGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_PhuongThucThanhToans_PhuongThucThanhToanIDPhuongThucThanhToan",
                table: "HoaDons",
                column: "PhuongThucThanhToanIDPhuongThucThanhToan",
                principalTable: "PhuongThucThanhToans",
                principalColumn: "IDPhuongThucThanhToan");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangPhieuGiams_KhachHang_KhachHangIDKhachHang",
                table: "KhachHangPhieuGiams",
                column: "KhachHangIDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangPhieuGiams_PhieuGiamGias_PhieuGiamGiaIDPhieuGiamGia",
                table: "KhachHangPhieuGiams",
                column: "PhieuGiamGiaIDPhieuGiamGia",
                principalTable: "PhieuGiamGias",
                principalColumn: "IDPhieuGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuHoaDons_HoaDons_HoaDonIDHoaDon",
                table: "LichSuHoaDons",
                column: "HoaDonIDHoaDon",
                principalTable: "HoaDons",
                principalColumn: "IDHoaDon");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTroChuyens_KhachHang_KhachHangIDKhachHang",
                table: "PhongTroChuyens",
                column: "KhachHangIDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_PhongTroChuyens_NhanViens_NhanVienIDNhanVien",
                table: "PhongTroChuyens",
                column: "NhanVienIDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_DotGiamGias_DotGiamGiaIDDotGiamGia",
                table: "SanPhamChiTiets",
                column: "DotGiamGiaIDDotGiamGia",
                principalTable: "DotGiamGias",
                principalColumn: "IDDotGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_KichCos_KichCoIDKichCo",
                table: "SanPhamChiTiets",
                column: "KichCoIDKichCo",
                principalTable: "KichCos",
                principalColumn: "IDKichCo");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_MauSacs_MauSacIDMauSac",
                table: "SanPhamChiTiets",
                column: "MauSacIDMauSac",
                principalTable: "MauSacs",
                principalColumn: "IDMauSac");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_SanPhams_SanPhamIDSanPham",
                table: "SanPhamChiTiets",
                column: "SanPhamIDSanPham",
                principalTable: "SanPhams",
                principalColumn: "IDSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamDotGiams_DotGiamGias_DotGiamGiaIDDotGiamGia",
                table: "SanPhamDotGiams",
                column: "DotGiamGiaIDDotGiamGia",
                principalTable: "DotGiamGias",
                principalColumn: "IDDotGiamGia");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamDotGiams_SanPhamChiTiets_SanPhamChiTietIDSanPhamChiTiet",
                table: "SanPhamDotGiams",
                column: "SanPhamChiTietIDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_ChatLieus_ChatLieuIDChatLieu",
                table: "SanPhams",
                column: "ChatLieuIDChatLieu",
                principalTable: "ChatLieus",
                principalColumn: "IDChatLieu");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_DanhMucs_DanhMucIDDanhMuc",
                table: "SanPhams",
                column: "DanhMucIDDanhMuc",
                principalTable: "DanhMucs",
                principalColumn: "IDDanhMuc");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_KieuDangs_KieuDangIDKieuDang",
                table: "SanPhams",
                column: "KieuDangIDKieuDang",
                principalTable: "KieuDangs",
                principalColumn: "IDKieuDang");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_LoaiOngs_LoaiOngIDLoaiOng",
                table: "SanPhams",
                column: "LoaiOngIDLoaiOng",
                principalTable: "LoaiOngs",
                principalColumn: "IDLoaiOng");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_LungQuans_LungQuanIDLungQuan",
                table: "SanPhams",
                column: "LungQuanIDLungQuan",
                principalTable: "LungQuans",
                principalColumn: "IDLungQuan");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhams_ThuongHieus_ThuongHieuIDThuongHieu",
                table: "SanPhams",
                column: "ThuongHieuIDThuongHieu",
                principalTable: "ThuongHieus",
                principalColumn: "IDThuongHieu");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_KhachHang_KhachHangIDKhachHang",
                table: "TinNhans",
                column: "KhachHangIDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_NhanViens_NhanVienIDNhanVien",
                table: "TinNhans",
                column: "NhanVienIDNhanVien",
                principalTable: "NhanViens",
                principalColumn: "IDNhanVien");

            migrationBuilder.AddForeignKey(
                name: "FK_TinNhans_PhongTroChuyens_PhongTroChuyenIDPhongTroChuyen",
                table: "TinNhans",
                column: "PhongTroChuyenIDPhongTroChuyen",
                principalTable: "PhongTroChuyens",
                principalColumn: "IDPhongTroChuyen");
        }
    }
}
