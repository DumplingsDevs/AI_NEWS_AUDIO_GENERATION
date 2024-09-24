using System.Net.Http.Json;
using AiNews.AudioProviders.OpenAI.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiNews.AudioProviders.ElevenLabs;

internal class ElevenLabsClient : IElevenLabsClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ElevenLabsClient> _logger;
    private readonly ElevenLabsOptions _options;

    public ElevenLabsClient(IHttpClientFactory httpClientFactory, ILogger<ElevenLabsClient> logger,
        IOptions<ElevenLabsOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<byte[]> GetAudio(string? previousText, string currentText, string? nextText, ElevenLabsPayload payload)
    {
        var httpClient = GetClient();
        
        var requestData = new
        {
            model_id = payload.ModelId,
            text = currentText,
            previous_text = previousText,
            next_text = nextText,
            voice_settings = new
            {
                similarity_boost = payload.SimilarityBoost,
                stability = payload.Stability,
                style = payload.Style,
                use_speaker_boost = payload.UseSpeakerBoost
            }
        };
        
        using var response = await httpClient.PostAsJsonAsync($"/v1/text-to-speech/{payload.VoiceId}?output_format=mp3_44100_128", requestData);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("ElevenLabs Request success with parameters: {@RequestData}", requestData);
            return await response.Content.ReadAsByteArrayAsync();
        }

        _logger.LogError("ErrorCode from OpenAI: {StatusCode} {Response}", response.StatusCode, await response.Content.ReadAsStringAsync());

        throw new OpenAiException(response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private HttpClient GetClient()
    {
        var httpClient = _httpClientFactory.CreateClient("ElevenLabs");
        httpClient.Timeout = TimeSpan.FromMinutes(10);
        httpClient.BaseAddress = new Uri(_options.ApiUrl);
        httpClient.DefaultRequestHeaders.Add("xi-api-key", _options.ApiKey);

        return httpClient;
    }
}