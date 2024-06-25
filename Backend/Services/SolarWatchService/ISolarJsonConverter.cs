using SolarWatch.Model;

namespace SolarWatch.Services.SolarWatchService;

public interface ISolarJsonConverter
{
    public Sunrise ParseSunriseData(string data);
    
    public Sunset ParseSunsetData(string data);
}