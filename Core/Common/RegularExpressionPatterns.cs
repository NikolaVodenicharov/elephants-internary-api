namespace Core.Common
{
    public sealed class RegularExpressionPatterns
    {
        // Some.#!
        public const string LettersDotsNumberSignsAndExclamationMarks = "^[a-zA-Z.!#]+$";

        // Some9 0-
        public const string WordsDigidsDashesAndWhiteSpacePattern = $"^[A-Za-z0-9-\\s]+$";
    }
}