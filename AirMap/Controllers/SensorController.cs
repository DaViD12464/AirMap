using Microsoft.AspNetCore.Mvc;
using AirMap.Data;
using AirMap.Helpers;
using AirMap.Models;

namespace AirMap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        // redundant endpoint at this point - temporarily kept for future usage
        #region GetIconDataEndpoint 


        [HttpPost("GetIconData")]   
        public ActionResult<string> GetIconData([FromBody] SensorModel sensor)
        {
            var iconData = AirQualityHelper.IconHelper(sensor);
            return Ok(iconData);
        }
        #endregion


        [HttpPost("GetIconDataBatch")]
        public ActionResult<List<object>> GetIconDataBatchWithIds([FromBody] List<SensorModel> sensors)
        {
            var result = sensors.Select(sensor => new {
                sensorId = sensor.Id,
                icon = AirQualityHelper.IconHelper(sensor)
            }).ToList<object>();

            return Ok(result);
        }
    }
}
