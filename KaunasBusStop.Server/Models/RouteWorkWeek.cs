using KaunasBusStop.Server.Enums;
using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class RouteWorkWeek
    {
        [Key]
        public int Id { get; set; }
        public string RouteId { get; set; }
        public string? RouteShorName { get; set; }
        public string? RouteLongName { get; set; }
        public TransportType? RouteType { get; set; }
        public int? RouteSortOrder { get; set; }
        public WorkDay? Monday { get; set; }
        public WorkDay? Tuesday { get; set; }
        public WorkDay? Wednesday { get; set; }
        public WorkDay? Thursday { get; set; }
        public WorkDay? Friday { get; set; }
        public WorkDay? Saturday { get; set; }
        public WorkDay? Sunday { get; set; }
    }
}
