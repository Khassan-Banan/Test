using Guide.Client.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using MudBlazor.Services;

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Guide.Client
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
            builder.Services.AddScoped<FiltersService>();
            builder.Services.AddScoped<TopBarService>();
            await builder.Build().RunAsync();
        }
    }
}