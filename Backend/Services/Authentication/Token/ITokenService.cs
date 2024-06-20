using Microsoft.AspNetCore.Identity;

namespace SolarWatch.Services.Authentication.Token;

public interface ITokenService
{
    public string CreateToken(IdentityUser user, string role);
}