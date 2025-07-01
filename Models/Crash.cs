namespace Plane_Crash_Visualization.Models
{
    public class Crash
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? Time { get; set; }
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsGeocoded { get; set; } = false;
        public string? Operator { get; set; }
        public string? Flight { get; set; }
        public string? Route { get; set; }
        public string? AC_Type { get; set; }
        public string? Manufacturer { get; set; }
        public string? AC_Model { get; set; }
        public string? Registration { get; set; }
        public string? Cn_ln { get; set; }
        public int? Aboard { get; set; }
        public int? AboardPassengers { get; set; }
        public int? AboardCrew { get; set; }
        public int? Fatalities { get; set; }
        public int? FatalitiesPassengers { get; set; }
        public int? FatalitiesCrew { get; set; }
        public int? Ground { get; set; }
        public string? Summary { get; set; }
        
    }
}
