using System.Text;

namespace DevBetterWeb.Vimeo.Tests.Extensions
{
  internal static class StringExtensions
  {
    public static string RemoveSpecialCharacters(this string str)
    {
      var sb = new StringBuilder();
      foreach (char c in str)
      {
        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }
  }
}
