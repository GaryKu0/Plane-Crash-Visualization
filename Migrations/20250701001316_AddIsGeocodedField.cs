using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plane_Crash_Visualization.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGeocodedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGeocoded",
                table: "Crashes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Crashes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Crashes",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeocoded",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Crashes");
        }
    }
}
