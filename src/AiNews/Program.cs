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
        
        services.Configure<OpenAiOptions>(options =>
        {
            options.ApiKey = "";
            options.ApiUrl = "https://api.openai.com";
        });

        services.Configure<ElevenLabsOptions>(options =>
        {
            options.ApiKey = "";
            options.ApiUrl = "https://api.elevenlabs.io";
        });
    })
    .Build();

host.Run();