using Microsoft.AspNetCore.Mvc;
using VillaSport.MicroService.Repositories;

namespace VillaSport.MicroService.Controllers
{
    [Route("api/villasport")]
    [ApiController]
    public class VillaSportController : ControllerBase
    {
        private readonly IVillaSportRepository _villaSportRepository;

        public VillaSportController(IVillaSportRepository villaSportRepository)
        {
            _villaSportRepository = villaSportRepository;
        }

        // GET api/villasport
        [HttpGet]
        public ActionResult<string> Get()
        {
            return _villaSportRepository.GetWelcomeMessage();
        }
    }
}
