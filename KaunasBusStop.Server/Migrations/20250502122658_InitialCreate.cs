using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KaunasBusStop.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Monday = table.Column<int>(type: "int", nullable: true),
                    Tuesday = table.Column<int>(type: "int", nullable: true),
                    Wednesday = table.Column<int>(type: "int", nullable: true),
                    Thursday = table.Column<int>(type: "int", nullable: true),
                    Friday = table.Column<int>(type: "int", nullable: true),
                    Saturday = table.Column<int>(type: "int", nullable: true),
                    Sunday = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RouteShorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteLongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteType = table.Column<int>(type: "int", nullable: true),
                    RouteURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteSortOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                });

            migrationBuilder.CreateTable(
                name: "RoutesWorkWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RouteShorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteLongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteType = table.Column<int>(type: "int", nullable: true),
                    RouteSortOrder = table.Column<int>(type: "int", nullable: true),
                    Monday = table.Column<int>(type: "int", nullable: true),
                    Tuesday = table.Column<int>(type: "int", nullable: true),
                    Wednesday = table.Column<int>(type: "int", nullable: true),
                    Thursday = table.Column<int>(type: "int", nullable: true),
                    Friday = table.Column<int>(type: "int", nullable: true),
                    Saturday = table.Column<int>(type: "int", nullable: true),
                    Sunday = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutesWorkWeeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    StopId = table.Column<int>(type: "int", nullable: false),
                    StopCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopLat = table.Column<float>(type: "real", nullable: true),
                    StopLon = table.Column<float>(type: "real", nullable: true),
                    StopURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationType = table.Column<int>(type: "int", nullable: true),
                    ParentStation = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.StopId);
                });

            migrationBuilder.CreateTable(
                name: "StopTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArrivalTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartureTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopId = table.Column<int>(type: "int", nullable: true),
                    StopSequence = table.Column<int>(type: "int", nullable: true),
                    PickUpType = table.Column<int>(type: "int", nullable: true),
                    DropOffType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    TripId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RouteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    TripHeadsign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectionId = table.Column<int>(type: "int", nullable: true),
                    BlockId = table.Column<int>(type: "int", nullable: true),
                    ShapeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WheelchairAccessible = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.TripId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calendars");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "RoutesWorkWeeks");

            migrationBuilder.DropTable(
                name: "Stops");

            migrationBuilder.DropTable(
                name: "StopTimes");

            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
