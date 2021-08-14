using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Models.Vimeo
{
  public class UserModel
  {
    public string Account { get; set; }
    public bool AvailableForHire { get; set; }
    public string Bio { get; set; }
    public bool CanWorkRemotely { get; set; }
    public string Clients { get; set; }
    public List<string> ContentFilter { get; set; }
    public DateTime CreatedTime { get; set; }
    public string Gender { get; set; }
    public string Link { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public PicturesModel Pictures { get; set; }
    public string ResourceKey { get; set; }
    public string ShortBio { get; set; }
    public string Uri { get; set; }
  }
}
