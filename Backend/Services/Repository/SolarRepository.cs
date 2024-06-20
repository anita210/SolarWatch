using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class SolarRepository : ISolarRepository
{
    private SolarWatchContext _solarWatchContext;

    public SolarRepository(SolarWatchContext solarWatchContext)
    {
        _solarWatchContext = solarWatchContext;
    }
    
    public async Task<IEnumerable<Sunrise>> GetAllSunrise()
    {
        return await _solarWatchContext.SunriseList.Include(c => c.City).ToListAsync();
    }

    public async Task <IEnumerable<Sunset>> GetAllSunset()
    {
        return await _solarWatchContext.SunsetList.Include(c => c.City).ToListAsync();
    }

    public async Task<Sunrise?> GetSunriseById(int id)
    {
        return await _solarWatchContext.SunriseList.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Sunrise sunrise)
    {
        _solarWatchContext.Add(sunrise);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task DeleteSunrise(Sunrise sunrise)
    {
        _solarWatchContext.Remove(sunrise);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task UpdateSunrise(Sunrise oldSunrise, Sunrise newSunrise)
    {
        try
        {
            _solarWatchContext.Entry(oldSunrise).Reference(s => s.City).LoadAsync();
            oldSunrise.SunriseTime = newSunrise.SunriseTime;
            oldSunrise.City.Name = newSunrise.City.Name;
            oldSunrise.City.State = newSunrise.City.State;
            oldSunrise.City.Country = newSunrise.City.Country;
            oldSunrise.City.Longitude = newSunrise.City.Longitude;
            oldSunrise.City.Latitude = newSunrise.City.Latitude;
            _solarWatchContext.Update(oldSunrise);
            await _solarWatchContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
     
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            
            throw new Exception("An error occurred while editing the city: " + ex.Message, ex);
        }
    }
    
    public async Task<Sunset?> GetSunsetById(int id)
    {
        return await _solarWatchContext.SunsetList.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Sunset sunset)
    {
        _solarWatchContext.Add(sunset);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task DeleteSunset(Sunset sunset)
    {
        _solarWatchContext.Remove(sunset);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task UpdateSunset(Sunset oldSunset, Sunset newSunset)
    {
        try
        {
            _solarWatchContext.Entry(oldSunset).Reference(s => s.City).LoadAsync();
            oldSunset.SunsetTime = newSunset.SunsetTime;
            oldSunset.City.Name = newSunset.City.Name;
            oldSunset.City.State = newSunset.City.State;
            oldSunset.City.Country = newSunset.City.Country;
            oldSunset.City.Longitude = newSunset.City.Longitude;
            oldSunset.City.Latitude = newSunset.City.Latitude;
            _solarWatchContext.Update(oldSunset);
            await _solarWatchContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
     
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            
            throw new Exception("An error occurred while editing the city: " + ex.Message, ex);
        }
    }
}