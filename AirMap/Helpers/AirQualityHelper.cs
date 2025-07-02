using AirMap.Models;
using Microsoft.JSInterop;

namespace AirMap.Helpers
{
    public static class AirQualityHelper
    {
        [JSInvokable("IconHelper")]
        public static string? IconHelper(SensorModel sensor)
        {
            try
            {
                if (sensor.SensorDataValues != null)
                    return "veryGoodAirQualityIcon";
                else
                    return "badAirQualityIcon";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AQ_Helper: {ex.Message}");
                return null;
            }
        }
    }
}
