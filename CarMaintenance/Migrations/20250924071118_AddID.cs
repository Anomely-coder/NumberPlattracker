using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tbl_Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "EmiratesID",
                table: "Tbl_Customers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers",
                column: "CarID",
                principalTable: "Tbl_Cars",
                principalColumn: "CarID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers");

            migrationBuilder.DropColumn(
                name: "EmiratesID",
                table: "Tbl_Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tbl_Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers",
                column: "CarID",
                principalTable: "Tbl_Cars",
                principalColumn: "CarID");
        }
    }
}
