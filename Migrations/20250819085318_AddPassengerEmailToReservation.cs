using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPassengerEmailToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PassengerEmail",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerEmail",
                table: "Reservations");
        }
    }
}
