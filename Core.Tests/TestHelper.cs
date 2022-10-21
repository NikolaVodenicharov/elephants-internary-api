using System.Text;
using System.Collections.Generic;

namespace Core.Tests
{
    public sealed class TestHelper
    {
        public static string GenerateString(int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append("a");
            }

            return sb.ToString();
        }

        public const string DisplayNameMock = "John Doe";
        public const string EmailMock = "John.Doe@endava.com";
        public const string ApplicationUrlMock = "http://backofficemock";

        public static readonly IEnumerable<object[]> InvalidEmails = new List<object[]>
        {
            new object[] { "k.c.a" },
            new object[] { ".invalid@example.c" },
            new object[] { "invalid@example..com" },
            new object[] { "invalid@example.com." },
            new object[] { "invalidexample" },
            new object[] { "invalidexample.com" },
            new object[] { "invalidexample.co.uk." },
            new object[] { "invalidexample.co_uk" },
            new object[] { "invalidexample.co_ukkkk" },
        };

        public static readonly IEnumerable<object[]> ValidEmails = new List<object[]>
        {
            new object[] { "user.example@test.com" },
            new object[] { "first-last@example.co.uk" },
            new object[] { "random123@example.gov.in" },
        };
    }
}
