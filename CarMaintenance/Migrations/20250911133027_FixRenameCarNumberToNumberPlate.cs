using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    public partial class FixRenameCarNumberToNumberPlate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CarNumber",
                table: "Tbl_Cars",
                newName: "NumberPlate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberPlate",
                table: "Cars",
                newName: "CarNumber");
        }
    }
}
