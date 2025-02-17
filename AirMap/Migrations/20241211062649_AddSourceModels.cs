using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Altitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Indoor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source1Models",
                columns: table => new
                {
                    Device = table.Column<string>(type: "text", nullable: false),
                    PM1 = table.Column<string>(type: "text", nullable: true),
                    PM25 = table.Column<string>(type: "text", nullable: true),
                    PM10 = table.Column<string>(type: "text", nullable: true),
                    Epoch = table.Column<string>(type: "text", nullable: true),
                    Lat = table.Column<decimal>(type: "numeric", nullable: false),
                    Lon = table.Column<decimal>(type: "numeric", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Indoor = table.Column<string>(type: "text", nullable: true),
                    Temperature = table.Column<string>(type: "text", nullable: true),
                    Humidity = table.Column<string>(type: "text", nullable: true),
                    HCHO = table.Column<string>(type: "text", nullable: true),
                    AveragePM1 = table.Column<string>(type: "text", nullable: true),
                    AveragePM25 = table.Column<string>(type: "text", nullable: true),
                    AveragePM10 = table.Column<string>(type: "text", nullable: true),
                    IJPString = table.Column<string>(type: "text", nullable: true),
                    IJPDescription = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source1Models", x => x.Device);
                });

            migrationBuilder.CreateTable(
                name: "Source2Models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true)
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
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "text", nullable: true),
                    ValueType = table.Column<string>(type: "text", nullable: true),
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
                name: "IX_Source2Models_LocationId",
                table: "Source2Models",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
