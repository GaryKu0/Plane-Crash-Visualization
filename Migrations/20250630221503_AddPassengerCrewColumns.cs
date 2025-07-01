using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plane_Crash_Visualization.Migrations
{
    /// <inheritdoc />
    public partial class AddPassengerCrewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AboardCrew",
                table: "Crashes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AboardPassengers",
                table: "Crashes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FatalitiesCrew",
                table: "Crashes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FatalitiesPassengers",
                table: "Crashes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboardCrew",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "AboardPassengers",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "FatalitiesCrew",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "FatalitiesPassengers",
                table: "Crashes");
        }
    }
}
