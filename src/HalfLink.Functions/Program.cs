using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddHttpClient("HalfLinkApiClient", client =>
{
    var halfLinkUrl = builder.Configuration.GetValue<string>("HalfLinkApiUrl");
    ArgumentException.ThrowIfNullOrWhiteSpace(halfLinkUrl, nameof(halfLinkUrl));

    client.BaseAddress = new Uri(halfLinkUrl);
});

builder.Build().Run();
