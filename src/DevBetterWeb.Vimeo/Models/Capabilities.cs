using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevBetterWeb.Vimeo.Models
{
  public class Capabilities
  {
    [JsonPropertyName("hasLiveSubscription")]
    public bool HasLiveSubscription { get; set; }
    [JsonPropertyName("hasEnterpriseLihp")]
    public bool HasEnterpriseLihp { get; set; }
    [JsonPropertyName("hasSvvTimecodedComments")]
    public bool HasSvvTimeCodedComments { get; set; }
  }
}
