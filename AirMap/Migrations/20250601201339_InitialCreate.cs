using System;
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
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorModelId = table.Column<long>(type: "bigint", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Altitude = table.Column<double>(type: "float", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indoor = table.Column<bool>(type: "bit", nullable: true),
                    ExactLocation = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Device = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PM1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PM25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PM10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    IJP = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    IJPStringEN = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    IJPString = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    IJPDescription = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    IJPDescriptionEN = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePM10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Indoor = table.Column<bool>(type: "bit", nullable: true),
                    PreviousIJP = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    HCHO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageHCHO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocationName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SamplingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: true),
                    SensorTypeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorModel_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SensorModel_SensorType_SensorTypeId",
                        column: x => x.SensorTypeId,
                        principalTable: "SensorType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sensor",
                columns: table => new
                {
                    SensorModelId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: true),
                    Pin = table.Column<int>(type: "int", nullable: true),
                    SensorTypeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensor", x => x.SensorModelId);
                    table.ForeignKey(
                        name: "FK_Sensor_SensorModel_SensorModelId",
                        column: x => x.SensorModelId,
                        principalTable: "SensorModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sensor_SensorType_SensorTypeId",
                        column: x => x.SensorTypeId,
                        principalTable: "SensorType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorDataValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorDataValuesId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorDataValues", x => new { x.SensorDataValuesId, x.Id });
                    table.ForeignKey(
                        name: "FK_SensorDataValues_SensorModel_SensorDataValuesId",
                        column: x => x.SensorDataValuesId,
                        principalTable: "SensorModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_SensorTypeId",
                table: "Sensor",
                column: "SensorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorModel_Device",
                table: "SensorModel",
                column: "Device",
                unique: true,
                filter: "[Device] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SensorModel_Id",
                table: "SensorModel",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorModel_LocationId",
                table: "SensorModel",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorModel_SensorTypeId",
                table: "SensorModel",
                column: "SensorTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensor");

            migrationBuilder.DropTable(
                name: "SensorDataValues");

            migrationBuilder.DropTable(
                name: "SensorModel");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "SensorType");
        }
    }
}
