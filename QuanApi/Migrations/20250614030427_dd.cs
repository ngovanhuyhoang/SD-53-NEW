using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanApi.Migrations
{
    /// <inheritdoc />
    public partial class dd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhanViens_VaiTro_VaiTroIDVaiTro",
                table: "NhanViens");

            migrationBuilder.DropIndex(
                name: "IX_NhanViens_VaiTroIDVaiTro",
                table: "NhanViens");

            migrationBuilder.DropColumn(
                name: "VaiTroIDVaiTro",
                table: "NhanViens");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_IDVaiTro",
                table: "NhanViens",
                column: "IDVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_NhanViens_VaiTro_IDVaiTro",
                table: "NhanViens",
                column: "IDVaiTro",
                principalTable: "VaiTro",
                principalColumn: "IDVaiTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhanViens_VaiTro_IDVaiTro",
                table: "NhanViens");

            migrationBuilder.DropIndex(
                name: "IX_NhanViens_IDVaiTro",
                table: "NhanViens");

            migrationBuilder.AddColumn<Guid>(
                name: "VaiTroIDVaiTro",
                table: "NhanViens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_VaiTroIDVaiTro",
                table: "NhanViens",
                column: "VaiTroIDVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_NhanViens_VaiTro_VaiTroIDVaiTro",
                table: "NhanViens",
                column: "VaiTroIDVaiTro",
                principalTable: "VaiTro",
                principalColumn: "IDVaiTro");
        }
    }
}
