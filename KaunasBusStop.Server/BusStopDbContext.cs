using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server
{
    public class BusStopDbContext : DbContext
    {
        public BusStopDbContext(DbContextOptions<BusStopDbContext> options) : base(options) { }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<StopTime> StopTimes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<RouteWorkWeek> RoutesWorkWeeks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
