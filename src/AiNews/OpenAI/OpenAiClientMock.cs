using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AiNews.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiNews.OpenAI;

/// <summary>
/// Used for tests
/// </summary>
internal class OpenAiClientMock : IOpenAiClient
{
    private readonly ILogger<OpenAiClientMock> _logger;
    private readonly AiNewsOptions _options;
    private static int Counter = 0;
    public OpenAiClientMock(ILogger<OpenAiClientMock> logger, IOptions<AiNewsOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<byte[]> GetAudio(string input)
    {
        Counter++;
        var counter = Counter;
        _logger.LogInformation($"Started GetAudio {counter}");
        Random rnd = new Random();

        await Task.Delay(rnd.Next(1000, 5000));

        _logger.LogInformation($"Finished GetAudio {counter}");

        return Encoding.ASCII.GetBytes("Finished");
    }
}