using System.Net;
using System.Text.Json;
using SolarWatch.Controllers;
using SolarWatch.Model;

namespace SolarWatch.Services;

public class GeocodingCity : IGeocodingCity
{
    private readonly ILogger<GeocodingCity> _logger;

    public GeocodingCity(ILogger<GeocodingCity> logger)
    {
        _logger = logger;
    }

    public async Task<City> GetCityLongAndLatByName(string cityName)
    {
        try
        {
            var data = await GetGeocodeDataString(cityName);

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogError("Failed to retrieve geocode data for city '{CityName}'.", cityName);
                return null;
            }

            var json = JsonDocument.Parse(data);

            if (json.RootElement.ValueKind != JsonValueKind.Array || json.RootElement.GetArrayLength() == 0)
            {
                _logger.LogError(
                    $"No city data found for the given name '{cityName}'. Possibly wrong city name or empty response.",
                    cityName);
                return null;
            }

            var cityData = json.RootElement[0];
            var name = cityData.GetProperty("name").GetString();
            var lat = cityData.GetProperty("lat").GetDouble();
            var lon = cityData.GetProperty("lon").GetDouble();
            string? country = null;
            string? state = null;
            if (cityData.TryGetProperty("country", out var countryProperty) && countryProperty.ValueKind == JsonValueKind.String)
            {
                country = countryProperty.GetString();
            }
            
            if (cityData.TryGetProperty("state", out var stateProperty) && stateProperty.ValueKind == JsonValueKind.String)
            {
                state = stateProperty.GetString();
            }
            

            var city = new City { Name = name, Latitude = lat, Longitude = lon, Country = country, State = state};
            return city;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching geocode data for city '{CityName}'.", cityName);
            return null;
        }
    }

    private async Task<string> GetGeocodeDataString(string cityName)
    {
        try
        {
            var apiKey = "a302a60c656a3f0ecff3eb4ade4e6c4e";
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName.ToLower()}&limit=1&appid={apiKey}";

            using var client = new HttpClient();

            var response = await client.GetAsync(url);

            _logger.LogInformation("Calling Geocoding API with url: {url}", url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve geocode data. Status code: {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An HTTP error occurred while calling the Geocoding API.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calling the Geocoding API.");
            throw;
        }
    }
}