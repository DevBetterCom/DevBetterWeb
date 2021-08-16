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

    public bool IsEmpty()
    {
      if(Query.Count == 0)
      {
        return true;
      }

      return false;
    }

    public QueryBuilder Add(string key, object value)
    {
      if (!string.IsNullOrEmpty(value?.ToString()))
      {
        Query[key] = value.ToString();
      }      

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
