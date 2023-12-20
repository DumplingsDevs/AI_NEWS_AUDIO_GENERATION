using AiNews;
using AiNews.Asp;
using AiNews.AudioProviders.ElevenLabs;
using AiNews.AudioProviders.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(
        builder => { builder.UseMiddleware<ExceptionLoggingMiddleware>(); }
    ).ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddScoped<IOpenAiClient, OpenAiClient>();
        services.AddScoped<IElevenLabsClient, ElevenLabsClient>();
        services.AddScoped<IAudioGenerationService, OpenAiAudioGenerationService>();
        services.AddScoped<IAudioGenerationService, ElevenLabsGenerationService>();
        services.AddOptions<OpenAiOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("OpenAiOptions").Bind(settings);
            });
        services.AddOptions<ElevenLabsOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("ElevenLabsOptions").Bind(settings);
            });
    })
    .Build();

host.Run();