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
        var payload = DeserializePayload(providerPayload);

        var contents = getContent(5000).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var policy = GetResiliencePolicy();

        var audioResults = await GenerateAudioSegments(contents, payload, policy);

        var audio = audioResults.SelectMany(x => x).ToArray();

        return new AudioGenerationResult(audio, "mp3");
    }

    private async Task<IEnumerable<byte[]>> GenerateAudioSegments(List<string> contents, ElevenLabsPayload payload, IAsyncPolicy policy)
    {
        var jobs = new List<Task<byte[]>>();

        for (int i = 0; i < contents.Count; i++)
        {
            var previousText = i > 0 ? contents[i - 1] : null;
            var currentText = contents[i];
            var nextText = i < contents.Count - 1 ? contents[i + 1] : null;

            jobs.Add(policy.ExecuteAsync(() =>
                _elevenLabsClient.GetAudio(previousText, currentText, nextText, payload)));
        }

        return await Task.WhenAll(jobs);
    }

    private static ElevenLabsPayload DeserializePayload(object providerPayload)
    {
        var payload = ((JsonElement)providerPayload).Deserialize<ElevenLabsPayload>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (payload is null)
        {
            throw new CannotDeserializePayloadException(((JsonElement)providerPayload).GetString(), "ElevenLabs");
        }

        return payload;
    }

    private static AsyncPolicyWrap GetResiliencePolicy()
    {
        var concurrencyLimit = Policy.BulkheadAsync(2, int.MaxValue);
        var retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        return Policy.WrapAsync(retryPolicy, concurrencyLimit);
    }
}