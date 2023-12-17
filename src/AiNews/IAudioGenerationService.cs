namespace AiNews;

public interface IAudioGenerationService
{
    Task<AudioGenerationResult> GetAudio(IEnumerable<string> contents);
    string Type { get; }
}