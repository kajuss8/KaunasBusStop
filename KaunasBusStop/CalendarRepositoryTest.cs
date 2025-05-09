using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Repositories;
using KaunasBusStop.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using KaunasBusStop.Server.Models;

namespace KaunasBusStopTest
{
    public class CalendarRepositoryTest : IDisposable
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly DbContextOptions<BusStopDbContext> _options;
        private BusStopDbContext _dbContext;
        private ICalendarRepository _calendarRepository;

        public CalendarRepositoryTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            _options = new DbContextOptionsBuilder<BusStopDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            Initialize();
        }

        public void Initialize()
        {
            _dbContext = new BusStopDbContext(_options);
            _calendarRepository = new CalendarRepository(_mockConfiguration.Object, _dbContext);
        }

        [Fact]
        public async Task CreateAllCalendarsAsync_ShouldImportCalendars()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test_calendars.txt");
            var csvContent = new StringBuilder();
            csvContent.AppendLine("service_id,monday,tuesday,wednesday,thursday,friday,saturday,sunday,start_date,end_date");
            csvContent.AppendLine("1,1,1,1,1,1,0,0,20230101,20231231");
            await File.WriteAllTextAsync(filePath, csvContent.ToString());

            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:CalendarsFileName"]).Returns("test_calendars.txt");

            // Act
            await _calendarRepository.CreateAllCalendarsAsync();

            // Assert
            var calendars = await _dbContext.Calendars.ToListAsync();
            Assert.Single(calendars);
            Assert.Equal(1, calendars[0].ServiceId);
            Assert.Equal(WorkDay.ServiceAvailable, calendars[0].Monday);
            Assert.Equal(WorkDay.ServiceAvailable, calendars[0].Tuesday);
            Assert.Equal(WorkDay.ServiceAvailable, calendars[0].Wednesday);
            Assert.Equal(WorkDay.ServiceAvailable, calendars[0].Thursday);
            Assert.Equal(WorkDay.ServiceAvailable, calendars[0].Friday);
            Assert.Equal(WorkDay.ServiceNotAvailable, calendars[0].Saturday);
            Assert.Equal(WorkDay.ServiceNotAvailable, calendars[0].Sunday);
            Assert.Equal(new DateOnly(2023, 1, 1), calendars[0].StartDate);
            Assert.Equal(new DateOnly(2023, 12, 31), calendars[0].EndDate);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateAllCalendarsAsync_ShouldThrowException_WhenFileNotFound()
        {
            // Arrange
            var filePath = "non_existent_file.txt";
            _mockConfiguration.Setup(c => c["GTFS:DestinationFolder"]).Returns(Directory.GetCurrentDirectory());
            _mockConfiguration.Setup(c => c["GTFS:CalendarsFileName"]).Returns(filePath);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _calendarRepository.CreateAllCalendarsAsync());
        }

        [Fact]
        public async Task GetCalendarsByServiceIdsAsync_ShouldReturnCalendars()
        {
            // Arrange
            var serviceIds = new List<int?> { 1, 2, 3 };
            var calendars = new List<Calendar>
            {
                new Calendar { ServiceId = 1 },
                new Calendar { ServiceId = 2 },
                new Calendar { ServiceId = 3 }
            };

            await _dbContext.Calendars.AddRangeAsync(calendars);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _calendarRepository.GetCalendarsByServiceIdsAsync(serviceIds);

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetCalendarsByServiceIdsAsync_ShouldReturnEmptyList_WhenNoMatchingServiceIds()
        {
            // Arrange
            var serviceIds = new List<int?> { 4, 5, 6 };
            var calendars = new List<Calendar>
            {
                new Calendar { ServiceId = 1 },
                new Calendar { ServiceId = 2 },
                new Calendar { ServiceId = 3 }
            };

            await _dbContext.Calendars.AddRangeAsync(calendars);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _calendarRepository.GetCalendarsByServiceIdsAsync(serviceIds);

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
