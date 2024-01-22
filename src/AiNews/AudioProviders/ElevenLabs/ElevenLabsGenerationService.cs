using System.Text.Json;
using AiNews.AudioProviders.OpenAI.Exceptions;
using AiNews.Exceptions;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;

namespace AiNews.AudioProviders.ElevenLabs;

internal class ElevenLabsGenerationService : IAudioGenerationService
{
    private readonly IElevenLabsClient _elevenLabsClient;

    public string Type => "ElevenLabs";

    public ElevenLabsGenerationService(IElevenLabsClient elevenLabsClient)
    {
        this._elevenLabsClient = elevenLabsClient;
    }

    public async Task<AudioGenerationResult> GetAudio(Func<int, IEnumerable<string>> getContent, object providerPayload)
    {
        var payload = ((JsonElement)providerPayload).Deserialize<ElevenLabsPayload>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});

        if (payload is null)
        {
            throw new CannotDeserializePayloadException(((JsonElement)providerPayload).GetString(), "ElevenLabs");
        }

        var contents = getContent(0).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var policy = GetResiliencePolicy();

        var jobs = contents.Select(
            async x
                => await policy.ExecuteAsync(async ()
                    => await _elevenLabsClient.GetAudio(x, payload)
                )
        );

        var audioResults = await Task.WhenAll(jobs);
        var audio = audioResults.SelectMany(x => x).ToArray();

        return new AudioGenerationResult(audio, "mp3");
    }

    private static AsyncPolicyWrap GetResiliencePolicy()
    {
        var rateLimit = Policy.RateLimitAsync(1, TimeSpan.FromMinutes(1), 20);
        var rateLimitPolicy = Policy.Handle<RateLimitRejectedException>()
            .WaitAndRetryAsync(100, _ => TimeSpan.FromSeconds(2))
            .WrapAsync(rateLimit);
        return Policy.Handle<OpenAiException>()
            .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(2 * retryCount))
            .WrapAsync(rateLimitPolicy);
    }
}