using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Controllers
{
    public class HealthCheckController:ControllerBase
    {
        // GET: api/Teams
        [HttpGet]
        public ActionResult GetHealthCheck()
        {
            return Ok("连接正常");
        }
    }
}
