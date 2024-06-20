using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Data;
using SolarWatch.Model;
using SolarWatch.Services;
using SolarWatch.Services.Repository;
using SolarWatch.Services.SolarWatchService;

namespace SolarWatch.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class SolarWatchController : ControllerBase
{
    private readonly ILogger<SolarWatchController> _logger;
    private readonly ISolarWatchData _solarWatchData;
    private readonly IGeocodingCity _geocodingCity;
    private readonly ICityRepository _cityRepository;
    private readonly ISolarRepository _solarRepository;

    public SolarWatchController(ILogger<SolarWatchController> logger,
        ISolarWatchData solarWatchData, IGeocodingCity geocodingCity, ICityRepository cityRepository,
        ISolarRepository solarRepository)
    {
        _logger = logger;
        _solarWatchData = solarWatchData;
        _geocodingCity = geocodingCity;
        _cityRepository = cityRepository;
        _solarRepository = solarRepository;
    }


    [HttpGet("sunrise/{cityName}/{dateTime}"), Authorize(Roles ="User, Admin")]
    public async Task<IActionResult> GetCitySunrise(string cityName, DateTime dateTime)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(cityName))
                return BadRequest("City name cannot be empty.");

            if (dateTime == DateTime.MinValue || dateTime > DateTime.Now)
                return BadRequest("Invalid date and time.");

            // Find or store city
            var city = await _cityRepository.GetByName(cityName);

            if (!_cityRepository.GetAll().Result.ToList().Contains(city))
            {
                city = await _geocodingCity.GetCityLongAndLatByName(cityName);
                await _cityRepository.Add(city);
            }

            // Get sunrise time

            var sunrise = await _solarWatchData.GetSunrise(city, dateTime);
            if (sunrise == null)
            {
                return NotFound("Cant find solar data for this city");
            }

            bool sunriseExists = (await _solarRepository.GetAllSunrise()).Any(s =>
                s.City.Name == sunrise.City.Name && s.SunriseTime == sunrise.SunriseTime);

            if (!sunriseExists)
            {
                await _solarRepository.Add(sunrise);
            }

            // Return the sunrise time
            return Ok(sunrise);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }
    
    [HttpGet("sunriseid/{sunriseId}"),Authorize(Roles = "Admin")]
    public async Task<ActionResult<Sunrise>> GetSunriseById(int sunriseId)
    {
        try
        {
            var foundSunrise = await _solarRepository.GetSunriseById(sunriseId);
            if (foundSunrise == null)
            {
                return NotFound("No sunrise found with this id!");
            }

            return Ok(foundSunrise);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }

    [HttpPut("sunriseid/{sunriseId}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<Sunrise>> EditSunrise(int sunriseId, Sunrise newSunrise)
    {
        try
        {
            var oldSunrise = await _solarRepository.GetSunriseById(sunriseId);
            if (oldSunrise == null)
            {
                return NotFound("No sunrise found to edit");
            }

            await _solarRepository.UpdateSunrise(oldSunrise, newSunrise);
            return Ok(oldSunrise);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occurred while editing the city: {e.Message}");
        }
    }

    [HttpDelete("sunriseid/{sunriseId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSunrise(int sunriseId)
    {
        try
        {
            var sunrise = await _solarRepository.GetSunriseById(sunriseId);
            if (sunrise == null)
            {
                return NotFound("No sunrise to delete");
            }

            await _solarRepository.DeleteSunrise(sunrise);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while editing the city: {e.Message}"); 
        }
    }


    [HttpGet("sunset/{cityName}/{dateTime}"),Authorize(Roles ="User, Admin")]
    public async Task<IActionResult> GetCitySunset(string cityName, DateTime dateTime)
    {
       
        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(cityName))
                return BadRequest("City name cannot be empty.");

            if (dateTime == DateTime.MinValue || dateTime > DateTime.Now)
                return BadRequest("Invalid date and time.");

            // Find or store city
            var city = await _cityRepository.GetByName(cityName);
            if (!_cityRepository.GetAll().Result.ToList().Contains(city))
            {
                city = await _geocodingCity.GetCityLongAndLatByName(cityName);
                await _cityRepository.Add(city);
            }

            // Get sunset time

            var sunset = await _solarWatchData.GetSunset(city, dateTime);
            if (sunset == null)
            {
                return NotFound("Cant find solar data for this city");
            }

            bool sunsetExists = (await _solarRepository.GetAllSunset()).Any(s =>
                s.City.Name == sunset.City.Name && s.SunsetTime == sunset.SunsetTime);

            if (!sunsetExists)
            {
                await _solarRepository.Add(sunset);
            }


            // Return the sunrise time
            return Ok(sunset);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }
    
    [HttpGet("/sunsetid/{sunsetId}"),Authorize(Roles = "Admin")]
    public async Task<ActionResult<Sunset>> GetSunsetById(int sunsetId)
    {
        try
        {
            var foundSunset = await _solarRepository.GetSunsetById(sunsetId);
            if (foundSunset == null)
            {
                return NotFound("No sunset found with this id!");
            }

            return Ok(foundSunset);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}");
        }
    }

    [HttpPut("/sunsetid/{sunsetId}"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<Sunset>> EditSunset(int sunsetId, Sunset newSunset)
    {
        try
        {
            var oldSunset = await _solarRepository.GetSunsetById(sunsetId);
            if (oldSunset == null)
            {
                return NotFound("No sunrise found to edit");
            }

            await _solarRepository.UpdateSunset(oldSunset, newSunset);
            return Ok(oldSunset);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"An error occurred: {e.Message}");
        }
    }
    
    [HttpDelete("/sunsetid/{sunsetId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteSunset(int sunsetId)
    {
        try
        {
            var sunset = await _solarRepository.GetSunsetById(sunsetId);
            if (sunset == null)
            {
                return NotFound("No sunset to delete");
            }

            await _solarRepository.DeleteSunset(sunset);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {e.Message}"); 
        }
    }
}