using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiNews.AudioProviders.OpenAI.Exceptions;
using AiNews.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiNews.AudioProviders.OpenAI;

internal class OpenAiClient : IOpenAiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenAiClient> _logger;
    private readonly AiNewsOptions _options;

    public OpenAiClient(IHttpClientFactory httpClientFactory, ILogger<OpenAiClient> logger,
        IOptions<AiNewsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<byte[]> GetAudio(string input)
    {
        var httpClient = GetClient();

        var requestData = new
        {
            model = "tts-1-hd",
            voice = "nova",
            input,
        };

        using var response = await httpClient.PostAsJsonAsync("/v1/audio/speech", requestData);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        _logger.LogError($"ErrorCode from OpenAI: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");

        throw new OpenAiException(response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private HttpClient GetClient()
    {
        var httpClient = _httpClientFactory.CreateClient("OpenAI");
        httpClient.BaseAddress = new Uri(_options.OpenAiUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.OpenAiKey);

        return httpClient;
    }
}