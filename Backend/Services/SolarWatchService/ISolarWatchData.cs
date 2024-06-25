using SolarWatch.Model;
using SolarWatch.Model.DTOModel;

namespace SolarWatch.Services.SolarWatchService;

public interface ISolarWatchData
{
    public Task<Sunrise> GetSunrise(City city,SolarDTO solarDto);
    public Task<Sunset> GetSunset(City city, SolarDTO solarDto);
}