using System.Globalization;
using AirMap.Models;

namespace AirMap.Helpers
{
    public static class ModelHelper
    {
        /// <summary>
        ///     Parses a string into a nullable float.
        /// </summary>
        public static float? ParseFloat(string? value)
        {
            return float.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : null;
        }

        /// <summary>
        ///     Parses a string into a nullable int.
        /// </summary>
        public static int? ParseInt(string? value)
        {
            return int.TryParse(value, out var result) ? result : null;
        }

        /// <summary>
        ///     Parses a string into a nullable double.
        /// </summary>
        public static double? ParseDouble(string? value)
        {
            return double.TryParse(value, CultureInfo.InvariantCulture, out var result) ? result : null;
        }

        /// <summary>
        ///     Parses a string ("1" or "0") into a nullable boolean.
        /// </summary>
        public static bool? ParseBool(string? value)
        {
            return value == "1" ? true : value == "0" ? false : null;
        }


        /// <summary>
        ///     Parses a string representing a Unix epoch timestamp into a DateTime.
        /// </summary>
        public static DateTime? ParseEpoch(string? epoch)
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
        public static List<SensorDataValues> ParseSensorDataValuesList(List<DTOs.SensorDataValues> sdv)
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
                    SourceApiId = item.Id == 0 ? rand.Next(1, Math.Abs(parsedValue.GetHashCode() + item.Value_Type!.GetHashCode())) : item.Id, // add id generating algorithm when item.id is 0
                    Value = parsedValue,
                    ValueType = item.Value_Type ?? "n/a"
                });
            }

            return result;
        }

        public static List<long?> GetSensorDataValuesIds(List<DTOs.SensorDataValues> sdv)
        {
            return sdv.Select(item => item.Id).Select(dummy => (long?)dummy).ToList();
        }
    }
}
