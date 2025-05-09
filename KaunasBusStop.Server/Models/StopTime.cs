using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class StopTime
    {
        [Key]
        public int Id { get; set; }
        public string? TripId { get; set; }
        public string? ArrivalTime { get; set; }
        public string? DepartureTime { get; set; }
        public int? StopId { get; set; }
        public int? StopSequence { get; set; }
        public int? PickUpType { get; set; }
        public int? DropOffType { get; set; }
    }
}
