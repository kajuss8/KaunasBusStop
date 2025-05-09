using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Moq;

namespace KaunasBusStopTest
{
    public class RouteScheduleRepositoryTest
    {
        [Fact]
        public async Task GetRouteSchedules_ReturnsCorrectData()
        {
            // Arrange
            var mockRepository = new Mock<IRouteScheduleRepository>();
            var routeId = "123";
            var expectedSchedules = new List<RouteSchedule>
            {
                new RouteSchedule
                {
                    RouteLongName = "Route 1",
                    ShapeId = "Shape1",
                    RouteInformation = new List<RouteInformation>
                    {
                        new RouteInformation
                        {
                            WorkDays = new List<string?> { "Monday", "Tuesday", "Wednesday" },
                            Stopinfos = new List<Stopinfo>
                            {
                                new Stopinfo
                                {
                                    StopId = 1,
                                    StopLat = 54.8985f,
                                    StopLon = 23.9036f,
                                    StopName = "Stop A",
                                    DepartureTime = new List<string> { "08:00", "09:00", "10:00" }
                                },
                                new Stopinfo
                                {
                                    StopId = 2,
                                    StopLat = 54.8990f,
                                    StopLon = 23.9040f,
                                    StopName = "Stop B",
                                    DepartureTime = new List<string> { "08:15", "09:15", "10:15" }
                                }
                            }
                        }
                    }
                },
                new RouteSchedule
                {
                    RouteLongName = "Route 2",
                    ShapeId = "Shape2",
                    RouteInformation = new List<RouteInformation>
                    {
                        new RouteInformation
                        {
                            WorkDays = new List<string?> { "Thursday", "Friday" },
                            Stopinfos = new List<Stopinfo>
                            {
                                new Stopinfo
                                {
                                    StopId = 3,
                                    StopLat = 54.9000f,
                                    StopLon = 23.9050f,
                                    StopName = "Stop C",
                                    DepartureTime = new List<string> { "12:00", "13:00", "14:00" }
                                }
                            }
                        }
                    }
                }
            };

            mockRepository
                .Setup(repo => repo.GetRouteSchedules(routeId))
                .ReturnsAsync(expectedSchedules);

            // Act
            var result = await mockRepository.Object.GetRouteSchedules(routeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSchedules.Count, result.Count);
            Assert.Equal(expectedSchedules[0].RouteLongName, result[0].RouteLongName);
            Assert.Equal(expectedSchedules[0].RouteInformation[0].Stopinfos[0].StopName, result[0].RouteInformation[0].Stopinfos[0].StopName);
            Assert.Equal(expectedSchedules[1].RouteInformation[0].WorkDays, result[1].RouteInformation[0].WorkDays);
        }

        [Fact]
        public async Task GetRouteSchedules_ReturnsEmptyList_WhenNoSchedulesFound()
        {
            // Arrange
            var mockRepository = new Mock<IRouteScheduleRepository>();
            var routeId = "456";

            mockRepository
                .Setup(repo => repo.GetRouteSchedules(routeId))
                .ReturnsAsync(new List<RouteSchedule>());

            // Act
            var result = await mockRepository.Object.GetRouteSchedules(routeId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
