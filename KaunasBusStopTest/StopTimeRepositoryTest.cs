using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using KaunasBusStop.Server.Repositories;
using KaunasBusStop.Server;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Configuration;

namespace KaunasBusStopTest
{
    public class StopTimeRepositoryTest : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private IStopTimeRepository _stopTimeRepository;

        public StopTimeRepositoryTest()
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
            _stopTimeRepository = new StopTimeRepository(_dbContext, _mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateAllStopTimesAsync_ShouldImportStopTimes()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test_stop_times.txt");
            var csvContent = new StringBuilder();
            csvContent.AppendLine("trip_id,arrival_time,departure_time,stop_id,stop_sequence,pickup_type,drop_off_type");
            csvContent.AppendLine("1,12:00:00,12:05:00,1,1,0,0");
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:StopTimesFileName"]).Returns("test_stop_times.txt");

            // Act
            await _stopTimeRepository.CreateAllStopTimesAsync();

            // Assert
            var stopTimes = await _dbContext.StopTimes.ToListAsync();
            Assert.Single(stopTimes);
            Assert.Equal("1", stopTimes[0].TripId);
            Assert.Equal("12:00", stopTimes[0].ArrivalTime);
            Assert.Equal("12:05", stopTimes[0].DepartureTime);
            Assert.Equal(1, stopTimes[0].StopId);
            Assert.Equal(1, stopTimes[0].StopSequence);
            Assert.Equal(0, stopTimes[0].PickUpType);
            Assert.Equal(0, stopTimes[0].DropOffType);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateAllStopTimesAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "non_existent_file.csv";
            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:StopTimesFileName"]).Returns(filePath);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopTimeRepository.CreateAllStopTimesAsync());
        }

        [Fact]
        public async Task GetAllStopTimesByStopid_ShouldReturnStopTimes()
        {
            // Arrange
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = 1, TripId = "1", ArrivalTime = "12:00", DepartureTime = "12:05" },
                new StopTime { StopId = 1, TripId = "2", ArrivalTime = "12:10", DepartureTime = "12:15" }
            };

            await _dbContext.StopTimes.AddRangeAsync(stopTimes);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _stopTimeRepository.GetAllStopTimesByStopid(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].TripId);
            Assert.Equal("2", result[1].TripId);
        }

        [Fact]
        public async Task GetAllStopTimesByStopid_ShouldThrowException_WhenNoStopTimesFound()
        {
            // Arrange
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopTimeRepository.GetAllStopTimesByStopid(1));
        }

        [Fact]
        public async Task GetStopTimesByStopIdAsync_ShouldReturnStopTimes()
        {
            // Arrange
            var stopTimes = new List<StopTime>
            {
                new StopTime { StopId = 1, TripId = "1", ArrivalTime = "12:00", DepartureTime = "12:05" },
                new StopTime { StopId = 1, TripId = "2", ArrivalTime = "12:10", DepartureTime = "12:15" }
            };

            await _dbContext.StopTimes.AddRangeAsync(stopTimes);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _stopTimeRepository.GetStopTimesByStopIdAsync(1);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].TripId);
            Assert.Equal("2", result[1].TripId);
        }

        [Fact]
        public async Task GetStopTimesByStopIdAsync_ShouldReturnEmptyList_WhenNoStopTimesFound()
        {
            // Arrange

            // Act
            var result = await _stopTimeRepository.GetStopTimesByStopIdAsync(1);

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
