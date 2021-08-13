using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Ardalis.ApiCaller
{
  public class QueryBuilder
  {
    public NameValueCollection Query { get; }

    public QueryBuilder()
    {
      Query = HttpUtility.ParseQueryString(string.Empty);
    }

    public QueryBuilder Add(string key, string value)
    {
      Query[key] = value;

      return this;
    }

    public string Build()
    {
      return Query?.ToString();
    }

    public List<KeyValuePair<string, string>> ToListKeyValuePair()
    {
      var result = new List<KeyValuePair<string, string>>();
      foreach (var item in Query.AllKeys.SelectMany(Query.GetValues, (k, v) => new { key = k, value = v }))
      {
        result.Add(new KeyValuePair<string, string>(item.key, item.value));
      }

      return result;
    }
  }
}
