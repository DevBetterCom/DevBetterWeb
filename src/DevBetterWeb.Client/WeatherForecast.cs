using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Client;
public class WeatherForecast
{
  public DateTime Date { get; set; }

  public int TemperatureC { get; set; }

  public string? Summary { get; set; }

  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
