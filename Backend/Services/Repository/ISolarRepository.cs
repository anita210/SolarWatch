using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ISolarRepository
{
    Task<IEnumerable<Sunrise>> GetAllSunrise();

    Task<IEnumerable<Sunset>> GetAllSunset();
    Task<Sunrise?> GetSunriseById(int id);
    Task Add(Sunrise sunrise);
    Task DeleteSunrise(Sunrise sunrise);
    Task UpdateSunrise(Sunrise oldSunrise, Sunrise newSunrise);
    Task<Sunset?> GetSunsetById(int id);

    Task Add(Sunset sunset);
    Task DeleteSunset(Sunset sunset);
    Task UpdateSunset(Sunset oldSunset, Sunset newSunset);
}