namespace AiNews.Exceptions;

internal class CannotDeserializePayloadException : Exception
{
    private const string ErrorMessage = "Payload {0} cannot be deserialized for provider {1}.";

    public CannotDeserializePayloadException(string? payload, string providerName) : base(string.Format(ErrorMessage, payload, providerName))
    {
    }
}