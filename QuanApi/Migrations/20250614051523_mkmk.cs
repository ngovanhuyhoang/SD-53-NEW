using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class mkmk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IDHoaTiet",
                table: "SanPhamChiTiets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HoaTiet",
                columns: table => new
                {
                    IDHoaTiet = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaHoaTiet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenHoaTiet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LanCapNhatCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaTiet", x => x.IDHoaTiet);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_IDHoaTiet",
                table: "SanPhamChiTiets",
                column: "IDHoaTiet");

            migrationBuilder.AddForeignKey(
                name: "FK_SanPhamChiTiets_HoaTiet_IDHoaTiet",
                table: "SanPhamChiTiets",
                column: "IDHoaTiet",
                principalTable: "HoaTiet",
                principalColumn: "IDHoaTiet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SanPhamChiTiets_HoaTiet_IDHoaTiet",
                table: "SanPhamChiTiets");

            migrationBuilder.DropTable(
                name: "HoaTiet");

            migrationBuilder.DropIndex(
                name: "IX_SanPhamChiTiets_IDHoaTiet",
                table: "SanPhamChiTiets");

            migrationBuilder.DropColumn(
                name: "IDHoaTiet",
                table: "SanPhamChiTiets");
        }
    }
}
