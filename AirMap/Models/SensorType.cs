using System.ComponentModel.DataAnnotations;

namespace AirMap.Models
{
    /// <summary>
    ///     Instance of Location class values with preferred value-types.
    /// </summary>
    public class SensorType
    {
        /// <summary>
        ///     Identity number of the sensor type for auto ID within DB.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     Identity number of the sensor type for auto ID within DB.
        /// </summary>
        public long? SourceApiId { get; set; }

        /// <summary>
        ///     Name of the sensor.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Manufacturer of the sensor.
        /// </summary>
        public string? Manufacturer { get; set; }
    }
}
