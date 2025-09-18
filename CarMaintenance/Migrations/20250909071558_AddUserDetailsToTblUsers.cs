using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDetailsToTblUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Tbl_Users",
                newName: "Role");

            migrationBuilder.AlterColumn<bool>(
                name: "UserStatus",
                table: "Tbl_Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tbl_Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Tbl_Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Tbl_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Tbl_Users");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Tbl_Users");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Tbl_Users",
                newName: "UserName");

            migrationBuilder.AlterColumn<int>(
                name: "UserStatus",
                table: "Tbl_Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
