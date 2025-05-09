
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<BusStopDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<ICalendarRepository, CalendarRepository>();
            builder.Services.AddTransient<IRouteRepository, RouteRepository>();
            builder.Services.AddTransient<IStopRepository, StopRepository>();
            builder.Services.AddTransient<IStopTimeRepository, StopTimeRepository>();
            builder.Services.AddTransient<ITripRepository, TripRepository>();
            builder.Services.AddTransient<IRouteWorkWeekRepository, RouteWorkWeekRepository>();
            builder.Services.AddTransient<IStopScheduleRepository, StopScheduleRepository>();
            builder.Services.AddTransient<IRouteScheduleRepository, RouteScheduleRepository>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("Access-Control-Allow-Origin");
                });
            });

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
