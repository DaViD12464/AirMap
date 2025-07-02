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
        [HttpPost("GetIconData")]
        public ActionResult<string> GetIconData([FromBody] SensorModel sensor)
        {
            var iconData = AirQualityHelper.IconHelper(sensor);
            return Ok(iconData);
        }
    }
}
