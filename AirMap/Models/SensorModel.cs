using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using AirMap.DTOs;

namespace AirMap.Models;

/// <summary>
///     Represents an internal model of sensor data with strongly typed properties.
///     This model is derived from LookO2Dto and is used throughout the application logic.
/// </summary>
public class SensorModel
{
    /// <summary>
    ///     Identity number of the sensor for auto ID within DB.
    /// </summary>
    [Key]
    public long Id { get; set; }

    /// <summary>
    ///     Identity number of the sensor from source API.
    /// </summary>
    public long? SourceApiId { get; set; }
    /// <summary>
    ///     Identifier of the sensor device.
    /// </summary>
    public string? Device { get; set; }

    /// <summary>
    ///     PM1 (particulate matter higher than 1µm) concentration.
    /// </summary>
    public float? Pm1 { get; set; }

    /// <summary>
    ///     PM2.5 (particulate matter higher than 2.5µm) concentration.
    /// </summary>
    public float? Pm25 { get; set; }

    /// <summary>
    ///     PM10 (particulate matter higher than 10µm) concentration.
    /// </summary>
    public float? Pm10 { get; set; }

    /// <summary>
    ///     Timestamp of the data, parsed from the epoch time.
    /// </summary>
    public DateTime? Timestamp { get; set; }

    /// <summary>
    ///     Latitude of the sensor's location.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    ///     Longitude of the sensor's location.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    ///     Integer air quality index.
    /// </summary>
    public int? Ijp { get; set; }

    /// <summary>
    ///     Short description of air quality in English.
    /// </summary>
    public string? IjpStringEn { get; set; }

    /// <summary>
    ///     Short description of air quality (localized).
    /// </summary>
    public string? IjpString { get; set; }

    /// <summary>
    ///     Detailed air quality description (localized).
    /// </summary>
    public string? IjpDescription { get; set; }

    /// <summary>
    ///     Detailed air quality description in English.
    /// </summary>
    public string? IjpDescriptionEn { get; set; }

    /// <summary>
    ///     Color code representing the air quality index.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    ///     Ambient temperature in Celsius.
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    ///     Relative humidity in percent.
    /// </summary>
    public float? Humidity { get; set; }

    /// <summary>
    ///     Averaged PM1 value.
    /// </summary>
    public float? AveragePm1 { get; set; }

    /// <summary>
    ///     Averaged PM2.5 value.
    /// </summary>
    public float? AveragePm25 { get; set; }

    /// <summary>
    ///     Averaged PM10 value.
    /// </summary>
    public float? AveragePm10 { get; set; }

    /// <summary>
    ///     Optional custom name for the sensor.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Indicates if the sensor is installed indoors.
    /// </summary>
    public bool? Indoor { get; set; }

    /// <summary>
    ///     Previous IJP value.
    /// </summary>
    public int? PreviousIjp { get; set; }

    /// <summary>
    ///     Formaldehyde (HCHO) concentration.
    /// </summary>
    public float? Hcho { get; set; }

    /// <summary>
    ///     Averaged formaldehyde (HCHO) concentration.
    /// </summary>
    public float? AverageHcho { get; set; }

    /// <summary>
    ///     Human-readable location name for the sensor.
    /// </summary>
    public string? LocationName { get; set; }

    /// <summary>
    ///     SamplingRate of the sensor.
    /// </summary>
    public int? SamplingRate { get; set; }

    /// <summary>
    ///     Location table reference, used for location data.
    /// </summary>
    public long? LocationId { get; set; }

    [ForeignKey("LocationId")] 
    public Location? Location { get; set; }

    /// <summary>
    ///     Sensor table reference, used for sensor data.
    /// </summary>
    public long? SensorId { get; set; }
    [ForeignKey("SensorId")]
    public Sensor? Sensor { get; set; }

    /// <summary>
    ///     List of SensorDataValuesIds, used for finding correct data.
    /// </summary>
    public List<long?> SensorDataValuesIds { get; set; } = [];

    /// <summary>
    ///     List of SensorDataValues, used for sensor data values.
    /// </summary>
    public List<SensorDataValues>? SensorDataValues { get; set; }

    /// <summary>
    ///     Converts a LookO2Dto instance into a strongly typed SensorModel.
    /// </summary>
    /// <param name="dto">DTO object with raw string data.</param>
    /// <returns>A parsed SensorModel instance.</returns>
    public static SensorModel FromDto(LookO2Dto dto)
    {
        return new SensorModel
        {
            Device = dto.Device,
            Pm1 = ParseFloat(dto.Pm1),
            Pm25 = ParseFloat(dto.Pm25),
            Pm10 = ParseFloat(dto.Pm10),
            Timestamp = ParseEpoch(dto.Epoch),
            Latitude = ParseDouble(dto.Lat),
            Longitude = ParseDouble(dto.Lon),
            Ijp = ParseInt(dto.Ijp),
            IjpStringEn = dto.IjpStringEn,
            IjpString = dto.IjpString,
            IjpDescription = dto.IjpDescription,
            IjpDescriptionEn = dto.IjpDescriptionEn,
            Color = dto.Color,
            Temperature = ParseFloat(dto.Temperature),
            Humidity = ParseFloat(dto.Humidity),
            AveragePm1 = ParseFloat(dto.AveragePm1),
            AveragePm25 = ParseFloat(dto.AveragePm25),
            AveragePm10 = ParseFloat(dto.AveragePm10),
            Name = dto.Name,
            Indoor = ParseBool(dto.Indoor),
            PreviousIjp = ParseInt(dto.PreviousIjp),
            Hcho = ParseFloat(dto.Hcho),
            AverageHcho = ParseFloat(dto.AverageHcho),
            LocationName = dto.LocationName
        };
    }

