using System;

namespace Plane_Crash_Visualization.Services
{
    public interface IContinentService
    {
        string GetContinentFromCoordinates(double latitude, double longitude);
    }

    public class ContinentService : IContinentService
    {
        public string GetContinentFromCoordinates(double latitude, double longitude)
        {
            // Simple continent determination based on coordinate ranges
            // This is a basic approximation - for more accuracy, you could use a geospatial library
            
            // North America
            if ((latitude >= 15 && latitude <= 83) && (longitude >= -168 && longitude <= -50))
            {
                return "North America";
            }
            
            // South America
            if ((latitude >= -60 && latitude <= 15) && (longitude >= -82 && longitude <= -30))
            {
                return "South America";
            }
            
            // Europe
            if ((latitude >= 35 && latitude <= 75) && (longitude >= -25 && longitude <= 40))
            {
                return "Europe";
            }
            
            // Africa
            if ((latitude >= -35 && latitude <= 38) && (longitude >= -20 && longitude <= 55))
            {
                return "Africa";
            }
            
            // Asia
            if ((latitude >= -10 && latitude <= 81) && (longitude >= 40 && longitude <= 180))
            {
                return "Asia";
            }
            
            // Oceania
            if ((latitude >= -50 && latitude <= 0) && (longitude >= 110 && longitude <= 180))
            {
                return "Oceania";
            }
            
            // Antarctica
            if (latitude < -60)
            {
                return "Antarctica";
            }
            
            // Handle edge cases and overlaps
            if (longitude >= -180 && longitude <= -168)
            {
                return latitude > 0 ? "Asia" : "Oceania";
            }
            
            // Default fallback
            return "Unknown";
        }
    }
}
