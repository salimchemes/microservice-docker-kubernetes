using System;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace VillaSport.MicroService.Controllers
{
    [Route("api/villasport")]
    [ApiController]
    public class VillaSportController : ControllerBase
    {
        // GET api/villasport
        [HttpGet]
        public ActionResult<string> Get()
        {  
            Log.Information("log info");
            Log.Warning("log warning");
            Log.Error("log error");
            Log.Fatal("log fatal");
            return $"VillaSport MS response :) from {Environment.MachineName}";
        }
    }
}
