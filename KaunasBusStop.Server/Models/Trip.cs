using KaunasBusStop.Server.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class Trip
    {
        public string? RouteId { get; set; }
        public int? ServiceId { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TripId { get; set; }
        public string? TripHeadsign { get; set; }
        public Direction? DirectionId { get; set; }
        public int? BlockId { get; set; }
        public string? ShapeId { get; set; }
        public int? WheelchairAccessible { get; set; }
    }
}
