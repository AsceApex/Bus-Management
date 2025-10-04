using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDriversTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Buses",
                newName: "PlateNumber");

            migrationBuilder.AddColumn<double>(
                name: "DistanceInKm",
                table: "Routes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedTime",
                table: "Routes",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "BusNumber",
                table: "Buses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusType",
                table: "Buses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropColumn(
                name: "DistanceInKm",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "EstimatedTime",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "BusNumber",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "BusType",
                table: "Buses");

            migrationBuilder.RenameColumn(
                name: "PlateNumber",
                table: "Buses",
                newName: "Number");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Routes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
