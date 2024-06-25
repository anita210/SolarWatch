using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatch.Services;

public interface IGeocodingJsonConverter
{
    public City ParseCityData(string data);

}