namespace Gonscheg.Helpers;

public static class TelegramMarkdownV2Helper
{
    private static readonly char[] SpecialChars =
        ['_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'];

    public static string Escape(string text, params char[] allowedChars)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        foreach (var ch in SpecialChars.Except(allowedChars))
        {
            text = text.Replace(ch.ToString(), "\\" + ch);
        }

        return text;
    }
}
