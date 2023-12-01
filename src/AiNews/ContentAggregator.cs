namespace AiNews;

internal static class ContentAggregator
{
    public static IEnumerable<string> GetContentsForAudio(string input, string separator, int maxContentLength)
    {
        var chunks = input.Split(separator);
        var tempText = "";

        foreach (var chunk in chunks)
        {
            if (tempText.Length + chunk.Length < 4095)
            {
                tempText += (chunk);
            }
            else
            {
                yield return tempText;
                tempText = "";
            }
        }

        if (tempText != "")
        {
            yield return tempText;
        }
    }
}
