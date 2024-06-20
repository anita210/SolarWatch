using System.Text;
using System.Text.Json;
using SolarWatch.Services.Authentication;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SolarWatchIntegrationTest.AuthHelper;

public class AuthorizationHelper
{
    public static async Task<string> LoginUser(HttpClient client)
    {
        var loginRequest = new AuthRequest("user@user.com", "password");
        var jsonReq = JsonSerializer.Serialize(loginRequest);
        var loginContent = new StringContent(jsonReq, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("/api/Auth/Login", loginContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, jsonOption);
            return authResponse.Token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<string> LoginAdmin(HttpClient client)
    {
        var loginRequest = new AuthRequest("admin@admin.com", "admin123");
        var jsonReq = JsonSerializer.Serialize(loginRequest);
        var loginContent = new StringContent(jsonReq, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("/api/Auth/Login", loginContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, jsonOption);
            return authResponse.Token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}