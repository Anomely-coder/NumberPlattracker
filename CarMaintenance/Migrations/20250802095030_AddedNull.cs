using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddedNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers");

            migrationBuilder.AlterColumn<int>(
                name: "CarID",
                table: "Tbl_Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers",
                column: "CarID",
                principalTable: "Tbl_Cars",
                principalColumn: "CarID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers");

            migrationBuilder.AlterColumn<int>(
                name: "CarID",
                table: "Tbl_Customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers",
                column: "CarID",
                principalTable: "Tbl_Cars",
                principalColumn: "CarID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
