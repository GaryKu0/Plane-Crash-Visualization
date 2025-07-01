using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plane_Crash_Visualization.Migrations
{
    /// <inheritdoc />
    public partial class AddManufacturerAndModelColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AC_Model",
                table: "Crashes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Crashes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AC_Model",
                table: "Crashes");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Crashes");
        }
    }
}
