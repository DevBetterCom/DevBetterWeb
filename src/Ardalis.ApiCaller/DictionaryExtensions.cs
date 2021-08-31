using System.Collections.Generic;

namespace Ardalis.ApiCaller
{
  public static class DictionaryExtensions
  {
    public static void AddIfNotNull<T, T2>(this Dictionary<T, T2> dictionary, T key, T2 value)
    {
      if (value != null) 
      {
        dictionary.Add(key, value); 
      }
    }
  }
}
