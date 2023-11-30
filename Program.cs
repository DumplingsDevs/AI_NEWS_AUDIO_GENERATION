using function;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddOptions<AiNewsOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AiNewsOptions").Bind(settings);
            });
    })
    .Build();
host.Run();
