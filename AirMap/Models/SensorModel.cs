using AirMap.DTOs;
using Newtonsoft.Json.Linq;

namespace AirMap.Models
{
    /// <summary>
    /// Represents an internal model of sensor data with strongly typed properties.
    /// This model is derived from LookO2Dto and is used throughout the application logic.
    /// </summary>
    public class SensorModel
    {

        /// <summary>
        /// Identifier of the sensor device.
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// PM1 (particulate matter higher than 1µm) concentration.
        /// </summary>
        public float? PM1 { get; set; }

        /// <summary>
        /// PM2.5 (particulate matter higher than 2.5µm) concentration.
        /// </summary>
        public float? PM25 { get; set; }

        /// <summary>
        /// PM10 (particulate matter higher than 10µm) concentration.
        /// </summary>
        public float? PM10 { get; set; }

        /// <summary>
        /// Timestamp of the data, parsed from the epoch time.
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Latitude of the sensor's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude of the sensor's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Integer air quality index.
        /// </summary>
        public int? IJP { get; set; }

        /// <summary>
        /// Short description of air quality in English.
        /// </summary>
        public string? IJPStringEN { get; set; }

        /// <summary>
        /// Short description of air quality (localized).
        /// </summary>
        public string? IJPString { get; set; }

        /// <summary>
        /// Detailed air quality description (localized).
        /// </summary>
        public string? IJPDescription { get; set; }

        /// <summary>
        /// Detailed air quality description in English.
        /// </summary>
        public string? IJPDescriptionEN { get; set; }

        /// <summary>
        /// Color code representing the air quality index.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Ambient temperature in Celsius.
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Relative humidity in percent.
        /// </summary>
        public float? Humidity { get; set; }

        /// <summary>
        /// Averaged PM1 value.
        /// </summary>
        public float? AveragePM1 { get; set; }

        /// <summary>
        /// Averaged PM2.5 value.
        /// </summary>
        public float? AveragePM25 { get; set; }

        /// <summary>
        /// Averaged PM10 value.
        /// </summary>
        public float? AveragePM10 { get; set; }

        /// <summary>
        /// Optional custom name for the sensor.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Indicates if the sensor is installed indoors.
        /// </summary>
        public bool? Indoor { get; set; }

        /// <summary>
        /// Previous IJP value.
        /// </summary>
        public int? PreviousIJP { get; set; }

        /// <summary>
        /// Formaldehyde (HCHO) concentration.
        /// </summary>
        public float? HCHO { get; set; }

        /// <summary>
        /// Averaged formaldehyde (HCHO) concentration.
        /// </summary>
        public float? AverageHCHO { get; set; }

        /// <summary>
        /// Human-readable location name for the sensor.
        /// </summary>
        public string? LocationName { get; set; }

        /// <summary>
        /// Converts a LookO2Dto instance into a strongly typed SensorModel.
        /// </summary>
        /// <param name="dto">DTO object with raw string data.</param>
        /// <returns>A parsed SensorModel instance.</returns>
        public static SensorModel FromDto(LookO2Dto dto)
        {
            return new SensorModel
            {
                Device = dto.Device,
                PM1 = ParseFloat(dto.PM1),
                PM25 = ParseFloat(dto.PM25),
                PM10 = ParseFloat(dto.PM10),
                Timestamp = ParseEpoch(dto.Epoch),
                Latitude = ParseDouble(dto.Latitude),
                Longitude = ParseDouble(dto.Longitude),
                IJP = ParseInt(dto.IJP),
                IJPStringEN = dto.IJPStringEN,
                IJPString = dto.IJPString,
                IJPDescription = dto.IJPDescription,
                IJPDescriptionEN = dto.IJPDescriptionEN,
                Color = dto.Color,
                Temperature = ParseFloat(dto.Temperature),
                Humidity = ParseFloat(dto.Humidity),
                AveragePM1 = ParseFloat(dto.AveragePM1),
                AveragePM25 = ParseFloat(dto.AveragePM25),
                AveragePM10 = ParseFloat(dto.AveragePM10),
                Name = dto.Name,
                Indoor = ParseBool(dto.Indoor),
                PreviousIJP = ParseInt(dto.PreviousIJP),
                HCHO = ParseFloat(dto.HCHO),
                AverageHCHO = ParseFloat(dto.AverageHCHO),
                LocationName = dto.LocationName
            };
        }
        /// <summary>
        /// Identity number of the sensor.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// SamplingRate of the sensor.
        /// </summary>
        public int SamplingRate { get; set; }

        /// <summary>
        /// Location table reference, used for location data.
        /// </summary>
        public Location? Location { get; set; }
        /// <summary>
        /// Sensor table reference, used for sensor data.
        /// </summary>
        public Sensor? Sensor { get; set; }
        /// <summary>
        /// List of SensorDataValues, used for sensor data values.
        /// </summary>
        public List<SensorDataValues>? SensorDataValues { get; set; }

