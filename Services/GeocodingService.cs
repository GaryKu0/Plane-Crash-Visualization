using System.Text.Json;

namespace Plane_Crash_Visualization.Services
{
    public interface IGeocodingService
    {
        Task<(double? lat, double? lng)> GeocodeLocationAsync(string location);
    }

    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeocodingService> _logger;
        private readonly Dictionary<string, (double? lat, double? lng)> _cache = new();
        
        // Rate limiting
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private DateTime _lastRequestTime = DateTime.MinValue;
        private readonly TimeSpan _rateLimitDelay = TimeSpan.FromSeconds(1); // 1 second between requests

        public GeocodingService(HttpClient httpClient, ILogger<GeocodingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            // Set user agent as required by Nominatim
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PlanecrashVisualization/1.0");
        }

        public async Task<(double? lat, double? lng)> GeocodeLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return (null, null);

            // Check cache first
            if (_cache.ContainsKey(location))
            {
                _logger.LogDebug("Cache hit for location: {Location}", location);
                return _cache[location];
            }

            await _semaphore.WaitAsync();
            try
            {
                // Rate limiting - ensure we don't hit Nominatim too frequently
                var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
                if (timeSinceLastRequest < _rateLimitDelay)
                {
                    var delay = _rateLimitDelay - timeSinceLastRequest;
                    await Task.Delay(delay);
                }

                var coordinates = await GeocodeWithNominatimAsync(location);
                
                // Cache the result (even if null to avoid repeated failed requests)
                _cache[location] = coordinates;
                _lastRequestTime = DateTime.UtcNow;
                
                return coordinates;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<(double? lat, double? lng)> GeocodeWithNominatimAsync(string location)
        {
            try
            {
                // Clean up the location string for better geocoding results
                var searchQuery = CleanLocationString(location);
                
                _logger.LogDebug("Geocoding location: {OriginalLocation} -> {CleanedLocation}", location, searchQuery);

                var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(searchQuery)}&format=json&limit=1&addressdetails=1";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<NominatimResult[]>(jsonContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (results != null && results.Length > 0)
                {
                    var result = results[0];
                    if (double.TryParse(result.Lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lat) && 
                        double.TryParse(result.Lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lng))
                    {
                        _logger.LogInformation("Successfully geocoded '{Location}' to ({Lat}, {Lng})", location, lat, lng);
                        return (lat, lng);
                    }
                }

                // If no result found, try with just the country/last part
                var parts = location.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length > 1)
                {
                    var fallbackQuery = parts[^1]; // Last part (usually country)
                    _logger.LogDebug("Trying fallback geocoding with: {FallbackQuery}", fallbackQuery);
                    
                    var fallbackUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(fallbackQuery)}&format=json&limit=1&addressdetails=1";
                    
                    var fallbackResponse = await _httpClient.GetAsync(fallbackUrl);
                    fallbackResponse.EnsureSuccessStatusCode();
                    
                    var fallbackJsonContent = await fallbackResponse.Content.ReadAsStringAsync();
                    var fallbackResults = JsonSerializer.Deserialize<NominatimResult[]>(fallbackJsonContent, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                    if (fallbackResults != null && fallbackResults.Length > 0)
                    {
                        var result = fallbackResults[0];
                        if (double.TryParse(result.Lat, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lat) && 
                            double.TryParse(result.Lon, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lng))
                        {
                            _logger.LogInformation("Successfully geocoded '{Location}' using fallback to ({Lat}, {Lng})", location, lat, lng);
                            return (lat, lng);
                        }
                    }
                }

                _logger.LogWarning("No geocoding results found for location: {Location}", location);
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error geocoding location: {Location}", location);
                return (null, null);
            }
        }

        private static string CleanLocationString(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return location;

            var cleaned = location
                .Replace("Off ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Near ", "", StringComparison.OrdinalIgnoreCase)
                .Trim()
                .TrimEnd(',');

            // Handle special cases for oceans and seas
            var oceanMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "North Atlantic Ocean", "Atlantic Ocean" },
                { "South Atlantic Ocean", "Atlantic Ocean" },
                { "North Pacific Ocean", "Pacific Ocean" },
                { "South Pacific Ocean", "Pacific Ocean" },
                { "Mediterranean Sea", "Mediterranean" },
                { "Caribbean Sea", "Caribbean" },
                { "Baltic Sea", "Baltic" },
                { "North Sea", "North Sea" },
                { "Black Sea", "Black Sea" },
                { "Red Sea", "Red Sea" },
                { "Arabian Sea", "Arabian Sea" },
                { "Bay of Bengal", "Bay of Bengal" },
                { "Persian Gulf", "Persian Gulf" },
                { "Gulf of Mexico", "Gulf of Mexico" },
                { "English Channel", "English Channel" },
                { "Bering Sea", "Bering Sea" }
            };

            foreach (var (key, value) in oceanMapping)
            {
                if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }

            return cleaned;
        }

        private class NominatimResult
        {
            public string Lat { get; set; } = string.Empty;
            public string Lon { get; set; } = string.Empty;
        }
    }
}
