using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace DevBetterWeb.Vimeo.Models
{

  public class AccessGrant
  {
    [JsonPropertyName("permission_policy")]
    public List<PermissionPolicy> PermissionPolicy { get; set; }
  }

}
