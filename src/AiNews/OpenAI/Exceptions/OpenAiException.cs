using System.Net;

namespace AiNews.OpenAI.Exceptions;

internal class OpenAiException : Exception
{
    private const string ErrorMessage = "ErrorCode from OpenAI: {0} {1}";

    public OpenAiException(HttpStatusCode statusCode, string httpContent) : base(string.Format(ErrorMessage, statusCode,
        httpContent))
    {
    }
}