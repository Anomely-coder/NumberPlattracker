using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailtoCNIC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tbl_Customers_CNIC",
                table: "Tbl_Customers");

            migrationBuilder.DropColumn(
                name: "CNIC",
                table: "Tbl_Customers");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Tbl_Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Tbl_Customers");

            migrationBuilder.AddColumn<string>(
                name: "CNIC",
                table: "Tbl_Customers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Customers_CNIC",
                table: "Tbl_Customers",
                column: "CNIC",
                unique: true);
        }
    }
}
