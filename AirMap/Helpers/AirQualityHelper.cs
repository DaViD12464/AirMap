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
                            return "unknownAirQualityIcon"; // TODO: add n/a icon name after adding unknown data icon to frontend

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

    private static int GetAirQualityLevel(double value, (double min, double max)[] thresholds) //TODO:add logic
    {
        for (var i = 0; i < thresholds.Length; i++)
            if (value >= thresholds[i].min && value <= thresholds[i].max)
                return i+1;

        return 0;
    }


    [JSInvokable("TextHelper")] //TODO: Create an TextBuilders for SensorCommunity and Look02 sensors
    public static Dictionary<string, object>? TextHelper(SensorModel sensor)
    {
        try
        {
            if (sensor is { Latitude: not null, Longitude: not null }) //Look02
                return new Dictionary<string, object>
                {
                    { "Name", sensor.Name ?? "LookO2_Sensor" },
                    { "PM1", sensor.Pm1.ToString() ?? "n/a" },
                    { "PM25", sensor.Pm25.ToString() ?? "n/a" },
                    { "PM10", sensor.Pm10.ToString() ?? "n/a" },
                    { "Latitude", sensor.Latitude },
                    { "Longitude", sensor.Longitude }
                    // Add more key-value pairs as needed to be displayed in Pop-up message
                };
            if (sensor.Location is { Latitude: not null, Longitude: not null }) //SC
                return new
                    Dictionary<string, object> //TODO: Reformat returned pop-up message values to return readings for SensorCommunity 
                    {
                        { "Name", sensor.Name ?? "SensorCommunity_Sensor" },
                        {
                            "PM1",
                            sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P0")).FirstOrDefault()?.Value
                                .ToString() ?? "n/a"
                        },
                        {
                            "PM10",
                            sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P1")).FirstOrDefault()?.Value
                                .ToString() ?? "n/a"
                        },
                        {
                            "PM25",
                            sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P2")).FirstOrDefault()?.Value
                                .ToString() ?? "n/a"
                        },
                        {
                            "PM4",
                            sensor.SensorDataValues?.Where(x => x.ValueType.Equals("P4")).FirstOrDefault()?.Value
                                .ToString() ?? "n/a"
                        },
                        //{ "PM10", sensor.Pm10Value },
                        { "Latitude", sensor.Location.Latitude },
                        { "Longitude", sensor.Location.Longitude }
                        // Add more key-value pairs as needed to be displayed in Pop-up message
                    };

            throw new Exception(
                "Exception in AQ_TEXTHelper: Unknown sensor, sensor has NULL Latitude & Longitude values.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AQ_TEXTHelper: {ex.Message}");
            return null;
        }
    }
}