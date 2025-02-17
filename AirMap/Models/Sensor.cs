namespace AirMap.Models
{
// create a universal model for all datasets
    public class Sensor
    {
        public string? Device { get; set; }
        public string? PM1 { get; set; }
        public string? PM25 { get; set; }
        public string? PM10 { get; set; }
        public string? Epoch { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int IJP { get; set; }
        public string? IJPStringEN { get; set; }
        public string? IJPString { get; set; }
        public string? IJPDescription { get; set; }
        public string? IJPDescriptionEN { get; set; }
        public string? Color { get; set; }
        public string? Temperature { get; set; }
        public string? Humidity { get; set; }
        public string? AveragePM1 { get; set; }
        public string? AveragePM25 { get; set; }
        public string? AveragePM10 { get; set; }
        public  string? Name { get; set; }
        public  string? Indoor { get; set; }
        public  string? PreviousIJP { get; set; }
        public  string? HCHO { get; set; }
        public  string? AverageHCHO { get; set; }

        public string? LocationName { get; set; }



    }
}
