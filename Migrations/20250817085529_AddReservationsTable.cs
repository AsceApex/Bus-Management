using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    PassengerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PassengerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ScheduleId",
                table: "Reservations",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Buses_BusId",
                table: "Schedules",
                column: "BusId",
                principalTable: "Buses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Drivers_DriverId",
                table: "Schedules",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Routes_BusRouteId",
                table: "Schedules",
                column: "BusRouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.DropTable(
                name: "Reservations");

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
    }
}
