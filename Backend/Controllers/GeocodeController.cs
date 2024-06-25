using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Model.DTOModel;
using SolarWatch.Services;
using SolarWatch.Services.Repository;
using SolarWatch.Services.SolarWatchService;

namespace SolarWatch.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class GeocodeController : ControllerBase
{
    private readonly ILogger<GeocodeController> _logger;

    private readonly IGeocodingCity _geocodingCity;

    private readonly ICityRepository _cityRepository;


    public GeocodeController(ILogger<GeocodeController> logger, IGeocodingCity geocodingCity,
        ICityRepository cityRepository)
    {
        _logger = logger;
        _geocodingCity = geocodingCity;
        _cityRepository = cityRepository;
    }

    [HttpGet, Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<City>> GetCityGeocodeByName([FromBody]GeocodeDTO cityInput)
    {
        try
        {
            var city = await _cityRepository.GetByName(cityInput.cityName);
            if (city != null)
            {
                return Ok(city);
            }

            city = await _geocodingCity.GetCityLongAndLatByName(cityInput);

            if (city == null)
            {
                return NotFound("City not created!");
            }

            await _cityRepository.Add(city);
            return Ok(city);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [HttpGet("cityid/{cityId}"), Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<City>> GetCityById(int cityId)
    {
        try
        {
            var foundCity = await _cityRepository.GetById(cityId);
            if (foundCity == null)
            {
                return NotFound("No city found with this id!");
            }

            return Ok(foundCity);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }


    [HttpPost, Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<City>> AddCity(City city)
    {
        try
        {
            await _cityRepository.Add(city);

            var createdCityUri = Url.Action("GetCityById", "Geocode", new { cityId = city.Id }, Request.Scheme);

            return Created(createdCityUri, city);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }

    [HttpPut("{cityId}"), Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> EditCity(int cityId, City city)
    {
        try
        {
            var cityToEdit = await _cityRepository.GetById(cityId);
            if (cityToEdit == null)
            {
                return NotFound("No city found with this id to edit!");
            }

            await _cityRepository.Update(cityToEdit, city);

            return Ok(cityToEdit);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }

    [HttpDelete("{cityId}"), Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteCity(int cityId)
    {
        try
        {
            var city = await _cityRepository.GetById(cityId);
            if (city == null)
            {
                return NotFound("No city found to delete");
            }

            await _cityRepository.Delete(city);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }
}