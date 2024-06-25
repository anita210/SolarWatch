using System.Net;
using System.Text.Json;
using SolarWatch.Controllers;
using SolarWatch.Model;
using SolarWatch.Model.DTOModel;

namespace SolarWatch.Services;

public class GeocodingCity : IGeocodingCity
{
    private readonly ILogger<GeocodingCity> _logger;

    private readonly IGeocodingJsonConverter _geocodingJsonConverter;

    public GeocodingCity(ILogger<GeocodingCity> logger, IGeocodingJsonConverter geocodingJsonConverter)
    {
        _logger = logger;
        _geocodingJsonConverter = geocodingJsonConverter;
    }

    public async Task<City> GetCityLongAndLatByName(GeocodeDTO cityInput)
    {
        try
        {
            var data = await GetGeocodeDataString(cityInput);

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogError("Failed to retrieve geocode data for city '{CityName}'.", cityInput.cityName);
                return null;
            }

            var json = JsonDocument.Parse(data);
            
            var city = _geocodingJsonConverter.ParseCityData(data);

            if (city == null)
            {
                _logger.LogError(
                    $"No city data found for the given name. Possibly wrong city name or empty response.");
            }

            return city;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching geocode data for city '{CityName}'.", cityInput.cityName);
            return null;
        }
    }

    private async Task<string> GetGeocodeDataString(GeocodeDTO cityInput)
    {
        try
        {
            var url = GetUrlFromCityData(cityInput);

            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("Insufficient input data!");
                return null;
            }

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


    private string GetUrlFromCityData(GeocodeDTO cityInput)
    {
        //NEEDS TO BE HIDDEN
        var apiKey = "a302a60c656a3f0ecff3eb4ade4e6c4e";

        if (cityInput.stateCode == null && cityInput is { countryCode: not null, cityName: not null })
        {
            return $"http://api.openweathermap.org/geo/1.0/direct?q={cityInput.cityName.ToLower()},{cityInput.countryCode.ToLower()}&limit=1&appid={apiKey}";
        }
        else if (cityInput.stateCode == null && cityInput is { countryCode: not null, cityName: not null })
        {
            return $"http://api.openweathermap.org/geo/1.0/direct?q={cityInput.cityName.ToLower()},{cityInput.stateCode.ToLower()},{cityInput.countryCode.ToLower()}&limit=1&appid={apiKey}";
        }

        return null;
    }
}