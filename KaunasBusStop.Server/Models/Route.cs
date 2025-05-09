using KaunasBusStop.Server.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class Route
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RouteId { get; set; }
        public string? RouteShorName { get; set; }
        public string? RouteLongName { get; set; }
        public string? RouteDescription { get; set; }
        public TransportType? RouteType { get; set; }
        public string? RouteURL { get; set; }
        public string? RouteColor { get; set; }
        public string? RouteText { get; set; }
        public int? RouteSortOrder { get; set; }
    }
}
