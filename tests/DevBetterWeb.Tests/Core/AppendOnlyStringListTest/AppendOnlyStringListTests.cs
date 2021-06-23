using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using Xunit;

namespace DevBetterWeb.Tests.Core.AppendOnlyStringListTest
{
  public class AppendOnlyStringListTests
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
