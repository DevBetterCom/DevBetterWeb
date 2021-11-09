using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevBetterWeb.Vimeo.Models;

public class Paging
{
  [JsonPropertyName("next")]
  public object Next { get; set; }

  [JsonPropertyName("previous")]
  public object Previous { get; set; }

  [JsonPropertyName("first")]
  public string First { get; set; }

  [JsonPropertyName("last")]
  public string Last { get; set; }
}
