namespace AirMap.Models
{ // data set for sensor community
    public class SensorSet2
    {
        public long? id { get; set; }
        public string? sampling_rate { get; set; }
        public DateTime? timestamp { get; set; }
        public Location? location { get; set; }
        public Sensor2? sensor2 { get; set; }
        public List<SensorDataValues>? sensordatavalues { get; set; }
    }

    public class Location
    {
        public long? id { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
        public string? altitude { get; set; }
        public string? country { get; set; }
        public int? exact_location { get; set; }
        public int? indoor { get; set; }
    }

    public class Sensor2
    {
        public long? id { get; set; }
        public string? pin { get; set; }
        public SensorType? sensor_type { get; set; }
    }

    public class SensorType
    {
        public long? id { get; set; }
        public string? name { get; set; }
        public string? manufacturer { get; set; }
    }

    public class SensorDataValues
    {
        public long? id { get; set; }
        public double? value { get; set; }
        public string? value_type { get; set; }
    }
}
