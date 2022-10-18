namespace Core.Common
{
    public sealed class RegularExpressionPatterns
    {
        public const string CampaignNamePattern = $"^[a-zA-Z0-9]+([-][a-zA-Z0-9]+|[ ][a-zA-Z0-9]+)*$";
        public const string SpecialityNamePattern = @"^[a-zA-Z.!#]+([\s]{1}[a-zA-Z.!#]+)*$";
        public const string EmailPattern = @"^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})$";
        public const string PersonNamesPattern = @"^([a-zA-Z]+([-\s]{1}[a-zA-Z]+)*)$";
        public const string LearningTopicNamePattern = @"^([a-zA-Z]+([-\s]{1}[a-zA-Z]+)*)$";
        public const string ValidationErrorMessageSplitPattern = @"Validation failed:|-- [a-zA-Z]+: |Severity: Error";
    }
}