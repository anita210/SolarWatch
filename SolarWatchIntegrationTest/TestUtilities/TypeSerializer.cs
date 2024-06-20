using System.Text;
using System.Text.Json;
using SolarWatch.Model;

namespace SolarWatchIntegrationTest.TestUtilities;

public static class TypeSerializer
{
    public static async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        
        var resString = await response.Content.ReadAsStringAsync();

        // Check if the response content is empty
        if (string.IsNullOrWhiteSpace(resString))
        {
            throw new InvalidOperationException("The response content is empty or null.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            var respContent = JsonSerializer.Deserialize<T>(resString, options);
            return respContent;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Error deserializing the response content to the target type.", ex);
        }
    }

    public static StringContent Serialize<T>(T newEntity)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            var json = JsonSerializer.Serialize(newEntity, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Error serializing the object to JSON", ex);
        }
    }
}