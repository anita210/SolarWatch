using System.Text.Json;
using SolarWatch.Model;
using SolarWatch.Services;

namespace SolarWatch.Services;

public class GeocodingJsonConverter : IGeocodingJsonConverter
{
    public GeocodingJsonConverter()
    {
        
    }
    public City ParseCityData(string data)
    {
        var json = JsonDocument.Parse(data);

        if (json.RootElement.ValueKind != JsonValueKind.Array || json.RootElement.GetArrayLength() == 0)
        {
            return null;
        }

        var cityData = json.RootElement[0];
        var name = cityData.GetProperty("name").GetString();
        var lat = cityData.GetProperty("lat").GetDouble();
        var lon = cityData.GetProperty("lon").GetDouble();
        string country = cityData.GetProperty("state").GetString();
        string? state = cityData.GetProperty("country").GetString();
        
        var city = new City { Name = name, Latitude = lat, Longitude = lon, Country = state, State = country};

        return city;
    }
}