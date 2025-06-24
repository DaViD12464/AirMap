using System.ComponentModel.DataAnnotations;

namespace AirMap.Models
{
    /// <summary>
    ///     Instance of Location table values with preferred value-types.
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     Identity number of the location for auto ID within DB.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     Identity number of the location from source API.
        /// </summary>
        public long? SourceApiId { get; set; }

        /// <summary>
        ///     Latitude of the sensor's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        ///     Longitude of the sensor's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        ///     Altitude of the sensor's location.
        /// </summary>
        public double? Altitude { get; set; }

        /// <summary>
        ///     Country of the sensor's location.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        ///     Indicates if the sensor is installed indoors.
        /// </summary>
        public bool? Indoor { get; set; }

        /// <summary>
        ///     Exact_location status of the sensor's location. Indicates if the location is exact or approximate.
        /// </summary>
        public bool? ExactLocation { get; set; }
    }
}
