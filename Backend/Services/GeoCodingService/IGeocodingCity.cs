using SolarWatch.Model;

namespace SolarWatch.Services;

public interface IGeocodingCity
{
    
    public Task<City> GetCityLongAndLatByName(string cityName);
}