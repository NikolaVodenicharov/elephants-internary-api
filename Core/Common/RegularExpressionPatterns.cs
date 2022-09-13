namespace Core.Common
{
    public sealed class RegularExpressionPatterns
    {
        // Some.#!
        public const string LettersDotsNumberSignsAndExclamationMarks = "^[a-zA-Z.!#]+$";

        // Some9 0-
        public const string CampaignNamePattern = $"^[a-zA-Z0-9]+([-][a-zA-Z0-9]+|[ ][a-zA-Z0-9]+)*$";
    }
}