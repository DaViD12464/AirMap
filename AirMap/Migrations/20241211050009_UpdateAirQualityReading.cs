using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirMap.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAirQualityReading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AveragePM1",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePM10",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePM25",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AirQualityReadings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HCHO",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Humidity",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IJP",
                table: "AirQualityReadings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IJPDescription",
                table: "AirQualityReadings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IJPString",
                table: "AirQualityReadings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "AirQualityReadings",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AveragePM1",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "AveragePM10",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "AveragePM25",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "HCHO",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "IJP",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "IJPDescription",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "IJPString",
                table: "AirQualityReadings");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "AirQualityReadings");
        }
    }
}
