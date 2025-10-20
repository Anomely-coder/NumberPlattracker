using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddoneTomanyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Customers_CarID",
                table: "Tbl_Customers");

            migrationBuilder.DropColumn(
                name: "CarID",
                table: "Tbl_Customers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Tbl_Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Cars_CustomerID",
                table: "Tbl_Cars",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars",
                column: "CustomerID",
                principalTable: "Tbl_Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tbl_Cars_Tbl_Customers_CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.DropIndex(
                name: "IX_Tbl_Cars_CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.AddColumn<int>(
                name: "CarID",
                table: "Tbl_Customers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Customers_CarID",
                table: "Tbl_Customers",
                column: "CarID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                table: "Tbl_Customers",
                column: "CarID",
                principalTable: "Tbl_Cars",
                principalColumn: "CarID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
