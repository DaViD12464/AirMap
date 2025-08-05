using Microsoft.AspNetCore.Mvc;
using AirMap.Data;
using AirMap.Helpers;
using AirMap.Models;
using Microsoft.EntityFrameworkCore;

namespace AirMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController(AppDbContext dbContext) : ControllerBase
    {
        private readonly AppDbContext _dbContext = dbContext;

        // redundant endpoint at this point - temporarily kept for future usage
        #region Redundant/Deprecated


        [HttpPost("GetIconData")]
        public ActionResult<string> GetIconData([FromBody] SensorModel sensor)
        {
            var iconData = AirQualityHelper.IconHelper(sensor);
            return Ok(iconData);
        }
        #endregion


        [HttpPost("GetIconDataBatch")]
        public ActionResult<List<object>> GetIconDataBatch([FromBody] List<SensorModel> sensors)
        {
            var result = sensors.Select(sensor => new {
                sensorId = sensor.Id,
                icon = AirQualityHelper.IconHelper(sensor)
            }).ToList<object>();

            return Ok(result);
        }

        [HttpPost("GetPopUpDataBatch")]
        public ActionResult<List<object>> GetPopUpDataBatch([FromBody] List<SensorModel> sensors)
        {
            var result = sensors.Select(sensor => new
            {
                sensorId = sensor.Id,
                textResults = AirQualityHelper.TextHelper(sensor)
            }).ToList<object>();
            return Ok(result);
        }

        [HttpGet("GetAllSensorData")]
        public async Task<ActionResult<List<SensorModel>>> GetAllSensorData()
        {
            var allSensors = await _dbContext.Set<SensorModel>()
                .Include(s => s.Location)
                .Include(s => s.Sensor)
                .Include(s => s.Sensor!.SensorType)
                .Include(s => s.SensorDataValues)
                .ToListAsync();

            return Ok(allSensors);
        }
        //not used, kept if manual data pulls would be needed
        #region ManualSensorDataPull 

        [HttpGet("GetAllSensorDataManual")]
        public async Task<ActionResult<List<SensorModel>>> GetAllSensorDataManual()
        {
            var allSensors = await DatabaseHelper.GetAllAsync<SensorModel>(_dbContext);

            var tasks = new[]
            {
                DatabaseHelper.GetMissingData<SensorModel, Location>(allSensors!, _dbContext),
                DatabaseHelper.GetMissingData<SensorModel, Sensor>(allSensors!, _dbContext),
                DatabaseHelper.GetMissingDataFromList<SensorModel, SensorDataValues>(allSensors, _dbContext)
            };

            var results = await Task.WhenAll(tasks);

            var modelsWithSensor = results
                .SelectMany(list => list)
                .Where(m => m.Sensor != null)
                .ToList();

            var sensorsFilled = await DatabaseHelper.GetMissingData<Sensor, SensorType>(
                modelsWithSensor.Select(m => m.Sensor!).ToList(), _dbContext);

            for (int i = 0; i < modelsWithSensor.Count; i++)
            {
                if (sensorsFilled[i] != null)
                {
                    modelsWithSensor[i].Sensor = sensorsFilled[i];
                }
            }

            #endregion

            return Ok(results[2]);
        }


    }
}
