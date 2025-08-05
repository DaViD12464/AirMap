using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AirMap.DTOs
{ /// <summary>
  /// Data Transfer Object (DTO) representing raw sensor data received from the Sensor_Community API.
  /// All properties are in default formats provided on API and may require parsing or validation.
  /// </summary>
    public class SensorCommunityDto
    {
        /// <summary>
        /// Identifier of the sensor device.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Sampling_rate of the sensor data.
        /// </summary>
        public string? Sampling_Rate { get; set; }
        /// <summary>
        /// Timestamp of the data, parsed from the epoch time.
        /// </summary>
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// Relation to Location table
        /// </summary>
        public Location? Location { get; set; }
        /// <summary>
        /// Relation to sensor table
        /// </summary>
        public Sensor? Sensor { get; set; }
        /// <summary>
        /// List of sensor data values.
        /// </summary>
        public List<SensorDataValues>? Sensordatavalues { get; set; }
    }

    /// <summary>
    /// Represents a location where the sensor is installed. Contains geographical information.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Identifier of the location.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Latitude of the sensor's location.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude of the sensor's location.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Altitude of the sensor's location.
        /// </summary>
        public double? Altitude { get; set; }
        /// <summary>
        /// Country of the sensor's location.
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// Exact_location status of the sensor's location. Indicates if the location is exact or approximate.
        /// </summary>
        public int? Exact_Location { get; set; } // currently comes as 1 or 0 in Int - preferably Boolean value
        /// <summary>
        /// Indoor status of the sensor's location. Indicates if the sensor is located indoors or outdoors.
        /// </summary>
        public int? Indoor { get; set; } // currently comes as 1 or 0 in Int - preferably Boolean value
    }

    /// <summary>
    /// Represents a sensor device. Contains information about the sensor type and its unique identifier.
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Identifier of the sensor.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Identifies the pin user by the sensor.
        /// </summary>
        public string? Pin { get; set; }
        /// <summary>
        /// Declaration of the sensor type.
        /// </summary>
        public SensorType? Sensor_Type { get; set; }
    }

    ///<summary>
    /// Represents the type of sensor. Contains information about the sensor's manufacturer and name.
    /// </summary>
    public class SensorType
    {
        /// <summary>
        /// Identifier of the sensor type.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Name of the sensor type. (String declaration of sensor type)
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Name/Brand of sensors manufacturer.
        /// </summary>
        public string? Manufacturer { get; set; }
    }

    /// <summary>
    /// Represents a single data value from the sensor. Contains the value and its type.
    /// </summary>
    public class SensorDataValues
    {
        /// <summary>
        /// Identifier of the exact sensor data value.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Value read by sensor.
        /// </summary>
        public string? Value { get; set; }
        /// <summary>
        /// Type of the value read by sensor.
        /// </summary>
        public required string Value_Type { get; set; }
    }
}
