namespace Core.Common
{
    public sealed class RegularExpressionPatterns
    {
        public const string WordsAndWhiteSpacePattern = "^[a-zA-Z\\s]+$";
        public const string WordsDigidsDashesAndWhiteSpacePattern = $"^[A-Za-z0-9-\\s]+$";
    }
}