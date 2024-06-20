using System.Net;
using SolarWatch.Model;
using SolarWatchIntegrationTest.AuthHelper;
using SolarWatchIntegrationTest.TestUtilities;


namespace SolarWatchIntegrationTest.ControllerTests;

public class GeocodeControllerTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;


    public GeocodeControllerTest()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetGeocodedCityByName_ReturnsOk_IfFoundInDb()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync($"api/Geocode/testCity");

        var respContent = await TypeSerializer.Deserialize<City>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("testCity", respContent.Name);
    }

    [Fact]
    public async Task GetGeocodedCityByName_ReturnsOk_IfNotInDb()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


        var response = await _client.GetAsync($"api/Geocode/washington");

        var respContent = await TypeSerializer.Deserialize<City>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Washington", respContent.Name);
    }

    [Fact]
    public async Task GetGeocodeCityByName_ReturnsNotFound_IfInvalidCityName()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync($"api/Geocode/noCityTest");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGeocodeCityByName_ReturnsUnauthorized_IfUnauthorized()
    {
        var response = await _client.GetAsync($"api/Geocode/washington");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetGeocodeCityById_ReturnsOk_IfCorrectId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync($"api/Geocode/cityid/1");

        var respContent = await TypeSerializer.Deserialize<City>(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, respContent.Id);
    }


    [Fact]
    public async Task GetGeocodeCityById_ReturnsNotFound_IfIncorrectId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.GetAsync($"api/Geocode/cityid/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGeocodeCityById_ReturnsUnauthorized_IfUnauthorized()
    {
        var response = await _client.GetAsync($"api/Geocode/cityId/1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCityById_ReturnsOk_IfCorrectId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        //POST A CITY TO CHANGE
        var newCity = new City
            { Name = "newCityName", Longitude = 0.03, Latitude = 0.04, Country = "newCountry", State = "newState" };
        var content = TypeSerializer.Serialize<City>(newCity);

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PostAsync($"api/Geocode", content);
        response.EnsureSuccessStatusCode();
        var respContent = await TypeSerializer.Deserialize<City>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(newCity.Name, respContent.Name);
        Assert.NotEqual(0, respContent.Id);


        //UPDATE IT
        var newCityV2 = new City
        {
            Name = "updatedCityName", Longitude = 0.04, Latitude = 0.05, Country = "updatedCountry",
            State = "updatedState"
        };
        var putContent = TypeSerializer.Serialize<City>(newCityV2);

        var putResponse = await _client.PutAsync($"api/Geocode/{respContent.Id}", putContent);
        putResponse.EnsureSuccessStatusCode();
        var putRespContent = await TypeSerializer.Deserialize<City>(putResponse);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        Assert.Equal(newCityV2.Name, putRespContent.Name);
        Assert.Equal(2, putRespContent.Id);
    }

    [Fact]
    public async Task UpdateCityById_ReturnsNotFound_IfNoCorrectId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        var newCityV2 = new City
        {
            Name = "updatedCityName", Longitude = 0.04, Latitude = 0.05, Country = "updatedCountry",
            State = "updatedState"
        };
        var putContent = TypeSerializer.Serialize<City>(newCityV2);

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var putResponse = await _client.PutAsync($"api/Geocode/3", putContent);


        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateCityById_ReturnsUnauthorized_IfNotLoggedIn()
    {
        var newCityV2 = new City
        {
            Name = "updatedCityName", Longitude = 0.04, Latitude = 0.05, Country = "updatedCountry",
            State = "updatedState"
        };
        var putContent = TypeSerializer.Serialize<City>(newCityV2);
        var putResponse = await _client.PutAsync($"api/Geocode/3", putContent);


        Assert.Equal(HttpStatusCode.Unauthorized, putResponse.StatusCode);
    }

    [Fact]
    public async Task PostCity_ReturnsCreated()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        var newCity = new City
            { Name = "newCityName", Longitude = 0.03, Latitude = 0.04, Country = "newCountry", State = "newState" };
        var content = TypeSerializer.Serialize<City>(newCity);

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PostAsync($"api/Geocode", content);
        response.EnsureSuccessStatusCode();
        var respContent = await TypeSerializer.Deserialize<City>(response);

        var newCityLocation = response.Headers.Location;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/api/Geocode/cityid/2", newCityLocation.OriginalString);
        Assert.Equal(newCity.Name, respContent.Name);
        Assert.Equal(newCity.Longitude, respContent.Longitude);
        Assert.Equal(newCity.Latitude, respContent.Latitude);
        Assert.Equal(newCity.Country, respContent.Country);
        Assert.Equal(newCity.State, respContent.State);
        Assert.NotEqual(0, respContent.Id);
    }

    [Fact]
    public async Task PostCity_ReturnsBadRequest_IfModelStateIsInvalid()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        var invalidCity = new City
        {
            // Missing the Name 
            Longitude = 0.03,
            Latitude = 0.04,
            Country = "newCountry",
            State = "newState"
        };

        var content = TypeSerializer.Serialize<City>(invalidCity);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var response = await _client.PostAsync("api/Geocode", content);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("The Name field is required.", response.Content.ReadAsStringAsync().Result);
    }

    [Fact]
    public async Task PostCity_ReturnsUnauthorized_IfNotLoggedIn()
    {
        var invalidCity = new City
        {
            // Missing the Name 
            Longitude = 0.03,
            Latitude = 0.04,
            Country = "newCountry",
            State = "newState"
        };

        var content = TypeSerializer.Serialize<City>(invalidCity);

        var response = await _client.PostAsync("api/Geocode", content);


        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    
    public async Task DeleteCityById_ReturnsNoContent_IfCorrectId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        //POST A CITY TO DELETE
        var newCity = new City
            { Name = "newCityName", Longitude = 0.03, Latitude = 0.04, Country = "newCountry", State = "newState" };
        var content = TypeSerializer.Serialize<City>(newCity);

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PostAsync($"api/Geocode", content);
        response.EnsureSuccessStatusCode();
        var respContent = await TypeSerializer.Deserialize<City>(response);

        var newCityLocation = response.Headers.Location;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/api/Geocode/cityid/2", newCityLocation.OriginalString);
        Assert.Equal(newCity.Name, respContent.Name);
        Assert.Equal(newCity.Longitude, respContent.Longitude);
        Assert.Equal(newCity.Latitude, respContent.Latitude);
        Assert.Equal(newCity.Country, respContent.Country);
        Assert.Equal(newCity.State, respContent.State);
        Assert.NotEqual(0, respContent.Id);


        //DELETE IT
        var delResponse = await _client.DeleteAsync($"api/Geocode/{respContent.Id}");
        response.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.NoContent, delResponse.StatusCode);
    }
    
    [Fact]

    public async Task DeleteCity_ReturnsNotFound_IfNoValidId()
    {
        var token = await AuthorizationHelper.LoginUser(_client);
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.DeleteAsync($"api/Geocode/2");


        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]

    public async Task DeleteCity_ReturnsUnauthorized_IfNotLoggedIn()
    {
        var token = await AuthorizationHelper.LoginUser(_client);

        //POST A CITY TO DELETE
        var newCity = new City
            { Name = "newCityName", Longitude = 0.03, Latitude = 0.04, Country = "newCountry", State = "newState" };
        var content = TypeSerializer.Serialize<City>(newCity);

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var response = await _client.PostAsync($"api/Geocode", content);
        response.EnsureSuccessStatusCode();
        var respContent = await TypeSerializer.Deserialize<City>(response);

        var newCityLocation = response.Headers.Location;

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("http://localhost/api/Geocode/cityid/2", newCityLocation.OriginalString);
        Assert.Equal(newCity.Name, respContent.Name);
        Assert.Equal(newCity.Longitude, respContent.Longitude);
        Assert.Equal(newCity.Latitude, respContent.Latitude);
        Assert.Equal(newCity.Country, respContent.Country);
        Assert.Equal(newCity.State, respContent.State);
        Assert.NotEqual(0, respContent.Id);


        //DELETE IT

        _client.DefaultRequestHeaders.Remove("Authorization");
        var delResponse = await _client.DeleteAsync($"api/Geocode/{respContent.Id}");
        
        Assert.Equal(HttpStatusCode.Unauthorized, delResponse.StatusCode);
    }
}