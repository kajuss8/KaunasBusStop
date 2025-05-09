using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Repositories;
using KaunasBusStop.Server;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Configuration;
using KaunasBusStop.Server.Models;

namespace KaunasBusStopTest
{
    public class RouteRepositoryTest : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private IRouteRepository _routeRepository;

        public RouteRepositoryTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            _options = new DbContextOptionsBuilder<BusStopDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            Initialize();
        }

        private void Initialize()
        {
            _dbContext = new BusStopDbContext(_options);
            _routeRepository = new RouteRepository(_dbContext, _mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateAllRoutesAsync_ShouldImportRoutes()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test_routes.txt");
            var csvContent = new StringBuilder();
            csvContent.AppendLine("route_id,route_short_name,route_long_name,route_desc,route_type,route_url,route_color,route_text_color,route_sort_order");
            csvContent.AppendLine("1,ShortName1,LongName1,Description1,3,URL1,Color1,TextColor1,1");
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:RoutesFileName"]).Returns("test_routes.txt");

            // Act
            await _routeRepository.CreateAllRoutesAsync();

            // Assert
            var routes = await _dbContext.Routes.ToListAsync();
            Assert.Single(routes);
            Assert.Equal("1", routes[0].RouteId);
            Assert.Equal("ShortName1", routes[0].RouteShorName);
            Assert.Equal("LongName1", routes[0].RouteLongName);
            Assert.Equal("Description1", routes[0].RouteDescription);
            Assert.Equal(TransportType.Bus, routes[0].RouteType);
            Assert.Equal("URL1", routes[0].RouteURL);
            Assert.Equal("Color1", routes[0].RouteColor);
            Assert.Equal("TextColor1", routes[0].RouteText);
            Assert.Equal(1, routes[0].RouteSortOrder);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateAllRoutesAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "non_existent_file.csv";
            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:RoutesFileName"]).Returns(filePath);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _routeRepository.CreateAllRoutesAsync());
        }

        [Fact]
        public async Task GetAllRoutes_ShouldReturnAllRoutes()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route { RouteId = "1", RouteShorName = "ShortName1", RouteLongName = "LongName1", RouteType = TransportType.Bus },
                new Route { RouteId = "2", RouteShorName = "ShortName2", RouteLongName = "LongName2", RouteType = TransportType.Trolleybus }
            };

            await _dbContext.Routes.AddRangeAsync(routes);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _routeRepository.GetAllRoutes();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].RouteId);
            Assert.Equal("2", result[1].RouteId);
        }

        [Fact]
        public async Task GetRoutesByRouteIdsAsync_ShouldReturnMatchingRoutes()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route { RouteId = "1", RouteShorName = "ShortName1", RouteLongName = "LongName1", RouteType = TransportType.Bus },
                new Route { RouteId = "2", RouteShorName = "ShortName2", RouteLongName = "LongName2", RouteType = TransportType.Trolleybus }
            };

            await _dbContext.Routes.AddRangeAsync(routes);
            await _dbContext.SaveChangesAsync();

            var routeIds = new List<string?> { "1", "2" };

            // Act
            var result = await _routeRepository.GetRoutesByRouteIdsAsync(routeIds);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].RouteId);
            Assert.Equal("2", result[1].RouteId);
        }

        [Fact]
        public async Task GetRoutesByRouteIdsAsync_ShouldReturnEmptyList_WhenNoMatchingRouteIds()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route { RouteId = "1", RouteShorName = "ShortName1", RouteLongName = "LongName1", RouteType = TransportType.Bus },
                new Route { RouteId = "2", RouteShorName = "ShortName2", RouteLongName = "LongName2", RouteType = TransportType.Trolleybus }
            };

            await _dbContext.Routes.AddRangeAsync(routes);
            await _dbContext.SaveChangesAsync();

            var routeIds = new List<string?> { "3", "4" };

            // Act
            var result = await _routeRepository.GetRoutesByRouteIdsAsync(routeIds);

            // Assert
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
