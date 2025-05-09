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
    public class StopRepositoryTest : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private IStopRepository _stopRepository;

        public StopRepositoryTest()
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
            _stopRepository = new StopRepository(_dbContext, _mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateAllStopsAsync_ShouldImportStops()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test_stops.txt");
            var csvContent = new StringBuilder();
            csvContent.AppendLine("stop_id,stop_code,stop_name,stop_desc,stop_lat,stop_lon,stop_url,location_type,parent_station");
            csvContent.AppendLine("1,Code1,Name1,Desc1,1.1,2.2,URL1,1,1");
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:StopsFileName"]).Returns("test_stops.txt");

            // Act
            await _stopRepository.CreateAllStopsAsync();

            // Assert
            var stops = await _dbContext.Stops.ToListAsync();
            Assert.Single(stops);
            Assert.Equal(1, stops[0].StopId);
            Assert.Equal("Code1", stops[0].StopCode);
            Assert.Equal("Name1", stops[0].StopName);
            Assert.Equal("Desc1", stops[0].StopDesc);
            Assert.Equal(1.1f, stops[0].StopLat);
            Assert.Equal(2.2f, stops[0].StopLon);
            Assert.Equal("URL1", stops[0].StopURL);
            Assert.Equal(1, stops[0].LocationType);
            Assert.Equal(1, stops[0].ParentStation);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateAllStopsAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "non_existent_file.csv";
            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:StopsFileName"]).Returns(filePath);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _stopRepository.CreateAllStopsAsync());
        }

        [Fact]
        public async Task GetAllStopIds_ShouldReturnAllStopIds()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { StopId = 1, StopName = "Stop1" },
                new Stop { StopId = 2, StopName = "Stop2" }
            };

            await _dbContext.Stops.AddRangeAsync(stops);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _stopRepository.GetAllStopIds();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(2, result);
        }

        [Fact]
        public async Task GetAllStopIds_ShouldReturnEmptyList_WhenNoStopsExist()
        {
            // Arrange

            // Act
            var result = await _stopRepository.GetAllStopIds();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStopNameByIdAsync_ShouldReturnStopName()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { StopId = 1, StopName = "Stop1" },
                new Stop { StopId = 2, StopName = "Stop2" }
            };

            await _dbContext.Stops.AddRangeAsync(stops);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _stopRepository.GetStopNameByIdAsync(1);

            // Assert
            Assert.Equal("Stop1", result);
        }

        [Fact]
        public async Task GetStopNameByIdAsync_ShouldReturnNull_WhenStopIdNotFound()
        {
            // Arrange
            var stops = new List<Stop>
            {
                new Stop { StopId = 1, StopName = "Stop1" },
                new Stop { StopId = 2, StopName = "Stop2" }
            };

            await _dbContext.Stops.AddRangeAsync(stops);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _stopRepository.GetStopNameByIdAsync(3);

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
