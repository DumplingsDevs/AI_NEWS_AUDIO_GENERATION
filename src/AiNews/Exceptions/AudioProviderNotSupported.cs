namespace AiNews.Exceptions;

internal class AudioProviderNotSupported : Exception
{
    private const string ErrorMessage = "Audio provider with name {0} not supported.";

    public AudioProviderNotSupported(string audioProviderName) : base(string.Format(ErrorMessage, audioProviderName))
    {
    }
}