using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class nullable_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_SensorType_SensorTypeId",
                table: "Sensor");

            migrationBuilder.AlterColumn<decimal>(
                name: "SamplingRate",
                table: "SensorModel",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<long>(
                name: "SensorTypeId",
                table: "Sensor",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Sensor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_SensorType_SensorTypeId",
                table: "Sensor",
                column: "SensorTypeId",
                principalTable: "SensorType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_SensorType_SensorTypeId",
                table: "Sensor");

            migrationBuilder.AlterColumn<decimal>(
                name: "SamplingRate",
                table: "SensorModel",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SensorTypeId",
                table: "Sensor",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Sensor",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_SensorType_SensorTypeId",
                table: "Sensor",
                column: "SensorTypeId",
                principalTable: "SensorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
