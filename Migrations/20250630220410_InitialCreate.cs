using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plane_Crash_Visualization.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crashes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Time = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AC_Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Registration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cn_ln = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aboard = table.Column<int>(type: "int", nullable: true),
                    Fatalities = table.Column<int>(type: "int", nullable: true),
                    Ground = table.Column<int>(type: "int", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crashes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Crashes");
        }
    }
}
