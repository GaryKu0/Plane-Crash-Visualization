using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Data;
using Plane_Crash_Visualization.Models;
using Plane_Crash_Visualization.Services;

namespace Plane_Crash_Visualization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrashesController : ControllerBase
    {
        private readonly CrashDbContext _context;
        private readonly IGeocodingService _geocodingService;
        private readonly IContinentService _continentService;

        public CrashesController(CrashDbContext context, IGeocodingService geocodingService, IContinentService continentService)
        {
            _context = context;
            _geocodingService = geocodingService;
            _continentService = continentService;
        }

        // GET: api/crashes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCrashes(
            [FromQuery] string? fields = null,
            [FromQuery] int? startYear = null,
            [FromQuery] int? endYear = null,
            [FromQuery] string? operator_ = null,
            [FromQuery] string? manufacturer = null,
            [FromQuery] int? limit = null)
        {
            var query = _context.Crashes.AsQueryable();

            // Apply filters
            if (startYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year >= startYear.Value);
            
            if (endYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year <= endYear.Value);
            
            if (!string.IsNullOrEmpty(operator_))
                query = query.Where(c => c.Operator != null && c.Operator.Contains(operator_));
            
            if (!string.IsNullOrEmpty(manufacturer))
                query = query.Where(c => c.Manufacturer != null && c.Manufacturer.Contains(manufacturer));

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var crashes = await query.OrderBy(c => c.Date).ToListAsync();

            // Return selected fields or full objects
            if (!string.IsNullOrEmpty(fields))
            {
                var fieldList = fields.Split(',').Select(f => f.Trim()).ToList();
                return Ok(crashes.Select(c => SelectFields(c, fieldList)));
            }

            return Ok(crashes);
        }

        // GET: api/crashes/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            var totalCrashes = await _context.Crashes.CountAsync();
            var totalFatalities = await _context.Crashes.SumAsync(c => c.Fatalities ?? 0);
            var totalAboard = await _context.Crashes.SumAsync(c => c.Aboard ?? 0);
            var firstCrash = await _context.Crashes.Where(c => c.Date.HasValue).MinAsync(c => c.Date);
            var lastCrash = await _context.Crashes.Where(c => c.Date.HasValue).MaxAsync(c => c.Date);

            return Ok(new
            {
                TotalCrashes = totalCrashes,
                TotalFatalities = totalFatalities,
                TotalAboard = totalAboard,
                TotalPassengerFatalities = await _context.Crashes.SumAsync(c => c.FatalitiesPassengers ?? 0),
                TotalCrewFatalities = await _context.Crashes.SumAsync(c => c.FatalitiesCrew ?? 0),
                DateRange = new { From = firstCrash, To = lastCrash }
            });
        }

        // GET: api/crashes/by-year
        [HttpGet("by-year")]
        public async Task<ActionResult<IEnumerable<object>>> GetCrashesByYear()
        {
            var crashesByYear = await _context.Crashes
                .Where(c => c.Date.HasValue)
                .GroupBy(c => c.Date!.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    CrashCount = g.Count(),
                    TotalFatalities = g.Sum(c => c.Fatalities ?? 0),
                    TotalAboard = g.Sum(c => c.Aboard ?? 0),
                    PassengerFatalities = g.Sum(c => c.FatalitiesPassengers ?? 0),
                    CrewFatalities = g.Sum(c => c.FatalitiesCrew ?? 0),
                    PassengerAboard = g.Sum(c => c.AboardPassengers ?? 0),
                    CrewAboard = g.Sum(c => c.AboardCrew ?? 0)
                })
                .OrderBy(x => x.Year)
                .ToListAsync();

            return Ok(crashesByYear);
        }

        // GET: api/crashes/by-operator
        [HttpGet("by-operator")]
        public async Task<ActionResult<IEnumerable<object>>> GetCrashesByOperator([FromQuery] int limit = 20)
        {
            var crashesByOperator = await _context.Crashes
                .Where(c => !string.IsNullOrEmpty(c.Operator))
                .GroupBy(c => c.Operator)
                .Select(g => new
                {
                    Operator = g.Key,
                    CrashCount = g.Count(),
                    TotalFatalities = g.Sum(c => c.Fatalities ?? 0),
                    TotalAboard = g.Sum(c => c.Aboard ?? 0)
                })
                .OrderByDescending(x => x.CrashCount)
                .Take(limit)
                .ToListAsync();

            return Ok(crashesByOperator);
        }

        // GET: api/crashes/casualties-breakdown
        [HttpGet("casualties-breakdown")]
        public async Task<ActionResult<object>> GetCasualtiesBreakdown()
        {
            var data = await _context.Crashes
                .Where(c => c.Fatalities.HasValue || c.Aboard.HasValue)
                .Select(c => new
                {
                    c.Id,
                    c.Date,
                    c.Operator,
                    c.Location,
                    TotalAboard = c.Aboard ?? 0,
                    PassengersAboard = c.AboardPassengers ?? 0,
                    CrewAboard = c.AboardCrew ?? 0,
                    TotalFatalities = c.Fatalities ?? 0,
                    PassengerFatalities = c.FatalitiesPassengers ?? 0,
                    CrewFatalities = c.FatalitiesCrew ?? 0,
                    SurvivalRate = c.Aboard.HasValue && c.Aboard > 0 
                        ? (double)(c.Aboard.Value - (c.Fatalities ?? 0)) / c.Aboard.Value 
                        : (double?)null
                })
                .OrderBy(c => c.Date)
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/crashes/timeline
        [HttpGet("timeline")]
        public async Task<ActionResult<IEnumerable<object>>> GetTimeline([FromQuery] string groupBy = "year")
        {
            var query = _context.Crashes.Where(c => c.Date.HasValue);

            if (groupBy.ToLower() == "month")
            {
                var timelineByMonth = await query
                    .GroupBy(c => new { c.Date!.Value.Year, c.Date.Value.Month })
                    .Select(g => new
                    {
                        Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        CrashCount = g.Count(),
                        TotalFatalities = g.Sum(c => c.Fatalities ?? 0)
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToListAsync();

                return Ok(timelineByMonth);
            }
            else
            {
                var timelineByYear = await query
                    .GroupBy(c => c.Date!.Value.Year)
                    .Select(g => new
                    {
                        Period = g.Key.ToString(),
                        Year = g.Key,
                        CrashCount = g.Count(),
                        TotalFatalities = g.Sum(c => c.Fatalities ?? 0)
                    })
                    .OrderBy(x => x.Year)
                    .ToListAsync();

                return Ok(timelineByYear);
            }
        }

        // GET: api/crashes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Crash>> GetCrash(int id)
        {
            var crash = await _context.Crashes.FindAsync(id);

            if (crash == null)
            {
                return NotFound();
            }

            return crash;
        }

        // GET: api/crashes/map-data
        [HttpGet("map-data")]
        public async Task<ActionResult<IEnumerable<object>>> GetMapData(
            [FromQuery] int? startYear = null,
            [FromQuery] int? endYear = null,
            [FromQuery] int? minFatalities = null,
            [FromQuery] int? maxFatalities = null,
            [FromQuery] string? operator_ = null,
            [FromQuery] string? manufacturer = null,
            [FromQuery] int? limit = null)
        {
            var query = _context.Crashes.AsQueryable();

            // Apply filters
            if (startYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year >= startYear.Value);
            
            if (endYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year <= endYear.Value);
            
            if (minFatalities.HasValue)
                query = query.Where(c => (c.Fatalities ?? 0) >= minFatalities.Value);
            
            if (maxFatalities.HasValue)
                query = query.Where(c => (c.Fatalities ?? 0) <= maxFatalities.Value);
            
            if (!string.IsNullOrEmpty(operator_))
                query = query.Where(c => c.Operator != null && c.Operator.Contains(operator_));
            
            if (!string.IsNullOrEmpty(manufacturer))
                query = query.Where(c => c.Manufacturer != null && c.Manufacturer.Contains(manufacturer));

            // Only return crashes that have coordinates (for map display)
            query = query.Where(c => c.Latitude.HasValue && c.Longitude.HasValue);

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var mapData = await query
                .OrderBy(c => c.Date)
                .Select(c => new
                {
                    id = c.Id,
                    date = c.Date,
                    time = c.Time,
                    location = c.Location,
                    latitude = c.Latitude,
                    longitude = c.Longitude,
                    operator_ = c.Operator,
                    flight = c.Flight,
                    route = c.Route,
                    ac_Type = c.AC_Type,
                    registration = c.Registration,
                    cn_ln = c.Cn_ln,
                    aboard = c.Aboard,
                    aboardPassengers = c.AboardPassengers,
                    aboardCrew = c.AboardCrew,
                    fatalities = c.Fatalities,
                    fatalitiesPassengers = c.FatalitiesPassengers,
                    fatalitiesCrew = c.FatalitiesCrew,
                    ground = c.Ground,
                    summary = c.Summary
                })
                .ToListAsync();

            return Ok(mapData);
        }

        // POST: api/crashes/geocode
        [HttpPost("geocode")]
        public async Task<ActionResult<object>> GeocodeAllCrashes([FromQuery] bool force = false)
        {
            try
            {
                var query = _context.Crashes.AsQueryable();
                
                if (!force)
                {
                    // Only geocode crashes that haven't been geocoded yet
                    query = query.Where(c => !c.IsGeocoded && !string.IsNullOrEmpty(c.Location));
                }
                else
                {
                    // Force geocode all crashes with locations
                    query = query.Where(c => !string.IsNullOrEmpty(c.Location));
                }

                var crashesToGeocode = await query.ToListAsync();
                
                var totalCrashes = crashesToGeocode.Count;
                var successCount = 0;
                var failureCount = 0;

                foreach (var crash in crashesToGeocode)
                {
                    if (string.IsNullOrWhiteSpace(crash.Location))
                        continue;

                    var (lat, lng) = await _geocodingService.GeocodeLocationAsync(crash.Location);
                    
                    if (lat.HasValue && lng.HasValue)
                    {
                        crash.Latitude = lat.Value;
                        crash.Longitude = lng.Value;
                        crash.IsGeocoded = true;
                        successCount++;
                    }
                    else
                    {
                        crash.IsGeocoded = true; // Mark as attempted to avoid repeated failures
                        failureCount++;
                    }

                    // Save every 10 records to avoid losing progress
                    if ((successCount + failureCount) % 10 == 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                // Save any remaining changes
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Geocoding completed",
                    totalProcessed = totalCrashes,
                    successCount = successCount,
                    failureCount = failureCount,
                    percentage = totalCrashes > 0 ? (double)successCount / totalCrashes * 100 : 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Geocoding failed", message = ex.Message });
            }
        }

        // GET: api/crashes/geocoding-stats
        [HttpGet("geocoding-stats")]
        public async Task<ActionResult<object>> GetGeocodingStats()
        {
            var totalCrashes = await _context.Crashes.CountAsync();
            var geocodedCount = await _context.Crashes.CountAsync(c => c.IsGeocoded && c.Latitude.HasValue && c.Longitude.HasValue);
            var attemptedCount = await _context.Crashes.CountAsync(c => c.IsGeocoded);
            var notAttemptedCount = await _context.Crashes.CountAsync(c => !c.IsGeocoded && !string.IsNullOrEmpty(c.Location));

            return Ok(new
            {
                totalCrashes = totalCrashes,
                geocodedSuccessfully = geocodedCount,
                geocodingAttempted = attemptedCount,
                notAttempted = notAttemptedCount,
                successRate = attemptedCount > 0 ? (double)geocodedCount / attemptedCount * 100 : 0
            });
        }

        // GET: api/crashes/test-geocode?location=New York, USA
        [HttpGet("test-geocode")]
        public async Task<ActionResult<object>> TestGeocode([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest(new { error = "Location parameter is required" });
            }

            try
            {
                var (lat, lng) = await _geocodingService.GeocodeLocationAsync(location);
                
                return Ok(new
                {
                    originalLocation = location,
                    latitude = lat,
                    longitude = lng,
                    success = lat.HasValue && lng.HasValue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Geocoding failed", message = ex.Message });
            }
        }

        // Helper method to select specific fields from a crash object
        private object SelectFields(Crash crash, List<string> fields)
        {
            var result = new Dictionary<string, object?>();

            foreach (var field in fields)
            {
                switch (field.ToLower())
                {
                    case "id": result[field] = crash.Id; break;
                    case "date": result[field] = crash.Date; break;
                    case "time": result[field] = crash.Time; break;
                    case "location": result[field] = crash.Location; break;
                    case "operator": result[field] = crash.Operator; break;
                    case "flight": result[field] = crash.Flight; break;
                    case "route": result[field] = crash.Route; break;
                    case "ac_type": result[field] = crash.AC_Type; break;
                    case "registration": result[field] = crash.Registration; break;
                    case "cn_ln": result[field] = crash.Cn_ln; break;
                    case "aboard": result[field] = crash.Aboard; break;
                    case "aboardpassengers": result[field] = crash.AboardPassengers; break;
                    case "aboardcrew": result[field] = crash.AboardCrew; break;
                    case "fatalities": result[field] = crash.Fatalities; break;
                    case "fatalitiespassengers": result[field] = crash.FatalitiesPassengers; break;
                    case "fatalitiescrew": result[field] = crash.FatalitiesCrew; break;
                    case "ground": result[field] = crash.Ground; break;
                    case "summary": result[field] = crash.Summary; break;
                }
            }

            return result;
        }

        // GET: api/crashes/operators
        [HttpGet("operators")]
        public async Task<ActionResult<IEnumerable<string>>> GetOperators()
        {
            var operators = await _context.Crashes
                .Where(c => !string.IsNullOrEmpty(c.Operator))
                .Select(c => c.Operator!)
                .Distinct()
                .OrderBy(o => o)
                .ToListAsync();

            return Ok(operators);
        }

        // GET: api/crashes/manufacturers
        [HttpGet("manufacturers")]
        public async Task<ActionResult<IEnumerable<string>>> GetManufacturers()
        {
            var manufacturers = await _context.Crashes
                .Where(c => !string.IsNullOrEmpty(c.Manufacturer))
                .Select(c => c.Manufacturer!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            return Ok(manufacturers);
        }

        // GET: api/crashes/by-continent
        [HttpGet("by-continent")]
        public async Task<ActionResult<IEnumerable<object>>> GetCrashesByContinent(
            [FromQuery] int? startYear = null,
            [FromQuery] int? endYear = null,
            [FromQuery] string? operator_ = null,
            [FromQuery] string? manufacturer = null)
        {
            var query = _context.Crashes
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue);

            // Apply filters
            if (startYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year >= startYear.Value);
            
            if (endYear.HasValue)
                query = query.Where(c => c.Date.HasValue && c.Date.Value.Year <= endYear.Value);
            
            if (!string.IsNullOrEmpty(operator_))
                query = query.Where(c => c.Operator != null && c.Operator.Contains(operator_));
            
            if (!string.IsNullOrEmpty(manufacturer))
                query = query.Where(c => c.Manufacturer != null && c.Manufacturer.Contains(manufacturer));

            var crashes = await query.ToListAsync();
            
            var crashesByContinent = crashes
                .GroupBy(c => _continentService.GetContinentFromCoordinates(c.Latitude!.Value, c.Longitude!.Value))
                .Select(g => new
                {
                    Continent = g.Key,
                    CrashCount = g.Count(),
                    TotalFatalities = g.Sum(c => c.Fatalities ?? 0),
                    TotalAboard = g.Sum(c => c.Aboard ?? 0),
                    FatalityRate = g.Sum(c => c.Aboard ?? 0) > 0 
                        ? (double)g.Sum(c => c.Fatalities ?? 0) / g.Sum(c => c.Aboard ?? 0) * 100 
                        : 0.0
                })
                .Where(x => x.Continent != "Unknown")
                .OrderByDescending(x => x.CrashCount)
                .ToList();

            return Ok(crashesByContinent);
        }
    }
}