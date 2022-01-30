using System;
using System.Threading;
using DevBetterWeb.Vimeo.Interfaces;

namespace DevBetterWeb.Vimeo.Services.UtilityServices;

public class SleepService : ISleepService
{
  public void Sleep(int milliseconds)
  {
    Thread.Sleep(milliseconds);
  }

  public void Sleep(TimeSpan timeout)
  {
    Thread.Sleep(timeout);
  }
}
