using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBusSeatsAndUniqueReservationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_ScheduleId",
                table: "Reservations");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusSeats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusSeats_Buses_BusId",
                        column: x => x.BusId,
                        principalTable: "Buses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ScheduleId_SeatNumber",
                table: "Reservations",
                columns: new[] { "ScheduleId", "SeatNumber" },
                unique: true,
                filter: "[SeatNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BusSeats_BusId_SeatNumber",
                table: "BusSeats",
                columns: new[] { "BusId", "SeatNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusSeats");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ScheduleId_SeatNumber",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ScheduleId",
                table: "Reservations",
                column: "ScheduleId");
        }
    }
}
