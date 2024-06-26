using IpLookup.Api.Downloader;
using IpLookup.Api.Endpoints;
using IpLookup.Api.Import;
using IpLookup.Api.Lookup;
using IpLookup.Api.Storage.InMemory;

namespace IpLookup.Api;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
               .AddEndpointsApiExplorer()
               .AddSwaggerGen();

        builder.Services.Configure<ImportTaskOptions>(
            builder.Configuration.GetSection(ImportTaskOptions.ImportTask));

        builder.Services.AddSingleton<IDownloader, Downloader.Downloader>();
        builder.Services.AddSingleton<ImportService>();
        builder.Services.AddSingleton<ImportTask>();
        builder.Services.AddHostedService<ImportTask>(
            p => p.GetRequiredService<ImportTask>());

        builder.Services.AddSingleton<IIpIndex, IpIndex>();
        builder.Services.AddSingleton<IIpLookupService, IpLookupService>();
    }

    public static void RegisterMiddlewares(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseExceptionHandler(ErrorHandler.Endpoint);
    }

    public static void RegisterEndpoints(this WebApplication app)
    {
        app.MapErrorHandler();
        app.MapStatsEndpoints();
        app.MapIpLookupEndpoints();
    }
}