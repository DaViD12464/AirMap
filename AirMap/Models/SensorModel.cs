using AirMap.DTOs;

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

        public long? Id { get; set; }
        public int SamplingRate { get; set; }

        public Location? Location { get; set; }
        public List<SensorDataValues>? SensorDataValues { get; set; }

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
                //TODO: Finish the DTO SensorModel for sensor_community, apply changes to existing "location" variables
                //Location.id = (int)dto.location?.id,
                //Location.latitude = (decimal)dto.location?.latitude,
                //Location.longitude = (decimal)dto.location?.longitude,
                //Location.altitude = (decimal)dto.location?.altitude,


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
    }
    public class Location
    {
        public long? Id { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Altitude { get; set; }
        public string? Country { get; set; }
        public bool? Indoor { get; set; }
        public bool? ExactLocation { get; set; }
    }
}
