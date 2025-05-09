using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class Stop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StopId { get; set; }
        public string? StopCode { get; set; }
        public string? StopName { get; set; }
        public string? StopDesc { get; set; }
        public float? StopLat { get; set; }
        public float? StopLon { get; set; }
        public string? StopURL { get; set; }
        public int? LocationType { get; set; }
        public int? ParentStation { get; set; }
    }
}
