namespace Core.Common
{
    public sealed class RegularExpressionPatterns
    {
        // Some.#!
        public const string LettersDotsNumberSignsAndExclamationMarks = "^[a-zA-Z.!#]+$";

        // Some9 0-
        public const string CampaignNamePattern = $"^[a-zA-Z0-9]+([-][a-zA-Z0-9]+|[ ][a-zA-Z0-9]+)*$";
        public const string EmailPattern = @"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})$";
        public const string PersonNamesPattern = @"^([a-zA-Z]+([-\s]{1}[a-zA-Z]+)*)$";

        public const string LearningTopicNamePattern = $"^[a-zA-Z0-9]+([-][a-zA-Z0-9]+|[ ][a-zA-Z0-9]+)*$";
    }
}