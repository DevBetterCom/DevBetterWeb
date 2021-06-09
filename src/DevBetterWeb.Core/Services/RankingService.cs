using System;
using System.Collections.Generic;
using System.Linq;

namespace DevBetterWeb.Core.Services
{
  public class RankingService<TKey> where TKey: notnull
  {
    public Dictionary<TKey, int> Rank(IEnumerable<TKey> items)
    {
      var result = new Dictionary<TKey, int>();

      var counts = items
        .Distinct()
        .OrderByDescending(i=>i);

      var rank = 1;
      foreach (var count in counts)
      {
        result[count] = rank;
        rank += 1;
      }

      return result;
    }
  }
}
