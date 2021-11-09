using System;

namespace DevBetterWeb.Vimeo.Helpers;

public class RandomHelper
{
  private readonly Random _random = new Random();

  public int CreateNumber(int min, int max)
  {
    return _random.Next(min, max);
  }
}
