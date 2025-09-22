using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class ADDChangesTopopulateTheCarNumberInRecipt : Migration
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

            migrationBuilder.RenameColumn(
                name: "ReceiptsDetailsID",
                table: "Tbl_ReceiptDetails",
                newName: "ReceiptDetailID");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tbl_ReceiptDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Tbl_ReceiptDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Tbl_Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                onDelete: ReferentialAction.Cascade);
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
                name: "Description",
                table: "Tbl_ReceiptDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Tbl_ReceiptDetails");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Tbl_Cars");

            migrationBuilder.RenameColumn(
                name: "ReceiptDetailID",
                table: "Tbl_ReceiptDetails",
                newName: "ReceiptsDetailsID");

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
                principalColumn: "CarID");
        }
    }
}
