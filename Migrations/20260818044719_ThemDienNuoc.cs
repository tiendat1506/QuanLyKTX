using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKTX.Migrations
{
    /// <inheritdoc />
    public partial class ThemDienNuoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiSoDienNuocs",
                columns: table => new
                {
                    MaChiSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    Thang = table.Column<int>(type: "int", nullable: false),
                    Nam = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienCu = table.Column<int>(type: "int", nullable: false),
                    ChiSoDienMoi = table.Column<int>(type: "int", nullable: false),
                    ChiSoNuocCu = table.Column<int>(type: "int", nullable: false),
                    ChiSoNuocMoi = table.Column<int>(type: "int", nullable: false),
                    NgayGhi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiSoDienNuocs", x => x.MaChiSo);
                    table.ForeignKey(
                        name: "FK_ChiSoDienNuocs_Phongs_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "Phongs",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiSoDienNuocs_MaPhong_Thang_Nam",
                table: "ChiSoDienNuocs",
                columns: new[] { "MaPhong", "Thang", "Nam" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiSoDienNuocs");
        }
    }
}
