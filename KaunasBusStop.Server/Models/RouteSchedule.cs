using KaunasBusStop.Server.Enums;

namespace KaunasBusStop.Server.Models
{
    public class RouteSchedule
    {
        public string? RouteLongName { get; set; }
        public string? ShapeId { get; set; }
        public List<RouteInformation>? RouteInformation { get; set; } = new List<RouteInformation>();
    }

    public class RouteInformation
    {
        public List<string?>? WorkDays { get; set; }
        public List<Stopinfo>? Stopinfos { get; set; } = new List<Stopinfo>();
    }

    public class Stopinfo
    {
        public int StopId { get; set; }
        public float? StopLat { get; set; }
        public float? StopLon { get; set; }
        public string? StopName { get; set; }
        public List<string>? DepartureTime { get; set; }
    }
}
