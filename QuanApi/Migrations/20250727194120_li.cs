using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class li : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<Guid>(
                name: "ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThuVienAnhs",
                columns: table => new
                {
                    IDThuVienAnh = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaAnh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UrlAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LoaiAnh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KichThuoc = table.Column<long>(type: "bigint", nullable: false),
                    DinhDang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChieuRong = table.Column<int>(type: "int", nullable: false),
                    ChieuCao = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LanCapNhatCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    LaAnhChinh = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuVienAnhs", x => x.IDThuVienAnh);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnhSanPhams_ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams",
                column: "ThuVienAnhIDThuVienAnh");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhSanPhams_ThuVienAnhs_ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams",
                column: "ThuVienAnhIDThuVienAnh",
                principalTable: "ThuVienAnhs",
                principalColumn: "IDThuVienAnh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhSanPhams_ThuVienAnhs_ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams");

            migrationBuilder.DropTable(
                name: "ThuVienAnhs");

            migrationBuilder.DropIndex(
                name: "IX_AnhSanPhams_ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams");

            migrationBuilder.DropColumn(
                name: "ThuVienAnhIDThuVienAnh",
                table: "AnhSanPhams");

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
        }
    }
}
