using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            return $"VillaSport MS response :) from {Environment.MachineName}";
        }
    }
}
