using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class FirstCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_Cars",
                columns: table => new
                {
                    CarID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberPlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Cars", x => x.CarID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Services",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    ServiceStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Services", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CNIC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarID = table.Column<int>(type: "int", nullable: false),
                    CustomerStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Customers", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK_Tbl_Customers_Tbl_Cars_CarID",
                        column: x => x.CarID,
                        principalTable: "Tbl_Cars",
                        principalColumn: "CarID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Receipts",
                columns: table => new
                {
                    ReceiptID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    CarID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Receipts", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_Tbl_Receipts_Tbl_Cars_CarID",
                        column: x => x.CarID,
                        principalTable: "Tbl_Cars",
                        principalColumn: "CarID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tbl_Receipts_Tbl_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Tbl_Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_TransferCars",
                columns: table => new
                {
                    TransferID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FromCustomerID = table.Column<int>(type: "int", nullable: false),
                    ToCustomerID = table.Column<int>(type: "int", nullable: false),
                    CarID = table.Column<int>(type: "int", nullable: false),
                    CarsCarID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_TransferCars", x => x.TransferID);
                    table.ForeignKey(
                        name: "FK_Tbl_TransferCars_Tbl_Cars_CarID",
                        column: x => x.CarID,
                        principalTable: "Tbl_Cars",
                        principalColumn: "CarID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tbl_TransferCars_Tbl_Cars_CarsCarID",
                        column: x => x.CarsCarID,
                        principalTable: "Tbl_Cars",
                        principalColumn: "CarID");
                    table.ForeignKey(
                        name: "FK_Tbl_TransferCars_Tbl_Customers_FromCustomerID",
                        column: x => x.FromCustomerID,
                        principalTable: "Tbl_Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tbl_TransferCars_Tbl_Customers_ToCustomerID",
                        column: x => x.ToCustomerID,
                        principalTable: "Tbl_Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_ReceiptDetails",
                columns: table => new
                {
                    ReceiptsDetailsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptID = table.Column<int>(type: "int", nullable: false),
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_ReceiptDetails", x => x.ReceiptsDetailsID);
                    table.ForeignKey(
                        name: "FK_Tbl_ReceiptDetails_Tbl_Receipts_ReceiptID",
                        column: x => x.ReceiptID,
                        principalTable: "Tbl_Receipts",
                        principalColumn: "ReceiptID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tbl_ReceiptDetails_Tbl_Services_ServiceID",
                        column: x => x.ServiceID,
                        principalTable: "Tbl_Services",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Customers_CarID",
                table: "Tbl_Customers",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ReceiptDetails_ReceiptID",
                table: "Tbl_ReceiptDetails",
                column: "ReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_ReceiptDetails_ServiceID",
                table: "Tbl_ReceiptDetails",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Receipts_CarID",
                table: "Tbl_Receipts",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Receipts_CustomerID",
                table: "Tbl_Receipts",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_TransferCars_CarID",
                table: "Tbl_TransferCars",
                column: "CarID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_TransferCars_CarsCarID",
                table: "Tbl_TransferCars",
                column: "CarsCarID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_TransferCars_FromCustomerID",
                table: "Tbl_TransferCars",
                column: "FromCustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_TransferCars_ToCustomerID",
                table: "Tbl_TransferCars",
                column: "ToCustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_ReceiptDetails");

            migrationBuilder.DropTable(
                name: "Tbl_TransferCars");

            migrationBuilder.DropTable(
                name: "Tbl_Users");

            migrationBuilder.DropTable(
                name: "Tbl_Receipts");

            migrationBuilder.DropTable(
                name: "Tbl_Services");

            migrationBuilder.DropTable(
                name: "Tbl_Customers");

            migrationBuilder.DropTable(
                name: "Tbl_Cars");
        }
    }
}
