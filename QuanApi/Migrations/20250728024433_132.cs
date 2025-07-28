using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class _132 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    ChieuCao = table.Column<int>(type: "int", nullable: false),
                    ChieuRong = table.Column<int>(type: "int", nullable: false),
                    DinhDang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KichThuoc = table.Column<long>(type: "bigint", nullable: false),
                    LaAnhChinh = table.Column<bool>(type: "bit", nullable: false),
                    LanCapNhatCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoaiAnh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaAnh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiTao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    UrlAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
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
    }
}