        /// <summary>
        /// Converts a SensorCommunityDto instance into a strongly typed SensorModel.
        /// </summary>
        /// <param name="dto">DTO object with data types fetched from source.</param>
        /// <returns>A parsed SensorModel instance.</returns>
        public static SensorModel FromDto(SensorCommunityDto dto)
        {
            return new SensorModel
            {
                Id = dto.id,
                SamplingRate = ParseInt(dto.sampling_rate) ?? 0,
                Timestamp = dto.timestamp,
                Location = new Location
                {
                    Id = dto.location!.id,
                    Latitude = dto.location!.latitude,
                    Longitude = dto.location!.longitude,
                    Altitude = dto.location!.altitude,
                    Country = dto.location!.country,
                    ExactLocation = ParseBool(dto.location.ExactLocation.ToString()),
                    Indoor = ParseBool(dto.location.Indoor.ToString()),
                },
                Sensor = new Sensor
                {
                    Id = dto.sensor!.id,
                    Pin = ParseInt(dto.sensor.pin),
                    SensorType = new SensorType()
                    {
                        Id = dto.sensor.sensor_type!.id,
                        Name = dto.sensor.sensor_type.name,
                        Manufacturer = dto.sensor.sensor_type.manufacturer
                    }
                },
                SensorDataValues = ParseSensorDataValuesList(dto.sensordatavalues!)
               

            };
        }

        /// <summary>
        /// Parses a string into a nullable float.
        /// </summary>
        private static float? ParseFloat(string? value) =>
            float.TryParse(value, out var result) ? result : null;

        /// <summary>
        /// Parses a string into a nullable int.
        /// </summary>
        private static int? ParseInt(string? value) =>
            int.TryParse(value, out var result) ? result : null;

        /// <summary>
        /// Parses a string into a nullable double.
        /// </summary>
        private static double? ParseDouble(string? value) =>
            double.TryParse(value, out var result) ? result : null;

        /// <summary>
        /// Parses a string ("1" or "0") into a nullable boolean.
        /// </summary>
        private static bool? ParseBool(string? value) =>
            value == "1" ? true : value == "0" ? false : null;


        /// <summary>
        /// Parses a string representing a Unix epoch timestamp into a DateTime.
        /// </summary>
        private static DateTime? ParseEpoch(string? epoch)
        {
            return long.TryParse(epoch, out var seconds)
                ? DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime
                : null;
        }

        /// <summary>
        /// Parses a list of SensorDataValues from the DTO into a strongly typed list.
        /// Changes Value from string to double type.
        /// </summary>
        /// <param name="sdv">List from DTO source.</param>
        /// <returns>Returns list of parsed items to use preferred value-types.</returns>
        private static List<AirMap.Models.SensorDataValues> ParseSensorDataValuesList(List<AirMap.DTOs.SensorDataValues> sdv)
        {
            var result = new List<SensorDataValues>();
            foreach (var item in sdv)
            {
                result.Add(new SensorDataValues
                {
                    Id = item.id,
                    Value = ParseDouble(item.value?.ToString()),
                    ValueType = item.value_type
                });
            }
            return result;
        }
    }

    /// <summary>
    /// Instance of Location table values with preferred value-types.
    /// </summary>
    public class Location
    {   /// <summary>
        /// Identity number of the location.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// Latitude of the sensor's location.
        /// </summary>
        public double? Latitude { get; set; }
        /// <summary>
        /// Longitude of the sensor's location.
        /// </summary>
        public double? Longitude { get; set; }
        /// <summary>
        /// Altitude of the sensor's location.
        /// </summary>
        public double? Altitude { get; set; }
        /// <summary>
        /// Country of the sensor's location.
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// Indicates if the sensor is installed indoors.
        /// </summary>
        public bool? Indoor { get; set; }
        /// <summary>
        /// Exact_location status of the sensor's location. Indicates if the location is exact or approximate.
        /// </summary>
        public bool? ExactLocation { get; set; }
    }
    /// <summary>
    /// Instance of Sensor class values with preferred value-types.
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Identity number of the sensor.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// Pin used by the sensor.
        /// </summary>
        public int? Pin { get; set; }
        /// <summary>
        /// SensorType table reference, used for sensor type data.
        /// </summary>
        public SensorType? SensorType { get; set; }
    }
    /// <summary>
    /// Instance of Location class values with preferred value-types.
    /// </summary>
    public class SensorType
    {   /// <summary>
        /// Identity number of the sensor type.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// Name of the sensor.
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Manufacturer of the sensor.
        /// </summary>
        public string? Manufacturer { get; set; }
    }
    /// <summary>
    /// Instance of SensorDataValues class values with preferred value-types - used for Lists of SensorDataValues.
    /// </summary>
    public class SensorDataValues
    {   /// <summary>
        /// Identity number of the sensor data value.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// Value of the sensor data, parsed from string to double.
        /// </summary>
        public double? Value { get; set; }
        /// <summary>
        /// Type of the sensor data value, presented in string.
        /// </summary>
        public string? ValueType { get; set; }
    }
}
