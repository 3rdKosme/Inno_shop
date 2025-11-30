using Inno_Shop.ProductService.Infrastructure.Options;
using Inno_Shop.ProductService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inno_Shop.ProductService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            var root = new InMemoryDatabaseRoot();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestProductDb", root);
            });
            
            services.Configure<ProductServiceOptions>(options =>
            {
                options.HeaderName = "ProductServiceKey";
                options.ServiceKey = "test-service-key";
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);
            services.AddScoped<TestAuthHandler>();
            services.PostConfigureAll<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            });
        });
    }
}



