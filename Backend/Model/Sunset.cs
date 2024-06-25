using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SolarWatch.Model;

public class Sunset
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public City? City { get; set; }
    public DateTime SunsetTime { get; set; }
}