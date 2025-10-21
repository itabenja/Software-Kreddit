using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using kreddit_app;
using kreddit_app.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Læs API-base-URL fra konfiguration (wwwroot/appsettings*.json)
var apiBaseUrl = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("Api:BaseUrl mangler i konfigurationen (fx appsettings.Development.json).");
}

// Registrér HttpClient til at ramme backend-API'et direkte
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Registrér dine services der bruger HttpClient
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();