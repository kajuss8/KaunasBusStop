using KaunasBusStop.Server.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KaunasBusStop.Server.Models
{
    public class Calendar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ServiceId { get; set; }
        public WorkDay? Monday { get; set; }
        public WorkDay? Tuesday { get; set; }
        public WorkDay? Wednesday { get; set; }
        public WorkDay? Thursday { get; set; }
        public WorkDay? Friday { get; set; }
        public WorkDay? Saturday { get; set; }
        public WorkDay? Sunday { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
