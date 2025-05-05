using AirMap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Newtonsoft.Json;
using System.Collections.Generic;
using AirMap.DTOs;

//TODO: do przeróbki całkowitej pod DB - DataService file

[ApiController]
[Route("api/[controller]")]
public class DataService : ControllerBase
{
    private readonly ILogger<DataService> _logger;

    public DataService(ILogger<DataService> logger)
    {
        _logger = logger;
    }


    [HttpPost("process-sensors")]
    public Task<IActionResult> ProcessSensorsData()  //public async Task - removed to remove warn from Error List as this file is to be rebuilt
    {
        return null; // additional "return null;" to avoid compilation error
    }
} //additional }
/*
try
{
    string folderPath = "Data\\api\\sensors";
    var filePaths = Directory.GetFiles(folderPath, "*.json");

    if (!filePaths.Any())
        return NotFound("No JSON files found in the specified directory.");

    var allSensors = new List<SensorModel>();

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
//LookO2
if (jsonData.Contains("\"Device\"")) return "set1";
// Sensor.Community
if (jsonData.Contains("\"sensor\"") &&
    jsonData.Contains("\"location\"") &&
    jsonData.Contains("\"sensordatavalues\"") &&
    jsonData.Contains("\"value_type\""))
{
    return "set2";
}

return "unknown";
}

//Set1 for LookO2
private static List<SensorModel> ProcessSet1(string jsonData)
{
var sensors = JsonConvert.DeserializeObject<List<LookO2Dto>>(jsonData) ?? new List<LookO2Dto>();
return sensors.Select(s => new SensorModel
{
    Device = s.Device,
    PM1 = s.PM1?.ToString(),
    PM25 = s.PM25?.ToString(),
    PM10 = s.PM10?.ToString(),
    Epoch = s.Epoch?.ToString(),
    Latitude = s.Latitude?.ToString(),
    Longitude = s.Longitude?.ToString(),
    IJP = s.IJP?.ToString(),
    IJPStringEN = s.IJPStringEN,
    IJPString = s.IJPString,
    IJPDescription = s.IJPDescription,
    IJPDescriptionEN = s.IJPDescriptionEN,
    Color = s.Color,
    Temperature = s.Temperature?.ToString(),
    Humidity = s.Humidity?.ToString(),
    AveragePM1 = s.AveragePM1?.ToString(),
    AveragePM25 = s.AveragePM25?.ToString(),
    AveragePM10 = s.AveragePM10?.ToString(),
    Name = s.Name,
    Indoor = s.Indoor?.ToString(),
    PreviousIJP = s.PreviousIJP?.ToString(),
    HCHO = s.HCHO?.ToString(),
    AverageHCHO = s.AverageHCHO?.ToString(),
    LocationName = s.Name
}).ToList();
}

//Set2 for Sensor-Community
private static List<Sensor> ProcessSet2(string jsonData)
{
var rawData = JsonConvert.DeserializeObject<List<SensorCommunityDto>>(jsonData) ?? new List<SensorCommunityDto>();

var sensors = new List<Sensor>();

foreach (var entry in rawData)
{
    if (entry.location == null || entry.sensor2 == null || entry.sensordatavalues == null)
        continue;

    var pm10 = entry.sensordatavalues.FirstOrDefault(x => x.value_type == "P1")?.value;
    var pm25 = entry.sensordatavalues.FirstOrDefault(x => x.value_type == "P2")?.value;

    sensors.Add(new Sensor
    {
        Device = entry.sensor2.id?.ToString(),
        PM10 = pm10,
        PM25 = pm25,
        Latitude = entry.location.latitude,
        Longitude = entry.location.longitude,
        Epoch = entry.timestamp?.ToString("o"),
        LocationName = "Sensor.Community"
    });
}

return sensors;
}



private static List<Sensor> RemoveDuplicatesByCoordinates(List<Sensor> sensors)
{
return sensors.GroupBy(s => new { s.Latitude, s.Longitude })
              .Select(g => g.First())
              .ToList();
}
}
*/