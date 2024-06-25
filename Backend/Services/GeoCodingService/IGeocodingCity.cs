using SolarWatch.Model;
using SolarWatch.Model.DTOModel;

namespace SolarWatch.Services;

public interface IGeocodingCity
{
    
    public Task<City> GetCityLongAndLatByName(GeocodeDTO cityData);
}