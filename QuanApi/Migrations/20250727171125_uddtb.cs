using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class uddtb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhSanPhams_SanPhams_IDSanPham",
                table: "AnhSanPhams");

            migrationBuilder.DropColumn(
                name: "PhiVanChuyen",
                table: "HoaDons");

            migrationBuilder.RenameColumn(
                name: "IDSanPham",
                table: "AnhSanPhams",
                newName: "IDSanPhamChiTiet");

            migrationBuilder.RenameIndex(
                name: "IX_AnhSanPhams_IDSanPham",
                table: "AnhSanPhams",
                newName: "IX_AnhSanPhams_IDSanPhamChiTiet");

            migrationBuilder.InsertData(
                table: "PhuongThucThanhToans",
                columns: new[] { "IDPhuongThucThanhToan", "LanCapNhatCuoi", "MaPhuongThuc", "MoTa", "NgayTao", "NguoiCapNhat", "NguoiTao", "TenPhuongThuc", "TrangThai" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "TIEN_MAT", "Thanh toán bằng tiền mặt khi nhận hàng", new DateTime(2025, 7, 27, 17, 11, 24, 298, DateTimeKind.Utc).AddTicks(8850), null, "System", "Thanh toán tiền mặt", true },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, "CHUYEN_KHOAN", "Thanh toán bằng chuyển khoản ngân hàng", new DateTime(2025, 7, 27, 17, 11, 24, 298, DateTimeKind.Utc).AddTicks(8853), null, "System", "Chuyển khoản ngân hàng", true },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, "VIETTEL_PAY", "Thanh toán qua ví điện tử ViettelPay", new DateTime(2025, 7, 27, 17, 11, 24, 298, DateTimeKind.Utc).AddTicks(8856), null, "System", "ViettelPay", true },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, "MOMO", "Thanh toán qua ví điện tử MoMo", new DateTime(2025, 7, 27, 17, 11, 24, 298, DateTimeKind.Utc).AddTicks(8858), null, "System", "MoMo", true }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AnhSanPhams_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "AnhSanPhams",
                column: "IDSanPhamChiTiet",
                principalTable: "SanPhamChiTiets",
                principalColumn: "IDSanPhamChiTiet",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhSanPhams_SanPhamChiTiets_IDSanPhamChiTiet",
                table: "AnhSanPhams");

            migrationBuilder.DeleteData(
                table: "PhuongThucThanhToans",
                keyColumn: "IDPhuongThucThanhToan",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "PhuongThucThanhToans",
                keyColumn: "IDPhuongThucThanhToan",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "PhuongThucThanhToans",
                keyColumn: "IDPhuongThucThanhToan",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "PhuongThucThanhToans",
                keyColumn: "IDPhuongThucThanhToan",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.RenameColumn(
                name: "IDSanPhamChiTiet",
                table: "AnhSanPhams",
                newName: "IDSanPham");

            migrationBuilder.RenameIndex(
                name: "IX_AnhSanPhams_IDSanPhamChiTiet",
                table: "AnhSanPhams",
                newName: "IX_AnhSanPhams_IDSanPham");

            migrationBuilder.AddColumn<decimal>(
                name: "PhiVanChuyen",
                table: "HoaDons",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnhSanPhams_SanPhams_IDSanPham",
                table: "AnhSanPhams",
                column: "IDSanPham",
                principalTable: "SanPhams",
                principalColumn: "IDSanPham",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
