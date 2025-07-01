using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Data;
using Plane_Crash_Visualization.Models;

namespace Plane_Crash_Visualization.Services
{
    public class DatabaseUpdateService
    {
        private readonly CrashDbContext _context;
        private readonly AircraftTypeService _aircraftTypeService;
        private readonly ILogger<DatabaseUpdateService> _logger;

        public DatabaseUpdateService(
            CrashDbContext context, 
            AircraftTypeService aircraftTypeService, 
            ILogger<DatabaseUpdateService> logger)
        {
            _context = context;
            _aircraftTypeService = aircraftTypeService;
            _logger = logger;
        }

        public async Task<(int Updated, int Failed)> UpdateAircraftManufacturersAndModelsAsync()
        {
            _logger.LogInformation("Starting aircraft type analysis and database update...");

            var crashes = await _context.Crashes
                .Where(c => !string.IsNullOrEmpty(c.AC_Type))
                .ToListAsync();

            _logger.LogInformation($"Found {crashes.Count} crashes with aircraft type data");

            int updated = 0;
            int failed = 0;
            var failedEntries = new List<(int Id, string AC_Type, string Error)>();

            foreach (var crash in crashes)
            {
                try
                {
                    var (manufacturer, model) = _aircraftTypeService.ExtractManufacturerAndModel(crash.AC_Type);
                    
                    // Only update if we got valid results
                    if (!string.IsNullOrWhiteSpace(manufacturer) && !string.IsNullOrWhiteSpace(model))
                    {
                        crash.Manufacturer = manufacturer;
                        crash.AC_Model = model;
                        updated++;
                    }
                    else
                    {
                        _logger.LogWarning($"Could not extract manufacturer/model for: {crash.AC_Type}");
                        failedEntries.Add((crash.Id, crash.AC_Type ?? "", "Could not extract valid manufacturer/model"));
                        failed++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing crash ID {crash.Id} with AC_Type: {crash.AC_Type}");
                    failedEntries.Add((crash.Id, crash.AC_Type ?? "", ex.Message));
                    failed++;
                }
            }

            // Save changes to database
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated {updated} records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }

            // Log failed entries if any
            if (failedEntries.Any())
            {
                _logger.LogWarning($"Failed to process {failed} entries:");
                foreach (var (id, acType, error) in failedEntries.Take(10)) // Show first 10 failures
                {
                    _logger.LogWarning($"  ID {id}: '{acType}' - {error}");
                }
                if (failedEntries.Count > 10)
                {
                    _logger.LogWarning($"  ... and {failedEntries.Count - 10} more");
                }
            }

            return (updated, failed);
        }

        public async Task<Dictionary<string, object>> GetAnalysisReportAsync()
        {
            var crashes = await _context.Crashes.ToListAsync();
            
            var aircraftTypes = crashes
                .Where(c => !string.IsNullOrWhiteSpace(c.AC_Type))
                .Select(c => c.AC_Type!)
                .ToList();

            var manufacturerCounts = _aircraftTypeService.AnalyzeManufacturers(aircraftTypes);

            var report = new Dictionary<string, object>
            {
                ["TotalCrashes"] = crashes.Count,
                ["CrashesWithAircraftType"] = aircraftTypes.Count,
                ["UniqueAircraftTypes"] = aircraftTypes.Distinct().Count(),
                ["TopManufacturers"] = manufacturerCounts.Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                ["UpdatedRecords"] = crashes.Count(c => !string.IsNullOrWhiteSpace(c.Manufacturer)),
                ["PendingRecords"] = crashes.Count(c => !string.IsNullOrWhiteSpace(c.AC_Type) && string.IsNullOrWhiteSpace(c.Manufacturer))
            };

            return report;
        }

        public async Task<List<string>> GetSampleAircraftTypesAsync(int count = 20)
        {
            return await _context.Crashes
                .Where(c => !string.IsNullOrWhiteSpace(c.AC_Type))
                .Select(c => c.AC_Type!)
                .Distinct()
                .Take(count)
                .ToListAsync();
        }

        public void TestAircraftTypeExtraction()
        {
            _aircraftTypeService.TestExtractionLogic();
        }
    }
}
