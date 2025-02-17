namespace AirMap.Models
{//model for dataset airmonitor.pl
    public class SensorSet2
    {
        public string? lat { get; set; }
        public string? @long { get; set; }
        public List<SensorValue>? values { get; set; }
    }

    public class SensorValue
    {
        public double? value { get; set; }
        public string? dt { get; set; }
    }

}
