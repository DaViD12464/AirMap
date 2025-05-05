namespace AirMap.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) representing raw sensor data received from the LookO2 API.
    /// All properties are in string format and may require parsing or validation.
    /// </summary>
    public class LookO2Dto
    {

        /// <summary>
        /// Identifier of the sensor device.
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// PM1 (particulate matter higher than 1um) concentration as a string.
        /// </summary>
        public string? PM1 { get; set; }

        /// <summary>
        /// PM2.5 (particulate matter higher than 2.5µm) concentration as a string.
        /// </summary>
        public string? PM25 { get; set; }

        /// <summary>
        /// PM10 (particulate matter higher than 10µm) concentration as a string.
        /// </summary>
        public string? PM10 { get; set; }

        /// <summary>
        /// Unix epoch timestamp (in seconds) when the data was recorded.
        /// </summary>
        public string? Epoch { get; set; }

        /// <summary>
        /// Latitude of the sensor location as a string.
        /// </summary>
        public string? Latitude { get; set; }

        /// <summary>
        /// Longitude of the sensor location as a string.
        /// </summary>
        public string? Longitude { get; set; }

        /// <summary>
        /// Index of air quality pollution (IJP) as a stringified number.
        /// </summary>
        public string? IJP { get; set; }

        /// <summary>
        /// Air quality level description in English (short format).
        /// </summary>
        public string? IJPStringEN { get; set; }

        /// <summary>
        /// Air quality level description (short format, localized).
        /// </summary>
        public string? IJPString { get; set; }

        /// <summary>
        /// Air quality description in full detail (localized).
        /// </summary>
        public string? IJPDescription { get; set; }

        /// <summary>
        /// Air quality description in full detail (English).
        /// </summary>
        public string? IJPDescriptionEN { get; set; }

        /// <summary>
        /// Color code associated with the current IJP value.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Ambient temperature in degrees Celsius as a string.
        /// </summary>
        public string? Temperature { get; set; }

        /// <summary>
        /// Relative humidity percentage as a string.
        /// </summary>
        public string? Humidity { get; set; }

        /// <summary>
        /// Averaged PM1 value over a period of time as a string.
        /// </summary>
        public string? AveragePM1 { get; set; }

        /// <summary>
        /// Averaged PM2.5 value over a period of time as a string.
        /// </summary>
        public string? AveragePM25 { get; set; }

        /// <summary>
        /// Averaged PM10 value over a period of time as a string.
        /// </summary>
        public string? AveragePM10 { get; set; }

        /// <summary>
        /// Custom or user-defined name for the sensor.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Indicates whether the sensor is used indoors (usually "1" or "0").
        /// </summary>
        public string? Indoor { get; set; }

        /// <summary>
        /// Previously recorded IJP value.
        /// </summary>
        public string? PreviousIJP { get; set; }

        /// <summary>
        /// Formaldehyde (HCHO) concentration as a string.
        /// </summary>
        public string? HCHO { get; set; }

        /// <summary>
        /// Averaged formaldehyde (HCHO) concentration as a string.
        /// </summary>
        public string? AverageHCHO { get; set; }

        /// <summary>
        /// Human-readable location name assigned to the sensor.
        /// </summary>
        public string? LocationName { get; set; }
    }
}
