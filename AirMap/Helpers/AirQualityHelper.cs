using System.Text;
using AirMap.Models;
using Microsoft.JSInterop;

namespace AirMap.Helpers;

public static class AirQualityHelper
{
    [JSInvokable("IconHelper")]
    public static string? IconHelper(SensorModel sensor)
    {
        (double min, double max)[] Pm1Thresholds =
        [
            (0, 10), // veryGoodAirQualityIcon
            (11, 20), // goodAirQualityIcon
            (21, 35), // moderateAirQualityIcon
            (36, 50), // sufficientAirQualityIcon
            (51, 75), // badAirQualityIcon
            (76, int.MaxValue) // veryBadAirQualityIcon
        ];

        (double min, double max)[] Pm25Thresholds =
        [
            (0, 12), // veryGoodAirQualityIcon
            (13, 25), // goodAirQualityIcon
            (26, 35), // moderateAirQualityIcon
            (36, 50), // sufficientAirQualityIcon
            (51, 75), // badAirQualityIcon
            (76, int.MaxValue) // veryBadAirQualityIcon
        ];

        (double min, double max)[] Pm10Thresholds =
        [
            (0, 20), // veryGoodAirQualityIcon
            (21, 35), // goodAirQualityIcon
            (36, 50), // moderateAirQualityIcon
            (51, 100), // sufficientAirQualityIcon
            (101, 150), // badAirQualityIcon
            (151, int.MaxValue) // veryBadAirQualityIcon
        ];

        (double min, double max)[] Pm4Thresholds =
        [
            (0, 16),    // veryGoodAirQualityIcon
            (17, 30),   // goodAirQualityIcon
            (31, 45),   // moderateAirQualityIcon
            (46, 65),   // sufficientAirQualityIcon
            (66, 90),   // badAirQualityIcon
            (91, double.MaxValue) // veryBadAirQualityIcon
        ];

        try
        {
            var airQualityLevel = 0;
            if (sensor is { Latitude: not null, Longitude: not null }) //Look02
            {
                if (sensor is { Pm1: not null, Pm25: not null, Pm10: not null })
                {
                    var pm1Level = GetAirQualityLevel(sensor.Pm1!.Value, Pm1Thresholds);
                    var pm25Level = GetAirQualityLevel(sensor.Pm25!.Value, Pm25Thresholds);
                    var pm10Level = GetAirQualityLevel(sensor.Pm10!.Value, Pm10Thresholds);

                    airQualityLevel = Math.Max(pm1Level, Math.Max(pm25Level, pm10Level));
                }
                else return "unknownAirQualityIcon";
            }

            if (sensor.Location != null)
                if (sensor.Location is { Latitude: not null, Longitude: not null }) //SC
                {
                    if (sensor.SensorDataValues != null)
                    {
                        var knownTypes = new Dictionary<string, (double min, double max)[]>
                        {
                            { "P0", Pm1Thresholds },
                            { "P1", Pm10Thresholds },
                            { "P2", Pm25Thresholds },
                            { "P4", Pm4Thresholds}
                        };

                        var sensorValues = sensor.SensorDataValues
                            .Where(x => knownTypes.ContainsKey(x.ValueType))
                            .ToList();

                        // No values found
                        if (sensorValues.Count == 0)
                            return "unknownAirQualityIcon";

                        airQualityLevel = 1;

                        foreach (var value in sensorValues)
                        {
                            if (value.Value is not { } v)
                                continue;

                            var thresholds = knownTypes[value.ValueType];
                            var level = GetAirQualityLevel(v, thresholds);
                            airQualityLevel = Math.Max(airQualityLevel, level);
                        }
                    }
                    else
                    {
                        throw new Exception("Exception in AQ_ICONHelper: SensorDataValues is Null.");
                    }
                }
                else
                {
                    throw new Exception(
                        "Exception in AQ_ICONHelper: Unknown sensor, sensor has NULL Latitude & Longitude values.");
                }

            switch (airQualityLevel)
            {
                case 1: return "veryGoodAirQualityIcon";
                case 2: return "goodAirQualityIcon";
                case 3: return "moderateAirQualityIcon";
                case 4: return "sufficientAirQualityIcon";
                case 5: return "badAirQualityIcon";
                case 6: return "veryBadAirQualityIcon";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AQ_ICONHelper: {ex.Message}");
            return null;
        }

        return "";
    }

    private static int GetAirQualityLevel(double value, (double min, double max)[] thresholds)
    {
        for (var i = 0; i < thresholds.Length; i++)
            if (value >= thresholds[i].min && value <= thresholds[i].max)
                return i+1;

        return 0;
    }


    [JSInvokable("TextHelper")]
    public static string TextHelper(SensorModel sensor)
    {
        try
        {
            if (sensor is { Latitude: not null, Longitude: not null }) //Look02
            {
                StringBuilder L2 = new StringBuilder();
                //var builder = new StringBuilder();
                //using var stringWriter = new StringWriter(builder)
                //{
                //    NewLine = "<br/>"
                //};
                if (!string.IsNullOrWhiteSpace(sensor.Name))
                    L2.AppendLine($"Name: {sensor.Name}");
                else 
                    L2.AppendLine("Name: LookO2_Sensor");
                if (sensor.Pm1.HasValue)
                    L2.AppendLine($"PM1: {sensor.Pm1.Value} µg/m³");
                if (sensor.Pm25.HasValue)
                    L2.AppendLine($"PM2.5: {sensor.Pm25.Value} µg/m³");
                if (sensor.Pm10.HasValue)
                    L2.AppendLine($"PM10: {sensor.Pm10.Value} µg/m³");
                if (sensor.Hcho.HasValue && sensor.Hcho != 0)
                    L2.AppendLine($"HcHo: {sensor.Hcho.Value} µg/m³");
                L2.AppendLine("---------------------------------------------------");
                if (sensor.AveragePm1.HasValue)
                    L2.AppendLine($"Średnie PM1: {sensor.AveragePm1.Value} µg/m³");
                if (sensor.AveragePm25.HasValue)
                    L2.AppendLine($"Średnie PM2.5: {sensor.AveragePm25.Value} µg/m³");
                if (sensor.AveragePm10.HasValue)
                    L2.AppendLine($"Średnie PM10: {sensor.AveragePm10.Value}  µg/m³");
                if (sensor.AverageHcho.HasValue)
                    L2.AppendLine($"Średnie HCHO: {sensor.AverageHcho.Value}  µg/m³");
                L2.AppendLine("---------------------------------------------------");
                if (sensor.Temperature.HasValue && sensor.Temperature != 0)  // workaround - will not show values if Temperature is 0°C !!! 
                    L2.AppendLine($"Temperatura: {sensor.Temperature.Value} °C");
                if (sensor.Humidity.HasValue && sensor.Humidity != 0)
                    L2.AppendLine($"Wilgotność: {sensor.Humidity.Value} %");
                L2.AppendLine("---------------------------------------------------");
                if (sensor.IjpString is not null)
                {
                    L2.AppendLine("Wskaźnik jakości powietrza:");
                    L2.AppendLine($"{sensor.IjpString}");
                }
                if (sensor.IjpDescription is not null)
                {
                    L2.AppendLine("---------------------------------------------------");
                    L2.AppendLine($"{sensor.IjpDescription}");
                    L2.AppendLine("---------------------------------------------------");
                }


                return L2.ToString().Replace("\r\n", "<br/>");
            }

            if (sensor.Location is { Latitude: not null, Longitude: not null }) //SC 
            {
                StringBuilder SC = new StringBuilder();
                //var builder = new StringBuilder();
                //using var stringWriter = new StringWriter(builder)
                //{
                //    NewLine = "<br/>"
                //};
                if (!string.IsNullOrWhiteSpace(sensor.Name))
                    SC.AppendLine($"Name: {sensor.Name}");
                else
                    SC.AppendLine("Name: SensorCommunity_Sensor");
                if (sensor.Location.Country != null && sensor.Timestamp != null)
                    SC.AppendLine($"{sensor.Location.Country} | {sensor.Timestamp}");
                SC.AppendLine("---------------------------------------------------");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P0")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"PM1: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P0")).FirstOrDefault()?.Value.ToString()} µg/m³");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P2")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"PM2.5: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P2")).FirstOrDefault()?.Value.ToString()} µg/m³");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P1")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"PM10: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P1")).FirstOrDefault()?.Value.ToString()} µg/m³");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P4")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"PM4: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P4")).FirstOrDefault()?.Value.ToString()} µg/m³");
                if ((sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P0")).FirstOrDefault()?.Value == null) &&
                    (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P1")).FirstOrDefault()?.Value == null) &&
                    (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P2")).FirstOrDefault()?.Value == null) &&
                    (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P4")).FirstOrDefault()?.Value == null))
                    SC.AppendLine("Brak danych z odczytów PM1 / PM2.5 / PM10 / PM4.");
                SC.AppendLine("---------------------------------------------------");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("temperature")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"Temperatura: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("temperature")).FirstOrDefault()?.Value.ToString()} °C");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("humidity")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"Wilgotność: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("humidity")).FirstOrDefault()?.Value.ToString()} %");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("pressure_at_sealevel")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"Ciśnienie: {(sensor.SensorDataValues?.Where(x => x.ValueType.Equals("pressure_at_sealevel")).FirstOrDefault()?.Value /100).ToString()} hPa");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("noise_LAeq")).FirstOrDefault()?.Value != null)
                    SC.AppendLine($"Głośność: {sensor.SensorDataValues?.Where(x => x.ValueType.Equals("noise_LAeq")).FirstOrDefault()?.Value.ToString()} dB");
                if (sensor.SensorDataValues?.Where(x => x.ValueType.Equals("temperature")).FirstOrDefault()?.Value != null ||
                    sensor.SensorDataValues?.Where(x => x.ValueType.Equals("humidity")).FirstOrDefault()?.Value != null ||
                    sensor.SensorDataValues?.Where(x => x.ValueType.Equals("pressure_at_sealevel")).FirstOrDefault()?.Value != null || 
                    sensor.SensorDataValues?.Where(x => x.ValueType.Equals("noise_LAeq")).FirstOrDefault()?.Value != null) 
                    SC.AppendLine("---------------------------------------------------");
                return SC.ToString().Replace("\r\n", "<br/>");
            }
            else
            {
                throw new Exception("Exception in AQ_TEXTHelper: Unknown sensor, sensor has NULL Latitude & Longitude values.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AQ_TEXTHelper: {ex.Message}");
            return null!;
        }
    }
}