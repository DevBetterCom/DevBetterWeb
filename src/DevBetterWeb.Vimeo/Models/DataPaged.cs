using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevBetterWeb.Vimeo.Models
{
  public class DataPaged<T>
  {
    [JsonPropertyName("total")]
    public int Total { get; set; }
    [JsonPropertyName("page")]
    public int Page { get; set; }
    [JsonPropertyName("per_page")]
    public int PageIndex { get; set; }
    [JsonPropertyName("paging")]
    public Paging Paging { get; set; }
    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
  }
}
