using Microsoft.EntityFrameworkCore;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public class CityRepository : ICityRepository
{
    private SolarWatchContext _solarWatchContext;

    public CityRepository(SolarWatchContext solarWatchContext)
    {
        _solarWatchContext = solarWatchContext;
    }

    public async Task<IEnumerable<City>> GetAll()
    {
        return await _solarWatchContext.Cities.ToListAsync();
    }


    public async Task<City?> GetByName(string name)
    {
        return await _solarWatchContext.Cities.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<City?> GetById(int id)
    {
        return await _solarWatchContext.Cities.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(City city)
    {
        _solarWatchContext.Add(city);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task Delete(City city)
    {
        _solarWatchContext.Remove(city);
        await _solarWatchContext.SaveChangesAsync();
    }

    public async Task Update(City oldCity, City newCity)
    {
        try
        {
            oldCity.Name = newCity.Name;
            oldCity.State = newCity.State;
            oldCity.Country = newCity.Country;
            oldCity.Longitude = newCity.Longitude;
            oldCity.Latitude = newCity.Latitude;
            
            _solarWatchContext.Update(oldCity);
            
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