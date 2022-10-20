using System;
using Core.Features.Interns.Support;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Tests
{
    public class NameEdgeCaseTestHelper
    {
        public const string NameWithDigit = "NameWith1";
        public const string NameWithDot = "NameWith.";
        public const string NameWithDash = "NameWith-";
        public const string NameWithNumberSign = "NameWith#";
        public const string NameWithExclamationMark = "NameWith!";
        public const string NameWithAmpersand = "NameWith&";

        public const string FirstNameMock = "John";
        public const string LastNameMock = "Doe";
        public const string EmailMock = "John.Doe@endava.com";

        public static IEnumerable<object[]> InvalidEmails = new List<object[]>
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

        public static IEnumerable<object[]> ValidEmails = new List<object[]>
        {
            new object[] { "user.example@test.com" },
            new object[] { "first-last@example.co.uk" },
            new object[] { "random123@example.gov.in" },
        };

        public static IEnumerable<object[]> ValidPersonNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMinLength) },
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMaxLength) },
            new object[] { "John" },
            new object[] { "Ana-Maria" },
            new object[] { "Mary Alexandra" },
        };

        public static IEnumerable<object[]> InvalidPersonNames = new List<object[]>
        {
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMinLength - 1) },
            new object[] { TestHelper.GenerateString(InternValidationConstants.InternNameMaxLength + 1) },
            new object[] { "Name1" },
            new object[] { " Name" },
            new object[] { "Name " },
        };
    }
}
