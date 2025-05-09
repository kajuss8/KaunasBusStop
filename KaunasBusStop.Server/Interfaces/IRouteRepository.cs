namespace KaunasBusStop.Server.Interfaces
{
    public interface IRouteRepository
    {
        Task CreateAllRoutesAsync();
        Task<List<Models.Route>> GetAllRoutes();
        Task<List<Models.Route>> GetRoutesByRouteIdsAsync(List<string?> routeIds);
    }
}
