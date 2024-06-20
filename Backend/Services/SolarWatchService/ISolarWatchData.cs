using SolarWatch.Model;

namespace SolarWatch.Services.SolarWatchService;

public interface ISolarWatchData
{
    public Task<Sunrise> GetSunrise(City city, DateTime dateTime);
    public Task<Sunset> GetSunset(City city, DateTime dateTime);
}