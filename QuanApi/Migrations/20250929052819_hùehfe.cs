using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class hùehfe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    IDEmailLog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoaDonIDHoaDon = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhieuGiamGiaIDPhieuGiamGia = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ToEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
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
    }
}
