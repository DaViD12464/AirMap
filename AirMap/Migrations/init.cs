using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    SourceApiId = table.Column<long>(type: "bigint", nullable: true),
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
                    SourceApiId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sensor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceApiId = table.Column<long>(type: "bigint", nullable: true),
                    Pin = table.Column<int>(type: "int", nullable: true),
                    SensorTypeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sensor_SensorType_SensorTypeId",
                        column: x => x.SensorTypeId,
                        principalTable: "SensorType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SensorModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceApiId = table.Column<long>(type: "bigint", nullable: true),
                    Device = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Pm1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pm25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pm10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Ijp = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    IjpStringEn = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    IjpString = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    IjpDescription = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    IjpDescriptionEn = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePm1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePm25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AveragePm10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Indoor = table.Column<bool>(type: "bit", nullable: true),
                    PreviousIjp = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Hcho = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AverageHcho = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocationName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    SamplingRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LocationId = table.Column<long>(type: "bigint", nullable: true),
                    SensorId = table.Column<long>(type: "bigint", nullable: true),
                    SensorDataValuesIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_SensorModel_Sensor_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SensorDataValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceApiId = table.Column<long>(type: "bigint", nullable: true),
                    Value = table.Column<double>(type: "float", nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SensorModelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorDataValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorDataValues_SensorModel_SensorModelId",
                        column: x => x.SensorModelId,
                        principalTable: "SensorModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_SensorTypeId",
                table: "Sensor",
                column: "SensorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorDataValues_SensorModelId",
                table: "SensorDataValues",
                column: "SensorModelId");

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
                name: "IX_SensorModel_SensorId",
                table: "SensorModel",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorModel_SourceApiId",
                table: "SensorModel",
                column: "SourceApiId",
                unique: true,
                filter: "[SourceApiId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorDataValues");

            migrationBuilder.DropTable(
                name: "SensorModel");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Sensor");

            migrationBuilder.DropTable(
                name: "SensorType");
        }
    }
}
