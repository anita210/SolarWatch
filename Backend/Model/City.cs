using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Model;

public class City
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }
    
}
    


   

   