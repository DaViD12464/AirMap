using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirQualityReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    PM1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PM25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PM10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indoor = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HCHO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IJP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IJPString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IJPDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirQualityReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Altitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indoor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source1Models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Device = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PM1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PM25 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PM10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Epoch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Lon = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indoor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Humidity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HCHO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AveragePM1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AveragePM25 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AveragePM10 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IJPString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IJPDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source1Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source2Models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source2Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Source2Models_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SensorDataValue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source2ModelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorDataValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorDataValue_Source2Models_Source2ModelId",
                        column: x => x.Source2ModelId,
                        principalTable: "Source2Models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataValue_Source2ModelId",
                table: "SensorDataValue",
                column: "Source2ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Source1Models_Device",
                table: "Source1Models",
                column: "Device",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Source2Models_LocationId",
                table: "Source2Models",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirQualityReadings");

            migrationBuilder.DropTable(
                name: "SensorDataValue");

            migrationBuilder.DropTable(
                name: "Source1Models");

            migrationBuilder.DropTable(
                name: "Source2Models");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
