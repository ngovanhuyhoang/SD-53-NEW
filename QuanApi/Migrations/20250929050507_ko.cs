using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class ko : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "NhanViens");

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    IDEmailLog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ToEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhieuGiamGiaIDPhieuGiamGia = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HoaDonIDHoaDon = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.IDEmailLog);
                    table.ForeignKey(
                        name: "FK_EmailLogs_HoaDons_HoaDonIDHoaDon",
                        column: x => x.HoaDonIDHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "IDHoaDon");
                    table.ForeignKey(
                        name: "FK_EmailLogs_PhieuGiamGias_PhieuGiamGiaIDPhieuGiamGia",
                        column: x => x.PhieuGiamGiaIDPhieuGiamGia,
                        principalTable: "PhieuGiamGias",
                        principalColumn: "IDPhieuGiamGia");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_HoaDonIDHoaDon",
                table: "EmailLogs",
                column: "HoaDonIDHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_PhieuGiamGiaIDPhieuGiamGia",
                table: "EmailLogs",
                column: "PhieuGiamGiaIDPhieuGiamGia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "NhanViens",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
