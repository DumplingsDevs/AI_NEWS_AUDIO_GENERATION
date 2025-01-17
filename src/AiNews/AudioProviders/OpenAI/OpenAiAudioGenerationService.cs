using AiNews.AudioProviders.OpenAI.Exceptions;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;

namespace AiNews.AudioProviders.OpenAI;

internal class OpenAiAudioGenerationService : IAudioGenerationService
{
    private readonly IOpenAiClient _openAiClient;

    public string Type => "OpenAI";

    public OpenAiAudioGenerationService(IOpenAiClient openAiClient)
    {
        this._openAiClient = openAiClient;
    }

    public async Task<AudioGenerationResult> GetAudio(Func<int, IEnumerable<string>> getContent, object providerPayload)
    {
        var contents = getContent(4095);

        var policy = GetResiliencePolicy();

        var jobs = contents.Select(
            async x
                => await policy.ExecuteAsync(async ()
                    => await _openAiClient.GetAudio(x)
                )
        );

        var audioResults = await Task.WhenAll(jobs);
        var audio = audioResults.SelectMany(x => x).ToArray();

        return new AudioGenerationResult(audio, "mp3");
    }

    private static AsyncPolicyWrap GetResiliencePolicy()
    {
        var rateLimit = Policy.RateLimitAsync(1, TimeSpan.FromMinutes(1), 3);
        var rateLimitPolicy = Policy.Handle<RateLimitRejectedException>()
            .WaitAndRetryAsync(100, _ => TimeSpan.FromSeconds(2))
            .WrapAsync(rateLimit);
        return Policy.Handle<OpenAiException>()
            .WaitAndRetryAsync(3, retryCount => TimeSpan.FromSeconds(2 * retryCount))
            .WrapAsync(rateLimitPolicy);
    }
}