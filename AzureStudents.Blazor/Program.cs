using AzureStudents.Blazor.Api;
using AzureStudents.Blazor.Mapping;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace AzureStudents.Blazor;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // ==================================================================================================================
        // Mapping
        // ==================================================================================================================
        builder.Services.AddAutoMapper(
            typeof(AutoMapperProfile)
        );

        // ==================================================================================================================
        // Network (API Service, data transfer)
        // ==================================================================================================================
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Add API services with typed http clients
        builder.Services.AddHttpClient<IStudentsApiService, StudentsApiService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["StudentsApiBaseUrl"]!);
        });

        await builder.Build().RunAsync();
    }
}
