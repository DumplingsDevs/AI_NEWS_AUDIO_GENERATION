namespace AiNews.AudioProviders.ElevenLabs;

public interface IElevenLabsClient
{
    Task<byte[]> GetAudio(string previousText, string currentText, string nextText, ElevenLabsPayload payload);
}