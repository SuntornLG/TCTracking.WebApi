
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TCTracking.Service.Interface;

namespace TCTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplayController : ControllerBase
    {
        private readonly IDisplayService _displayService;
        public DisplayController(IDisplayService displayService)
        {
            _displayService = displayService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _displayService.GetDisplayItems();
            return Ok(results);
        }

    }
}
