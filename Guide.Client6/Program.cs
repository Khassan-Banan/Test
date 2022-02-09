using Guide.Client6.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Guide.Client6
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // MudBlazor
            builder.Services.AddMudServices();

            builder.Services.AddScoped<ProgramService>();
            builder.Services.AddScoped<EquivalencyService>();
            builder.Services.AddScoped<FilterService>();
            builder.Services.AddScoped<TopBarService>();
            await builder.Build().RunAsync();
        }
    }
}