    /// <summary>
    ///     Converts a SensorCommunityDto instance into a strongly typed SensorModel.
    /// </summary>
    /// <param name="dto">DTO object with data types fetched from source.</param>
    /// <returns>A parsed SensorModel instance.</returns>
    public static SensorModel FromDto(SensorCommunityDto dto)
    {
        return new SensorModel
        {
            SourceApiId = dto.Id,
            SamplingRate = ParseInt(dto.Sampling_Rate) ?? 0,
            Timestamp = dto.Timestamp,
            Location = new Location
            {
                SourceApiId = dto.Location!.Id,
                Latitude = dto.Location!.Latitude,
                Longitude = dto.Location!.Longitude,
                Altitude = dto.Location!.Altitude,
                Country = dto.Location!.Country,
                ExactLocation = ParseBool(dto.Location.Exact_Location.ToString()),
                Indoor = ParseBool(dto.Location.Indoor.ToString())
            },
            Sensor = new Sensor
            {
                SourceApiId = dto.Sensor!.Id,
                Pin = ParseInt(dto.Sensor.Pin),
                SensorType = dto.Sensor.Sensor_Type == null
                    ? new SensorType { Name = "n/a", Manufacturer = "n/a" }
                    : new SensorType //allow null SensorType value as it is not required
                    {
                        SourceApiId = dto.Sensor.Sensor_Type!.Id,
                        Name = dto.Sensor.Sensor_Type.Name,
                        Manufacturer = dto.Sensor.Sensor_Type.Manufacturer
                    }
            },
            SensorDataValues = ParseSensorDataValuesList(dto.Sensordatavalues!),
            SensorDataValuesIds = GetSensorDataValuesIds(dto.Sensordatavalues!),
        };
    }

    /// <summary>
    ///     Parses a string into a nullable float.
    /// </summary>
    private static float? ParseFloat(string? value)
    {
        return float.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : null;
    }

    /// <summary>
    ///     Parses a string into a nullable int.
    /// </summary>
    private static int? ParseInt(string? value)
    {
        return int.TryParse(value, out var result) ? result : null;
    }

    /// <summary>
    ///     Parses a string into a nullable double.
    /// </summary>
    private static double? ParseDouble(string? value)
    {
        return double.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : null;
    }

    /// <summary>
    ///     Parses a string ("1" or "0") into a nullable boolean.
    /// </summary>
    private static bool? ParseBool(string? value)
    {
        return value == "1" ? true : value == "0" ? false : null;
    }


    /// <summary>
    ///     Parses a string representing a Unix epoch timestamp into a DateTime.
    /// </summary>
    private static DateTime? ParseEpoch(string? epoch)
    {
        return long.TryParse(epoch, out var seconds)
            ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime
            : null;
    }

    /// <summary>
    ///     Parses a list of SensorDataValues from the DTO into a strongly typed list.
    ///     Changes Value from string to double type.
    /// </summary>
    /// <param name="sdv">List from DTO source.</param>
    /// <returns>Returns list of parsed items to use preferred value-types.</returns>
    private static List<SensorDataValues> ParseSensorDataValuesList(List<DTOs.SensorDataValues> sdv)
    {
        var result = new List<SensorDataValues>();
        foreach (var item in sdv)
        {
            var valid = double.TryParse(item.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue);

            // SQL Server doesn't support NaN or Infinity
            if (!valid || double.IsNaN(parsedValue) || double.IsInfinity(parsedValue)) parsedValue = 0.0;

            Random rand = new Random();  // add usage of System.Random to generate random numbers

            result.Add(new SensorDataValues
            {
                SourceApiId = item.Id == 0 ? rand.Next(1,Math.Abs(parsedValue.GetHashCode()+item.Value_Type!.GetHashCode())) : item.Id, // add id generating algorithm when item.id is 0
                Value = parsedValue,
                ValueType = item.Value_Type ?? "n/a"
            });
        }

        return result;
    }

    private static List<long?> GetSensorDataValuesIds(List<DTOs.SensorDataValues> sdv)
    {
        return sdv.Select(item => item.Id).Select(dummy => (long?)dummy).ToList();
    }
}







