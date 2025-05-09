using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using KaunasBusStop.Server.Repositories;
using KaunasBusStop.Server;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace KaunasBusStopTest
{
    public class StopScheduleRepositoryTest : IDisposable
    {
        private readonly Mock<IStopRepository> _mockStopRepository;
        private readonly Mock<IStopTimeRepository> _mockStopTimeRepository;
        private readonly Mock<ITripRepository> _mockTripRepository;
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<ICalendarRepository> _mockCalendarRepository;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private IStopScheduleRepository _stopScheduleRepository;

        public StopScheduleRepositoryTest()
        {
            _mockStopRepository = new Mock<IStopRepository>();
            _mockStopTimeRepository = new Mock<IStopTimeRepository>();
            _mockTripRepository = new Mock<ITripRepository>();
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockCalendarRepository = new Mock<ICalendarRepository>();

            _options = new DbContextOptionsBuilder<BusStopDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            Initialize();
        }

        private void Initialize()
        {
            _dbContext = new BusStopDbContext(_options);
            _stopScheduleRepository = new StopScheduleRepository(
                _dbContext,
                _mockStopRepository.Object,
                _mockStopTimeRepository.Object,
                _mockTripRepository.Object,
                _mockRouteRepository.Object,
                _mockCalendarRepository.Object
            );
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldReturnStopSchedule()
        {
            // Arrange
            var stopId = 1;
            var stopName = "Stop1";
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = stopId, TripId = "1", ArrivalTime = "12:00" },
                new StopTime { StopId = stopId, TripId = "2", ArrivalTime = "12:10" }
            };
            var trips = new List<Trip>
            {
                new Trip { TripId = "1", RouteId = "1", ServiceId = 1, ShapeId = "Shape1" },
                new Trip { TripId = "2", RouteId = "2", ServiceId = 2, ShapeId = "Shape2" }
            };
            var routes = new List<Route>
            {
                new Route { RouteId = "1", RouteShorName = "ShortName1", RouteLongName = "LongName1", RouteType = TransportType.Bus, RouteSortOrder = 1 },
                new Route { RouteId = "2", RouteShorName = "ShortName2", RouteLongName = "LongName2", RouteType = TransportType.Trolleybus, RouteSortOrder = 2 }
            };
            var calendars = new List<Calendar>
            {
                new Calendar { ServiceId = 1, Monday = WorkDay.ServiceAvailable },
                new Calendar { ServiceId = 2, Monday = WorkDay.ServiceAvailable }
            };

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync(stopName);
            _mockStopTimeRepository.Setup(repo => repo.GetStopTimesByStopIdAsync(stopId)).ReturnsAsync(stopTimes);
            _mockTripRepository.Setup(repo => repo.GetTripsByTripIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(trips);
            _mockRouteRepository.Setup(repo => repo.GetRoutesByRouteIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(routes);
            _mockCalendarRepository.Setup(repo => repo.GetCalendarsByServiceIdsAsync(It.IsAny<List<int?>>())).ReturnsAsync(calendars);

            // Act
            var result = await _stopScheduleRepository.GetStopScheduleByIdAsync(stopId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(stopName, result[0].StopName);
            Assert.Equal(2, result[0].StopInformation.Count);
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldThrowException_WhenStopNotFound()
        {
            // Arrange
            var stopId = 1;

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync((string)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopScheduleRepository.GetStopScheduleByIdAsync(stopId));
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldThrowException_WhenNoStopTimesFound()
        {
            // Arrange
            var stopId = 1;
            var stopName = "Stop1";

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync(stopName);
            _mockStopTimeRepository.Setup(repo => repo.GetStopTimesByStopIdAsync(stopId)).ReturnsAsync(new List<StopTime>());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopScheduleRepository.GetStopScheduleByIdAsync(stopId));
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldThrowException_WhenNoTripsFound()
        {
            // Arrange
            var stopId = 1;
            var stopName = "Stop1";
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = stopId, TripId = "1", ArrivalTime = "12:00" }
            };

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync(stopName);
            _mockStopTimeRepository.Setup(repo => repo.GetStopTimesByStopIdAsync(stopId)).ReturnsAsync(stopTimes);
            _mockTripRepository.Setup(repo => repo.GetTripsByTripIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(new List<Trip>());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopScheduleRepository.GetStopScheduleByIdAsync(stopId));
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldThrowException_WhenNoRoutesFound()
        {
            // Arrange
            var stopId = 1;
            var stopName = "Stop1";
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = stopId, TripId = "1", ArrivalTime = "12:00" }
            };
            var trips = new List<Trip>
            {
                new Trip { TripId = "1", RouteId = "1", ServiceId = 1, ShapeId = "Shape1" }
            };

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync(stopName);
            _mockStopTimeRepository.Setup(repo => repo.GetStopTimesByStopIdAsync(stopId)).ReturnsAsync(stopTimes);
            _mockTripRepository.Setup(repo => repo.GetTripsByTripIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(trips);
            _mockRouteRepository.Setup(repo => repo.GetRoutesByRouteIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(new List<Route>());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopScheduleRepository.GetStopScheduleByIdAsync(stopId));
        }

        [Fact]
        public async Task GetStopScheduleByIdAsync_ShouldThrowException_WhenNoCalendarsFound()
        {
            // Arrange
            var stopId = 1;
            var stopName = "Stop1";
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = stopId, TripId = "1", ArrivalTime = "12:00" }
            };
            var trips = new List<Trip>
            {
                new Trip { TripId = "1", RouteId = "1", ServiceId = 1, ShapeId = "Shape1" }
            };
            var routes = new List<Route>
            {
                new Route { RouteId = "1", RouteShorName = "ShortName1", RouteLongName = "LongName1", RouteType = TransportType.Bus, RouteSortOrder = 1 }
            };
            var calendars = new List<Calendar>
            {
                new Calendar { ServiceId = 1, Monday = WorkDay.ServiceAvailable }
            };

            _mockStopRepository.Setup(repo => repo.GetStopNameByIdAsync(stopId)).ReturnsAsync(stopName);
            _mockStopTimeRepository.Setup(repo => repo.GetStopTimesByStopIdAsync(stopId)).ReturnsAsync(stopTimes);
            _mockTripRepository.Setup(repo => repo.GetTripsByTripIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(trips);
            _mockRouteRepository.Setup(repo => repo.GetRoutesByRouteIdsAsync(It.IsAny<List<string?>>())).ReturnsAsync(routes);
            _mockCalendarRepository.Setup(repo => repo.GetCalendarsByServiceIdsAsync(It.IsAny<List<int?>>())).ReturnsAsync(calendars);

            // Act
            var result = await _stopScheduleRepository.GetStopScheduleByIdAsync(stopId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(stopName, result[0].StopName);
            Assert.Equal(1, result[0].StopInformation.Count);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
