using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;

namespace SolarWatch.Services.SolarWatchService;

public class SolarWatchData : ISolarWatchData
{
    private readonly ILogger<SolarWatchData> _logger;

    public SolarWatchData(ILogger<SolarWatchData> logger)
    {
        _logger = logger;
    }

    public async Task<Sunrise> GetSunrise(City city, DateTime dateTime)
    {
        // Solar data
        var solarData = await DownloadCityData(city, dateTime);

        if (string.IsNullOrEmpty(solarData))
        {
            throw new Exception("Solar data is empty or null.");
        }

        JsonDocument solarJson = JsonDocument.Parse(solarData);

        JsonElement results = solarJson.RootElement.GetProperty("results");

        string sunriseTimeString = results.GetProperty("sunrise").GetString();

        
        if (!DateTime.TryParseExact(sunriseTimeString, "h:mm:ss tt", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime sunriseTime))
        {
            throw new Exception("Failed to parse sunrise time from the JSON data.");
        }

        var sunrise = new Sunrise
        {
            City = city,
            SunriseTime = sunriseTime
        };
        

        return sunrise;
    }

    public async Task<Sunset> GetSunset(City city, DateTime dateTime)
    {
        try
        {
            // Solar data
            var solarData = await DownloadCityData(city, dateTime);

            if (string.IsNullOrEmpty(solarData))
            {
                _logger.LogError("Solar data is empty or null.");
                return null;
            }

            JsonDocument solarJson = JsonDocument.Parse(solarData);

            // Assuming the structure is known and reliable, directly access properties
            var results = solarJson.RootElement.GetProperty("results");
            var sunsetTimeString = results.GetProperty("sunset").GetString();

            // Parse sunset time
            if (!DateTime.TryParseExact(sunsetTimeString, "h:mm:ss tt", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime sunsetTime))
            {
                _logger.LogError("Failed to parse sunset time from the JSON data. Ensure sunset time is in the correct format (h:mm:ss tt).");
                return null;
            }

            var sunset = new Sunset
            {
                City = city,
                SunsetTime = sunsetTime
            };
            

            return sunset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting sunset data for city '{CityName}' at date '{DateTime}'.", city.Name, dateTime);
            return null;
        }
    }

    private async Task<string> DownloadCityData(City city, DateTime dateTime)
    {
        try
        {
            var url = CreateSolarUrlForCity(city, dateTime);
            _logger.LogInformation($"Solar url: {url}");

            using var client = new HttpClient();
            _logger.LogInformation("Calling SolarWatch API with url: {url}", url);

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode(); // Ensure the HTTP response is successful

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "An HTTP error occurred while calling the SolarWatch API.");
            return null; // or throw an exception if you prefer
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while downloading data for city '{CityName}' at date '{DateTime}'.", city.Name, dateTime);
            throw; // rethrow the exception to propagate it to the caller
        }
    }

    private string CreateSolarUrlForCity(City city, DateTime dateTime)
    {
        if (city == null)
        {
            _logger.LogError("Cannot find city to create url with!");
            return null;
        }

        string formattedDate = dateTime.ToString("yyyy-MM-dd");

        string url =
            $"https://api.sunrise-sunset.org/json?lat={city.Latitude.ToString()}&lng={city.Longitude.ToString()}&date={formattedDate}";

        return url;
    }
}