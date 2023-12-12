using AiNews;
using AiNews.Asp;
using AiNews.Extensions;
using AiNews.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(
        builder => { builder.UseMiddleware<ExceptionLoggingMiddleware>(); }
    ).ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddScoped<IOpenAiClient, OpenAiClient>();
        services.AddScoped<IAudioGenerationService, OpenAiAudioGenerationService>();
        services.AddOptions<AiNewsOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AiNewsOptions").Bind(settings);
            });
    })
    .Build();

host.Run();