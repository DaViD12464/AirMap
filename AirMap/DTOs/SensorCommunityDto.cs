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
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        /// <summary>
        /// Sampling_rate of the sensor data.
        /// </summary>
        public string? sampling_rate { get; set; }
        /// <summary>
        /// Timestamp of the data, parsed from the epoch time.
        /// </summary>
        public DateTime? timestamp { get; set; }
        /// <summary>
        /// Relation to Location table
        /// </summary>
        public Location? location { get; set; }
        /// <summary>
        /// Relation to sensor table
        /// </summary>
        public Sensor? sensor { get; set; }
        /// <summary>
        /// List of sensor data values.
        /// </summary>
        public List<SensorDataValues>? sensordatavalues { get; set; }
    }

    /// <summary>
    /// Represents a location where the sensor is installed. Contains geographical information.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Identifier of the location.
        /// </summary>
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        /// <summary>
        /// Latitude of the sensor's location.
        /// </summary>
        public double latitude { get; set; }
        /// <summary>
        /// Longitude of the sensor's location.
        /// </summary>
        public double longitude { get; set; }
        /// <summary>
        /// Altitude of the sensor's location.
        /// </summary>
        public double altitude { get; set; }
        /// <summary>
        /// Country of the sensor's location.
        /// </summary>
        public string? country { get; set; }
        /// <summary>
        /// Exact_location status of the sensor's location. Indicates if the location is exact or approximate.
        /// </summary>
        public int? ExactLocation { get; set; } // currently comes as 1 or 0 in Int - preferably Boolean value
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
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        /// <summary>
        /// Identifies the pin user by the sensor.
        /// </summary>
        public string? pin { get; set; }
        /// <summary>
        /// Declaration of the sensor type.
        /// </summary>
        public SensorType? sensor_type { get; set; }
    }

    ///<summary>
    /// Represents the type of sensor. Contains information about the sensor's manufacturer and name.
    /// </summary>
    public class SensorType
    {
        /// <summary>
        /// Identifier of the sensor type.
        /// </summary>
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        /// <summary>
        /// Name of the sensor type. (String declaration of sensor type)
        /// </summary>
        public string? name { get; set; }
        /// <summary>
        /// Name/Brand of sensors manufacturer.
        /// </summary>
        public string? manufacturer { get; set; }
    }

    /// <summary>
    /// Represents a single data value from the sensor. Contains the value and its type.
    /// </summary>
    public class SensorDataValues
    {
        /// <summary>
        /// Identifier of the exact sensor data value.
        /// </summary>
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
        /// <summary>
        /// Value read by sensor.
        /// </summary>
        public string? value { get; set; }
        /// <summary>
        /// Type of the value read by sensor.
        /// </summary>
        public string? value_type { get; set; }
    }
}
