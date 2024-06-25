using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Model.DTOModel;

public record GeocodeDTO(
    [Required]
    string cityName,
    string stateCode,
    [Required]
    string? countryCode);