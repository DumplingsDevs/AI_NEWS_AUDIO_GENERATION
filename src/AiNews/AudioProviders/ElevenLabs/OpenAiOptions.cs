namespace AiNews.AudioProviders.ElevenLabs;

public class ElevenLabsOptions
{
    public string ApiKey { get; set; }
    public string ApiUrl { get; set; }
    public string ModelId { get; set; } = "eleven_multilingual_v2";
    public double SimilarityBoost { get; set; } = 0.7;
    public double Stability { get; set; } = 0.8;
    public double Style { get; set; } = 0.1;
    public bool UseSpeakerBoost { get; set; } = true;
}
