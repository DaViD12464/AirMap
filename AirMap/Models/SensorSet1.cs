using System.ComponentModel.DataAnnotations.Schema;

namespace AirMap.Models
{ // data set for looko2

    public class SensorSet1
    {
        [ForeignKey("Id")]
        public long Id { get; set; } //added Id as for appDBContext 
        public string? Device { get; set; }
        public double? PM1 { get; set; }
        public double? PM25 { get; set; }
        public double? PM10 { get; set; }
        public long? Epoch { get; set; } // Czas UNIX w sekundach
        public double? Lat { get; set; } // Szerokość geograficzna
        public double? Lon { get; set; } // Długość geograficzna
        public int? IJP { get; set; }
        public string? IJPStringEN { get; set; }
        public string? IJPString { get; set; }
        public string? IJPDescription { get; set; }
        public string? IJPDescriptionEN { get; set; }
        public string? Color { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? AveragePM1 { get; set; }
        public double? AveragePM25 { get; set; }
        public double? AveragePM10 { get; set; }
        public string? Name { get; set; }
        public int? Indoor { get; set; }  // 0 lub 1
        public int? PreviousIJP { get; set; }
        public double? HCHO { get; set; }
        public double? AverageHCHO { get; set; }
    }

    // Klasa obsługująca listę czujników
    public class Root
    {
        public List<SensorSet1>? Sensors { get; set; }
    }
}