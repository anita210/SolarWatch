using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolarWatch.Data;
using SolarWatch.Model;

namespace SolarWatchIntegrationTest.TestUtilities;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SolarWatchContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            services.AddDbContext<SolarWatchContext>(options =>
            {
                options.UseInMemoryDatabase("solarTestDb");
                options.UseInternalServiceProvider(serviceProvider);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<SolarWatchContext>();
            appContext.Database.EnsureCreated();

            SeedUsersData(appContext);
            SeedTestCityData(appContext);
        });
    }

    private void SeedUsersData(SolarWatchContext context)
    {
        // Create roles if they don't exist
        var roleManager = context.GetService<RoleManager<IdentityRole>>();
        var configuration = context.GetService<IConfiguration>();
        var adminRoleName = configuration?["Roles:Admin"];
        var userRoleName = configuration?["Roles:User"];

        if (roleManager != null && adminRoleName != null && userRoleName != null)
        {
            if (!roleManager.Roles.Any(r => r.Name == adminRoleName))
            {
                roleManager.CreateAsync(new IdentityRole(adminRoleName)).Wait();
            }

            if (!roleManager.Roles.Any(r => r.Name == userRoleName))
            {
                roleManager.CreateAsync(new IdentityRole(userRoleName)).Wait();
            }
        }

        // Create admin user if it doesn't exist
        var userManager = context.GetService<UserManager<IdentityUser>>();
        var adminEmail = "admin@admin.com";

        if (userManager != null && adminRoleName != null)
        {
            if (userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail
                };

                var result = userManager.CreateAsync(adminUser, "admin123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, adminRoleName).Wait();
                }
            }
        }

        var userEmail = "user@user.com";
        if (userManager != null && userRoleName != null)
        {
            if (userManager.FindByEmailAsync(userEmail).Result == null)
            {
                var user = new IdentityUser
                {
                    UserName = "user",
                    Email = userEmail
                };

                var result = userManager.CreateAsync(user, "password").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, userRoleName).Wait();
                }
            }
        }
    }

    private void SeedTestCityData(SolarWatchContext context)
    {
        var testCity = new City
            { Name = "testCity", Longitude = 0.01, Latitude = 0.02, State = "testState", Country = "testCountry" };
        context.Cities.Add(testCity);
        context.SaveChanges();
        
    }
}