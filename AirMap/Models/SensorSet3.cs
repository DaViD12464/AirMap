namespace AirMap.Models
{//model for dataset GIOS
    public class SensorSet3
    {
        public int? id { get; set; }
        public string? stationName { get; set; }
        public string? gegrLat { get; set; }
        public string? gegrLon { get; set; }
        public City? city { get; set; }
        public string? addressStreet { get; set; }
    }

    public class City
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public Commune? commune { get; set; }
    }

    public class Commune
    {
        public string? communeName { get; set; }
        public string? districtName { get; set; }
        public string? provinceName { get; set; }
    }

}
