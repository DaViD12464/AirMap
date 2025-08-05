using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirMap.Models
{
    /// <summary>
    ///     Instance of Sensor class values with preferred value-types.
    /// </summary>
    public class Sensor
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
        ///     Pin used by the sensor.
        /// </summary>
        public int? Pin { get; set; }

        /// <summary>
        ///     SensorType table reference, used for sensor type data.
        /// </summary>
        public long? SensorTypeId { get; set; }
        [ForeignKey("SensorTypeId")]
        public SensorType? SensorType { get; set; }
    }
}
