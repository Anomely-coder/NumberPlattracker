using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddEditPlate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditedBy",
                table: "PlateAllocations");

            migrationBuilder.DropColumn(
                name: "EditedOn",
                table: "PlateAllocations");

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                table: "NumberPlates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedOn",
                table: "NumberPlates",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditedBy",
                table: "NumberPlates");

            migrationBuilder.DropColumn(
                name: "EditedOn",
                table: "NumberPlates");

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                table: "PlateAllocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedOn",
                table: "PlateAllocations",
                type: "datetime2",
                nullable: true);
        }
    }
}
