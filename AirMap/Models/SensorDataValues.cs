using System.ComponentModel.DataAnnotations;


namespace AirMap.Models
{
    /// <summary>
    ///     Instance of SensorDataValues class values with preferred value-types - used for Lists of SensorDataValues.
    /// </summary>
    public class SensorDataValues
    {
        /// <summary>
        ///     Identity number of the sensor data value for auto ID within DB.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     Identity number of the sensor data value from source API.
        /// </summary>
        public long? SourceApiId { get; set; }

        /// <summary>
        ///     Value of the sensor data, parsed from string to double.
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        ///     Type of the sensor data value, presented in string.
        /// </summary>
        public required string ValueType { get; set; }
    }
}
