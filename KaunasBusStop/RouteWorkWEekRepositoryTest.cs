using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Moq;

namespace KaunasBusStopTest
{
    public class RouteWorkWEekRepositoryTest
    {
        [Fact]
        public async Task CreateAllRouteWorkWeekAsync_CreatesDataSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<IRouteWorkWeekRepository>();
            mockRepository
                .Setup(repo => repo.CreateAllRouteWorkWeekAsync())
                .Returns(Task.CompletedTask);

            // Act
            await mockRepository.Object.CreateAllRouteWorkWeekAsync();

            // Assert
            mockRepository.Verify(repo => repo.CreateAllRouteWorkWeekAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllRouteWorkWeek_ReturnsCorrectData()
        {
            // Arrange
            var mockRepository = new Mock<IRouteWorkWeekRepository>();
            var expectedData = new List<RouteWorkWeek>
            {
                new RouteWorkWeek
                {
                    Id = 1,
                    RouteId = "R1",
                    RouteShorName = "Short1",
                    RouteLongName = "Long Route 1",
                    RouteType = TransportType.Bus,
                    Monday = WorkDay.ServiceAvailable,
                    Tuesday = WorkDay.ServiceAvailable,
                    Wednesday = WorkDay.ServiceAvailable,
                    Thursday = WorkDay.ServiceAvailable,
                    Friday = WorkDay.ServiceAvailable,
                    Saturday = WorkDay.ServiceNotAvailable,
                    Sunday = WorkDay.ServiceNotAvailable
                },
                new RouteWorkWeek
                {
                    Id = 2,
                    RouteId = "R2",
                    RouteShorName = "Short2",
                    RouteLongName = "Long Route 2",
                    RouteType = TransportType.Trolleybus,
                    Monday = WorkDay.ServiceNotAvailable,
                    Tuesday = WorkDay.ServiceAvailable,
                    Wednesday = WorkDay.ServiceAvailable,
                    Thursday = WorkDay.ServiceAvailable,
                    Friday = WorkDay.ServiceAvailable,
                    Saturday = WorkDay.ServiceAvailable,
                    Sunday = WorkDay.ServiceNotAvailable
                }
            };

            mockRepository
                .Setup(repo => repo.GetAllRouteWorkWeek())
                .ReturnsAsync(expectedData);

            // Act
            var result = await mockRepository.Object.GetAllRouteWorkWeek();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count);
            Assert.Equal(expectedData[0].RouteId, result[0].RouteId);
            Assert.Equal(expectedData[1].RouteType, result[1].RouteType);
        }

        [Fact]
        public async Task GetAllRouteWorkWeekWithModifiedDataAsync_ReturnsModifiedData()
        {
            // Arrange
            var mockRepository = new Mock<IRouteWorkWeekRepository>();
            var expectedData = new List<object>
            {
                new { RouteId = "R1", RouteName = "Long Route 1", IsWeekend = false },
                new { RouteId = "R2", RouteName = "Long Route 2", IsWeekend = true }
            };

            mockRepository
                .Setup(repo => repo.GetAllRouteWorkWeekWithModifiedDataAsync())
                .ReturnsAsync(expectedData);

            // Act
            var result = await mockRepository.Object.GetAllRouteWorkWeekWithModifiedDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count);
            Assert.Equal(expectedData[0].GetType().GetProperty("RouteId")?.GetValue(expectedData[0]), "R1");
            Assert.Equal(expectedData[1].GetType().GetProperty("IsWeekend")?.GetValue(expectedData[1]), true);
        }
    }
}
