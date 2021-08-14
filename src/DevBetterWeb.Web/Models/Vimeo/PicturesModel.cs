using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Models.Vimeo
{
  public class PicturesModel
  {
    public bool Active { get; set; }
    public bool DefaultPicture { get; set; }
    public string Link { get; set; }
    public string ResourceKey { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
    public List<string> Options { get; set; }
    public int Total { get; set; }
  }
}
