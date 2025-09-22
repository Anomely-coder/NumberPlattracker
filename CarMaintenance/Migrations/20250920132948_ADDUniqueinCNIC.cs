using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class ADDUniqueinCNIC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CNIC",
                table: "Tbl_Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Customers_CNIC",
                table: "Tbl_Customers",
                column: "CNIC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tbl_Customers_CNIC",
                table: "Tbl_Customers");

            migrationBuilder.AlterColumn<string>(
                name: "CNIC",
                table: "Tbl_Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
