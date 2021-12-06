using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class AccessGrant
{
  [JsonPropertyName("permission_policy")]
  public List<PermissionPolicy> PermissionPolicy { get; set; }
}
