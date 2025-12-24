using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MonitorImpresorasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(
            ILocationService locationService,
            ILogger<LocationsController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las ubicaciones
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var locations = await _locationService.GetAllLocationsAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ubicaciones");
                return StatusCode(500, new { message = "Error al obtener ubicaciones", error = ex.Message });
            }
        }
    }
}