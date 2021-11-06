using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Interactions
{
  [JsonPropertyName("follow")]
  public Follow Follow { get; set; }

  [JsonPropertyName("album")]
  public Album Album { get; set; }

  [JsonPropertyName("buy")]
  public Buy Buy { get; set; }

  [JsonPropertyName("channel")]
  public Channel Channel { get; set; }

  [JsonPropertyName("delete")]
  public Delete Delete { get; set; }

  [JsonPropertyName("edit")]
  public Edit Edit { get; set; }

  [JsonPropertyName("like")]
  public Like Like { get; set; }

  [JsonPropertyName("rent")]
  public Rent Rent { get; set; }

  [JsonPropertyName("report")]
  public Report Report { get; set; }

  [JsonPropertyName("subscribe")]
  public Subscribe Subscribe { get; set; }

  [JsonPropertyName("view_team_members")]
  public ViewTeamMembers ViewTeamMembers { get; set; }

  [JsonPropertyName("watched")]
  public Watched Watched { get; set; }

  [JsonPropertyName("watchlater")]
  public Watchlater Watchlater { get; set; }

  [JsonPropertyName("add_subfolder")]
  public AddSubfolder AddSubfolder { get; set; }

  [JsonPropertyName("interactions")]
  public Interactions InteractionsData { get; set; }

  [JsonPropertyName("add_privacy_user")]
  public AddPrivacyUser AddPrivacyUser { get; set; }

  [JsonPropertyName("block")]
  public Block Block { get; set; }

  [JsonPropertyName("connected_apps")]
  public ConnectedApps ConnectedApps { get; set; }
}
