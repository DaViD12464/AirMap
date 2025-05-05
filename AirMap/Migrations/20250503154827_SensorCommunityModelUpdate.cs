using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class SensorCommunityModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AirQualityReadings",
                table: "AirQualityReadings");

            migrationBuilder.RenameTable(
                name: "AirQualityReadings",
                newName: "AirQualityReading");

            migrationBuilder.AddColumn<int>(
                name: "SamplingRate",
                table: "Source2Models",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SensorId",
                table: "Source2Models",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Source1Models",
                type: "decimal(18,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Source1Models",
                type: "decimal(18,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Altitude",
                table: "Location",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "ExactLocation",
                table: "Location",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirQualityReading",
                table: "AirQualityReading",
                column: "Id");

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
                name: "Sensor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_Source2Models_SensorId",
                table: "Source2Models",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_SensorTypeId",
                table: "Sensor",
                column: "SensorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Source2Models_Sensor_SensorId",
                table: "Source2Models",
                column: "SensorId",
                principalTable: "Sensor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Source2Models_Sensor_SensorId",
                table: "Source2Models");

            migrationBuilder.DropTable(
                name: "Sensor");

            migrationBuilder.DropTable(
                name: "SensorType");

            migrationBuilder.DropIndex(
                name: "IX_Source2Models_SensorId",
                table: "Source2Models");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AirQualityReading",
                table: "AirQualityReading");

            migrationBuilder.DropColumn(
                name: "SamplingRate",
                table: "Source2Models");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Source2Models");

            migrationBuilder.DropColumn(
                name: "ExactLocation",
                table: "Location");

            migrationBuilder.RenameTable(
                name: "AirQualityReading",
                newName: "AirQualityReadings");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Source1Models",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Source1Models",
                type: "decimal(18,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Altitude",
                table: "Location",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirQualityReadings",
                table: "AirQualityReadings",
                column: "Id");
        }
    }
}
