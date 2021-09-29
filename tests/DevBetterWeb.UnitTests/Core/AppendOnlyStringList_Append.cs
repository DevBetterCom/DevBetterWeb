using DevBetterWeb.Core;
using Xunit;

namespace DevBetterWeb.UnitTests.Core
{
  public class AppendOnlyStringList_Append
  {
    [Fact]
    public void AppendsStringsToList()
    {
      var list = new AppendOnlyStringList();
      var testString = "I am a test";
      var anotherTestString = "Look! Another test string!";

      list.Append(testString);
      list.Append(anotherTestString);

      Assert.Contains(testString, list);
      Assert.Contains(anotherTestString, list);
    }
  }
}
