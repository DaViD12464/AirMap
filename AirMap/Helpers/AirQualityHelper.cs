using AirMap.Models;
using Microsoft.JSInterop;

namespace AirMap.Helpers;

public static class AirQualityHelper
{
    [JSInvokable("IconHelper")]
    public static string? IconHelper(SensorModel sensor)
    {
        try
        {
            if (sensor.Latitude != null && sensor.Longitude != null) //Look02
                if ((sensor.Pm1 >= 0 && sensor.Pm1 <= 10) & (sensor.Pm25 >= 0 && sensor.Pm25 <= 12) &
                    (sensor.Pm10 >= 0 && sensor.Pm10 <= 20)) //veryGoodAirQualityIcon
                    return "veryGoodAirQualityIcon";
                else if ((sensor.Pm1 >= 11 && sensor.Pm1 <= 20) & (sensor.Pm25 >= 13 && sensor.Pm25 <= 25) &
                         (sensor.Pm10 >= 21 && sensor.Pm10 <= 35)) // goodAirQualityIcon
                    return "goodAirQualityIcon";
                else if ((sensor.Pm1 >= 21 && sensor.Pm1 <= 35) & (sensor.Pm25 >= 26 && sensor.Pm25 <= 35) &
                         (sensor.Pm10 >= 36 && sensor.Pm10 <= 50)) // moderateAirQualityIcon
                    return "moderateAirQualityIcon";
                else if ((sensor.Pm1 >= 36 && sensor.Pm1 <= 50) & (sensor.Pm25 >= 36 && sensor.Pm25 <= 50) &
                         (sensor.Pm10 >= 51 && sensor.Pm10 <= 100)) // sufficientAirQualityIcon
                    return "sufficientAirQualityIcon";
                else if ((sensor.Pm1 >= 51 && sensor.Pm1 <= 75) & (sensor.Pm25 >= 51 && sensor.Pm25 <= 75) &
                         (sensor.Pm10 >= 101 && sensor.Pm10 <= 150)) // badAirQualityIcon
                    return "badAirQualityIcon";
                else if ((sensor.Pm1 > 75) & (sensor.Pm25 > 75) & (sensor.Pm10 > 150)) // veryBadAirQualityIcon
                    return "veryBadAirQualityIcon";
                else // issue proofing -- if sensor values are not in the expected range, return empty string to use default icon (defaultGreenIcon)
                    return "";
            if (sensor.Location?.Latitude != null && sensor.Location?.Longitude != null) //SC
                if (sensor.SensorDataValues != null)
                {
                    var sensorValues =
                        sensor.SensorDataValues.Where(x => x.ValueType.Equals("P1") || x.ValueType.Equals("P2"));
                    var pm10 = sensorValues.Where(x => x.ValueType.Equals("P1")).FirstOrDefault()?.Value;
                    var pm25 = sensorValues.Where(x => x.ValueType.Equals("P2")).FirstOrDefault()?.Value;
                    if ((pm25 >= 0 && pm25 <= 12) & (pm10 >= 0 && pm10 <= 20)) //veryGoodAirQualityIcon
                        return "veryGoodAirQualityIcon";
                    else if ((pm25 >= 13 && pm25 <= 25) & (pm10 >= 21 && pm10 <= 35)) // goodAirQualityIcon
                        return "goodAirQualityIcon";
                    else if ((pm25 >= 26 && pm25 <= 35) & (pm10 >= 36 && pm10 <= 50)) // moderateAirQualityIcon
                        return "moderateAirQualityIcon";
                    else if ((pm25 >= 36 && pm25 <= 50) & (pm10 >= 51 && pm10 <= 100)) // sufficientAirQualityIcon
                        return "sufficientAirQualityIcon";
                    else if ((pm25 >= 51 && pm25 <= 75) & (pm10 >= 101 && pm10 <= 150)) // badAirQualityIcon
                        return "badAirQualityIcon";
                    else if ((pm25 > 75) & (pm10 > 150)) // veryBadAirQualityIcon
                        return "veryBadAirQualityIcon";
                    else // issue proofing -- if sensor values are not in the expected range, return empty string to use default icon (defaultGreenIcon)
                        return "";
                }
                else
                {
                    throw new Exception("Exception in AQ_ICONHelper: SensorDataValues is Null.");
                }

            throw new Exception(
                "Exception in AQ_ICONHelper: Unknown sensor, sensor has NULL Latitude & Longitude values.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AQ_ICONHelper: {ex.Message}");
            return null;
        }
    }

    [JSInvokable("TextHelper")]
    public static Dictionary<string, object>? TextHelper(SensorModel sensor)
    {
        try
        {
            if (sensor.Latitude != null && sensor.Longitude != null) //Look02
                return new Dictionary<string, object>
                {
                    { "Name", sensor.Name ?? "LookO2_Sensor" },
                    { "PM1", sensor.Pm1.ToString() ?? "n/a"},
                    { "PM25", sensor.Pm25.ToString() ?? "n/a" },
                    { "PM10", sensor.Pm10.ToString() ?? "n/a" },
                    { "Latitude", sensor.Latitude },
                    { "Longitude", sensor.Longitude }
                    // Add more key-value pairs as needed to be displayed in Pop-up message
                };
            if (sensor.Location?.Latitude != null && sensor.Location?.Longitude != null) //SC
                return new Dictionary<string, object>  //TODO: Reformat returned pop-up message values to return readings for SensorCommunity
                {
                    { "Name", sensor.Name ?? "SensorCommunity_Sensor" },
                    //{ "PM1", sensor. },
                    //{ "PM25", sensor.Pm25Value },
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