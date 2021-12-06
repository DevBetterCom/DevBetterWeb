using System.Collections;
using System.Collections.Generic;

namespace DevBetterWeb.Core;

public class AppendOnlyStringList : IEnumerable<string>
{
  private List<string> _messages = new();

  public IEnumerator<string> GetEnumerator()
  {
    return _messages.GetEnumerator();
  }

  public void Append(string message)
  {
    _messages.Add(message);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return _messages.GetEnumerator();
  }
}
