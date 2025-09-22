using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableCarID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerID",
                table: "Tbl_Cars",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars",
                column: "CustomerID",
                principalTable: "Tbl_Customers",
                principalColumn: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerID",
                table: "Tbl_Cars",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars",
                column: "CustomerID",
                principalTable: "Tbl_Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
