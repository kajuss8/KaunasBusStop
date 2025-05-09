using System.Reflection;
using KaunasBusStop.Server.Enums;

namespace KaunasBusStop.Server.Interfaces
{
    public interface ICalendarRepository
    {
        Task CreateAllCalendarsAsync();
        Task<List<Models.Calendar>> GetCalendarsByServiceIdsAsync(List<int?> serviceIds);
        List<string> ConvertCalendarDaysToLetters(List<WorkDay> workDays);
    }
}
