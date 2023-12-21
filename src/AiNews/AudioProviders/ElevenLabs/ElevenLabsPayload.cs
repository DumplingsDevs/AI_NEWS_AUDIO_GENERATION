namespace AiNews.AudioProviders.ElevenLabs;

public record ElevenLabsPayload
{
    public string ModelId { get; set; } = "eleven_multilingual_v2";
    public string VoiceId { get; set; } = "vsqTqurA65sbFvOYeEmi";
    public float SimilarityBoost { get; set; } = (float)0.7;
    public float Stability { get; set; } = (float)0.8;
    public float Style { get; set; } = (float)0.1;
    public bool UseSpeakerBoost { get; set; } = true;
}
