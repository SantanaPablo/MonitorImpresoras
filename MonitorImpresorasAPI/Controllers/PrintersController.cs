using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MonitorImpresorasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrintersController : ControllerBase
    {
        private readonly IPrinterService _printerService;
        private readonly ILogger<PrintersController> _logger;

        public PrintersController(
            IPrinterService printerService,
            ILogger<PrintersController> logger)
        {
            _printerService = printerService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las impresoras con su información SNMP actualizada
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var printers = await _printerService.GetAllPrintersAsync();
                return Ok(printers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las impresoras");
                return StatusCode(500, new { message = "Error al obtener las impresoras", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una impresora específica por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var printer = await _printerService.GetPrinterByIdAsync(id);

                if (printer == null)
                    return NotFound(new { message = $"Impresora con ID {id} no encontrada" });

                return Ok(printer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener impresora {PrinterId}", id);
                return StatusCode(500, new { message = "Error al obtener la impresora", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene impresoras filtradas por ubicación
        /// </summary>
        [HttpGet("location/{locationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            try
            {
                var printers = await _printerService.GetPrintersByLocationAsync(locationId);
                return Ok(printers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener impresoras de ubicación {LocationId}", locationId);
                return StatusCode(500, new { message = "Error al obtener impresoras por ubicación", error = ex.Message });
            }
        }
    }
}