using System;
using System.Collections.Generic;
using System.IO.Pipelines;
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
using Microsoft.Extensions.Configuration;

namespace KaunasBusStopTest
{
    public class TripRepositoryTest : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private ITripRepository _tripRepository;

        public TripRepositoryTest()
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
            _tripRepository = new TripRepository(_dbContext, _mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateAllTripsAsync_ShouldImportTrips()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test_trips.txt");
            var csvContent = new StringBuilder();
            csvContent.AppendLine("route_id,service_id,trip_id,trip_headsign,direction_id,block_id,shape_id,wheelchair_accessible");
            csvContent.AppendLine("1,1,1,Headsign1,0,1,Shape1,1");
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:TripsFileName"]).Returns("test_trips.txt");

            try
            {
                // Act
                await _tripRepository.CreateAllTripsAsync();

                // Assert
                var trips = await _dbContext.Trips.ToListAsync();
                Assert.Single(trips);
                Assert.Equal("1", trips[0].RouteId);
                Assert.Equal(1, trips[0].ServiceId);
                Assert.Equal("1", trips[0].TripId);
                Assert.Equal("Headsign1", trips[0].TripHeadsign);
                Assert.Equal(Direction.Outbound, trips[0].DirectionId);
                Assert.Equal(1, trips[0].BlockId);
                Assert.Equal("Shape1", trips[0].ShapeId);
                Assert.Equal(1, trips[0].WheelchairAccessible);
            }
            finally
            {
                // Cleanup
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public async Task CreateAllTripsAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "non_existent_file.csv";
            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:TripsFileName"]).Returns(filePath);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _tripRepository.CreateAllTripsAsync());
        }

        [Fact]
        public async Task GetTripsByTripIdsAsync_ShouldReturnTrips()
        {
            // Arrange
            var trips = new List<Trip>
            {
                new Trip { TripId = "1", RouteId = "1", ServiceId = 1, ShapeId = "Shape1" },
                new Trip { TripId = "2", RouteId = "2", ServiceId = 2, ShapeId = "Shape2" }
            };

            await _dbContext.Trips.AddRangeAsync(trips);
            await _dbContext.SaveChangesAsync();

            var tripIds = new List<string?> { "1", "2" };

            // Act
            var result = await _tripRepository.GetTripsByTripIdsAsync(tripIds);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.TripId == "1");
            Assert.Contains(result, t => t.TripId == "2");
        }

        [Fact]
        public async Task GetTripsByTripIdsAsync_ShouldReturnEmptyList_WhenNoMatchingTripIds()
        {
            // Arrange
            var trips = new List<Trip>
            {
                new Trip { TripId = "1", RouteId = "1", ServiceId = 1, ShapeId = "Shape1" },
                new Trip { TripId = "2", RouteId = "2", ServiceId = 2, ShapeId = "Shape2" }
            };

            await _dbContext.Trips.AddRangeAsync(trips);
            await _dbContext.SaveChangesAsync();

            var tripIds = new List<string?> { "3", "4" };

            // Act
            var result = await _tripRepository.GetTripsByTripIdsAsync(tripIds);

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
