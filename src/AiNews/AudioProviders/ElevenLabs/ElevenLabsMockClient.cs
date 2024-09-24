using System.Net.Http.Json;
using System.Text;
using AiNews.AudioProviders.OpenAI.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiNews.AudioProviders.ElevenLabs;

internal class ElevenLabsMockClient : IElevenLabsClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ElevenLabsMockClient> _logger;
    private readonly ElevenLabsOptions _options;
    private static int Counter = 0;
    public ElevenLabsMockClient(IHttpClientFactory httpClientFactory, ILogger<ElevenLabsMockClient> logger,
        IOptions<ElevenLabsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<byte[]> GetAudio(string previousText, string currentText, string nextText, ElevenLabsPayload payload)
    {
        Counter++;
        var counter = Counter;
        _logger.LogInformation($"Started GetAudio {counter}");
        Random rnd = new Random();

        await Task.Delay(rnd.Next(1000, 5000));

        _logger.LogInformation($"Finished GetAudio {counter}");

        return Encoding.ASCII.GetBytes($"Previous: {previousText} {Environment.NewLine} currentText: {currentText} {Environment.NewLine} nextText: {nextText} {Environment.NewLine}{Environment.NewLine}");
    }
}