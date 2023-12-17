namespace AiNews;

public interface IAudioGenerationService
{
    Task<AudioGenerationResult> GetAudio(Func<int, IEnumerable<string>> getContent, string providerPayload);
    string Type { get; }
}