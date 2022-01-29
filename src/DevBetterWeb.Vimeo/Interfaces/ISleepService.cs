using System;

namespace DevBetterWeb.Vimeo.Interfaces;

public interface ISleepService
{
  void Sleep(int milliseconds);
  void Sleep(TimeSpan timeout);
}
