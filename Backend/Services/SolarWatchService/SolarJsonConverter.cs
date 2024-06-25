using System.Globalization;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using SolarWatch.Model;

namespace SolarWatch.Services.SolarWatchService;

public class SolarJsonConverter : ISolarJsonConverter
{
    public Sunrise ParseSunriseData(string data)
    {
        if (data.IsNullOrEmpty())
        {
            return null;
        }
        
        JsonDocument solarJson = JsonDocument.Parse(data);

        JsonElement results = solarJson.RootElement.GetProperty("results");

        string sunriseTimeString = results.GetProperty("sunrise").GetString();

        
        if (!DateTime.TryParseExact(sunriseTimeString, "h:mm:ss tt", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime sunriseTime))
        {
            return null;
        }

        var sunrise = new Sunrise
        {
            SunriseTime = sunriseTime
        };
        

        return sunrise;
    }

    public Sunset ParseSunsetData(string data)
    {
        
        if (data.IsNullOrEmpty())
        {
            return null;
        }
        
        JsonDocument solarJson = JsonDocument.Parse(data);
        
        var results = solarJson.RootElement.GetProperty("results");
        var sunsetTimeString = results.GetProperty("sunset").GetString();
            
        if (!DateTime.TryParseExact(sunsetTimeString, "h:mm:ss tt", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime sunsetTime))
        {
            return null;
        }

        var sunset = new Sunset
        {
            SunsetTime = sunsetTime
        };
            

        return sunset;
    }
}