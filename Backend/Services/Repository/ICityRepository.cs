using SolarWatch.Model;

namespace SolarWatch.Services.Repository;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetAll();

    Task<City?> GetByName(string name);

    Task<City?> GetById(int id);

    Task Add(City city);
    Task Delete(City city);
    Task Update(City oldCity, City newCity);
}