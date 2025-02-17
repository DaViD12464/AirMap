using AirMap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Newtonsoft.Json;
using System.Collections.Generic;

//do przeróbki całkowitej pod DB

[ApiController]
[Route("api/[controller]")]
public class SensorController : ControllerBase
{
    private readonly ILogger<SensorController> _logger;

    public SensorController(ILogger<SensorController> logger)
    {
        _logger = logger;
    }

    [HttpPost("process-sensors")]
    public async Task<IActionResult> ProcessSensorsData()
    {
        try
        {
            string folderPath = "Data\\api\\sensors";
            var filePaths = Directory.GetFiles(folderPath, "*.json");

            if (!filePaths.Any())
                return NotFound("No JSON files found in the specified directory.");

            var allSensors = new List<Sensor>();

            foreach (var filePath in filePaths)
            {
                string fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                string type = DetermineDataSetType(fileContent);

                switch (type.ToLower())
                {
                    case "set1":
                        allSensors.AddRange(ProcessSet1(fileContent));
                        break;
                    case "set2":
                        allSensors.AddRange(ProcessSet2(fileContent));
                        break;
                    case "set3":
                        allSensors.AddRange(ProcessSet3(fileContent));
                        break;
                    default:
                        _logger.LogWarning($"Unknown data set type in file: {Path.GetFileName(filePath)}");
                        break;
                }
            }

            var uniqueSensors = RemoveDuplicatesByCoordinates(allSensors);

            // Zapis scalonych danych do pliku
            string outputPath ="Data\\output\\merged_sensors.json";
            string? directoryPath = Path.GetDirectoryName(outputPath);
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }
           
            await System.IO.File.WriteAllTextAsync(outputPath, JsonConvert.SerializeObject(uniqueSensors, Newtonsoft.Json.Formatting.Indented));

            return Ok($"Data processed successfully. Output saved to: {outputPath}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing sensor data.");
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    private static string DetermineDataSetType(string jsonData)
    {
        if (jsonData.Contains("\"Device\"")) return "set1";
        if (jsonData.Contains("\"lat\"") && jsonData.Contains("\"values\"")) return "set2";
        if (jsonData.Contains("\"stationName\"")) return "set3";

        return "unknown";
    }

    private static List<Sensor> ProcessSet1(string jsonData)
    {
        var sensors = JsonConvert.DeserializeObject<List<SensorSet1>>(jsonData) ?? new List<SensorSet1>();
        return sensors.Select(s => new Sensor
        {
            Latitude = s.Lat,
            Longitude = s.Lon,
            PM1 = s.PM1,
            PM25 = s.PM25,
            PM10 = s.PM10,
            LocationName = s.Name
        }).ToList();
    }

    private static List<Sensor> ProcessSet2(string jsonData)
    {
        var rawData = JsonConvert.DeserializeObject<List<SensorSet2>>(jsonData) ?? new List<SensorSet2>();
        return rawData.Select(r => new Sensor
        {
            Latitude = r.lat,
            Longitude = r.@long,
            PM1 = r.values?.FirstOrDefault(v => v.dt == "pm1")?.value?.ToString(),
            PM25 = r.values?.FirstOrDefault(v => v.dt == "pm2.5")?.value?.ToString(),
            PM10 = r.values?.FirstOrDefault(v => v.dt == "pm10")?.value?.ToString()
        }).ToList();
    }

    private static List<Sensor> ProcessSet3(string jsonData)
    {
        try
        {
            // Deserializujemy dane jako listę obiektów SensorSet3
            var data = JsonConvert.DeserializeObject<List<Sensor>>(jsonData) ?? new List<Sensor>();
            return data;
        }
        catch (JsonSerializationException ex)
        {
            // Logowanie błędu dla lepszej analizy
            Console.WriteLine($"Deserializacja danych do SensorSet3 nie powiodła się: {ex.Message}");
            throw;
        }
    }


    private static List<Sensor> RemoveDuplicatesByCoordinates(List<Sensor> sensors)
    {
        return sensors.GroupBy(s => new { s.Latitude, s.Longitude })
                      .Select(g => g.First())
                      .ToList();
    }
}
