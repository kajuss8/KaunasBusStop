using KaunasBusStop.Server.Enums;

namespace KaunasBusStop.Server.Models
{
    public class StopSchedule
    {
        public string? StopName { get; set; }
        public List<StopInformation>? StopInformation { get; set; }
    }

    public class StopInformation
    {
        public string? ShapeId { get; set; }
        public string? RouteId { get; set; }
        public string? RouteShorName { get; set; }
        public string? RouteLongName { get; set; }
        public TransportType? RouteType { get; set; }
        public int? RouteSortOrder { get; set; }
        public List<string>? WorkDays { get; set; }
        public List<string> ArrivalTime { get; set; } = new List<string>();
    }
}
