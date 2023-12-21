namespace AiNews;

public interface IAudioGenerationService
{
    Task<AudioGenerationResult> GetAudio(Func<int, IEnumerable<string>> getContent, object providerPayload);
    string Type { get; }
}