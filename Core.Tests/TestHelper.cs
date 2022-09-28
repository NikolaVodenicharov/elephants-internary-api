using System.Text;

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
    }
}
