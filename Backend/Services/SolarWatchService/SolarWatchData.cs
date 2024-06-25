using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Model;
using SolarWatch.Model.DTOModel;

namespace SolarWatch.Services.SolarWatchService;

public class SolarWatchData : ISolarWatchData
{
    private readonly ILogger<SolarWatchData> _logger;

    private readonly ISolarJsonConverter _jsonConverter;

    public SolarWatchData(ILogger<SolarWatchData> logger, ISolarJsonConverter jsonConverter)
    {
        _logger = logger;
        _jsonConverter = jsonConverter;
    }

    public async Task<Sunrise> GetSunrise(City city, SolarDTO solarInput)
    {
        // GET SOLAR DATA
        var solarData = await DownloadCityData(solarInput);

        if (string.IsNullOrEmpty(solarData))
        {
            throw new Exception("Solar data is empty or null.");
        }

        var sunrise = _jsonConverter.ParseSunriseData(solarData);

        if (sunrise == null)
        {
            _logger.LogError(
                $"No city data found for the given name. Possibly wrong city name or empty response.");
        }
        else
        {
            sunrise.City = city;
        }

        return sunrise;
    }

    public async Task<Sunset> GetSunset(City city, SolarDTO solarInput)
    {
        try
        {
            // Solar data
            var solarData = await DownloadCityData(solarInput);

            if (string.IsNullOrEmpty(solarData))
            {
                _logger.LogError("Solar data is empty or null.");
                return null;
            }

            var sunset = _jsonConverter.ParseSunsetData(solarData);

            if (sunset == null)
            {
                _logger.LogError(
                    $"No city data found for the given name. Possibly wrong city name or empty response.");
            }
            else
            {
                sunset.City = city;
            }

            return sunset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting sunset data for city.");
            return null;
        }
    }

    private async Task<string> DownloadCityData(SolarDTO solarInput)
    {
        try
        {
            var url = CreateSolarUrlForCity(solarInput);
            if (url == null)
            {
                _logger.LogError("Cannot find city to create url with!");
                throw new InvalidOperationException("City not found to create URL.");
            }

            _logger.LogInformation($"Solar url: {url}");

            using var client = new HttpClient();
            _logger.LogInformation("Calling SolarWatch API with url: {url}", url);

            var response = await client.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "An HTTP error occurred while calling the SolarWatch API.");
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while downloading data for city.");
            throw;
        }
    }

    private string CreateSolarUrlForCity(SolarDTO solarInput)
    {
        if (solarInput == null)
        {
            _logger.LogError("Cannot find city to create url with!");
            return null;
        }

        string formattedDate = solarInput.dateTime.ToString("yyyy-MM-dd");

        string url =
            $"https://api.sunrise-sunset.org/json?lat={solarInput.latitude.ToString()}&lng={solarInput.longitude.ToString()}&date={formattedDate}";

        return url;
    }
}