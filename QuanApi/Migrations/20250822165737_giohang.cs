using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class giohang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IDKhachHang",
                table: "GioHangs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IDKhachHang",
                table: "GioHangs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GioHangs_KhachHang_IDKhachHang",
                table: "GioHangs",
                column: "IDKhachHang",
                principalTable: "KhachHang",
                principalColumn: "IDKhachHang",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
