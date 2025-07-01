using Microsoft.AspNetCore.Mvc;
using Plane_Crash_Visualization.Services;

namespace Plane_Crash_Visualization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AircraftAnalysisController : ControllerBase
    {
        private readonly DatabaseUpdateService _databaseUpdateService;
        private readonly ILogger<AircraftAnalysisController> _logger;

        public AircraftAnalysisController(
            DatabaseUpdateService databaseUpdateService,
            ILogger<AircraftAnalysisController> logger)
        {
            _databaseUpdateService = databaseUpdateService;
            _logger = logger;
        }

        /// <summary>
        /// Test the aircraft type extraction logic with sample data
        /// </summary>
        [HttpGet("test")]
        public IActionResult TestExtractionLogic()
        {
            try
            {
                _databaseUpdateService.TestAircraftTypeExtraction();
                return Ok(new { message = "Test completed. Check console output for results." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get analysis report of current aircraft types in the database
        /// </summary>
        [HttpGet("report")]
        public async Task<IActionResult> GetAnalysisReport()
        {
            try
            {
                var report = await _databaseUpdateService.GetAnalysisReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analysis report");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get sample aircraft types from the database
        /// </summary>
        [HttpGet("samples")]
        public async Task<IActionResult> GetSampleAircraftTypes([FromQuery] int count = 20)
        {
            try
            {
                var samples = await _databaseUpdateService.GetSampleAircraftTypesAsync(count);
                return Ok(new { samples });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sample aircraft types");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update all aircraft records to split AC_Type into Manufacturer and AC_Model
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateAircraftManufacturersAndModels()
        {
            try
            {
                _logger.LogInformation("Starting aircraft type update process...");
                
                var (updated, failed) = await _databaseUpdateService.UpdateAircraftManufacturersAndModelsAsync();
                
                var result = new
                {
                    message = "Aircraft type update completed",
                    updated,
                    failed,
                    timestamp = DateTime.UtcNow
                };

                _logger.LogInformation($"Update completed: {updated} updated, {failed} failed");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during aircraft type update");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Preview what the update would do without actually updating the database
        /// </summary>
        [HttpGet("preview")]
        public async Task<IActionResult> PreviewUpdate([FromQuery] int count = 10)
        {
            try
            {
                var samples = await _databaseUpdateService.GetSampleAircraftTypesAsync(count);
                var aircraftTypeService = new AircraftTypeService();
                
                var preview = samples.Select(acType =>
                {
                    var (manufacturer, model) = aircraftTypeService.ExtractManufacturerAndModel(acType);
                    return new
                    {
                        originalAcType = acType,
                        extractedManufacturer = manufacturer,
                        extractedModel = model
                    };
                }).ToList();

                return Ok(new { preview });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating preview");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
