using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "RouteId",
                table: "Schedules",
                newName: "BusRouteId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalTime",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BusId",
                table: "Schedules",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_BusRouteId",
                table: "Schedules",
                column: "BusRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DriverId",
                table: "Schedules",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Buses_BusId",
                table: "Schedules",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Drivers_DriverId",
                table: "Schedules",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Routes_BusRouteId",
                table: "Schedules",
                column: "BusRouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Buses_BusId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Drivers_DriverId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Routes_BusRouteId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BusId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_BusRouteId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_DriverId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "BusRouteId",
                table: "Schedules",
                newName: "RouteId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArrivalTime",
                table: "Schedules",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
