using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCarnameCarmodal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Tbl_Cars");

            migrationBuilder.DropColumn(
                name: "CarName",
                table: "Tbl_Cars");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Tbl_Cars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Tbl_Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarName",
                table: "Tbl_Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Tbl_Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
