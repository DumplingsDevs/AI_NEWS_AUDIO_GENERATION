namespace AiNews.AudioProviders.ElevenLabs;

public interface IElevenLabsClient
{
    Task<byte[]> GetAudio(string input, ElevenLabsPayload payload);
}