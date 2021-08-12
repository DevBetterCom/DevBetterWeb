using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace DevBetterWeb.Vimeo.Models{ 

    public class Connections
    {
        [JsonPropertyName("channels")]
        public Channels Channels { get; set; }

        [JsonPropertyName("groups")]
        public Groups Groups { get; set; }

        [JsonPropertyName("users")]
        public Users Users { get; set; }

        [JsonPropertyName("videos")]
        public Videos Videos { get; set; }

        [JsonPropertyName("albums")]
        public Albums Albums { get; set; }

        [JsonPropertyName("available_albums")]
        public AvailableAlbums AvailableAlbums { get; set; }

        [JsonPropertyName("available_channels")]
        public AvailableChannels AvailableChannels { get; set; }

        [JsonPropertyName("comments")]
        public Comments Comments { get; set; }

        [JsonPropertyName("credits")]
        public Credits Credits { get; set; }

        [JsonPropertyName("likes")]
        public Likes Likes { get; set; }

        [JsonPropertyName("ondemand")]
        public Ondemand Ondemand { get; set; }

        [JsonPropertyName("pictures")]
        public Pictures Pictures { get; set; }

        [JsonPropertyName("playback")]
        public Playback Playback { get; set; }

        [JsonPropertyName("publish_to_social")]
        public PublishToSocial PublishToSocial { get; set; }

        [JsonPropertyName("recommendations")]
        public Recommendations Recommendations { get; set; }

        [JsonPropertyName("related")]
        public Related Related { get; set; }

        [JsonPropertyName("season")]
        public Season Season { get; set; }

        [JsonPropertyName("texttracks")]
        public Texttracks Texttracks { get; set; }

        [JsonPropertyName("trailer")]
        public Trailer Trailer { get; set; }

        [JsonPropertyName("users_with_access")]
        public UsersWithAccess UsersWithAccess { get; set; }

        [JsonPropertyName("versions")]
        public Versions Versions { get; set; }

        [JsonPropertyName("ancestor_path")]
        public List<AncestorPath> AncestorPath { get; set; }

        [JsonPropertyName("folders")]
        public Folders Folders { get; set; }

        [JsonPropertyName("group_folder_grants")]
        public GroupFolderGrants GroupFolderGrants { get; set; }

        [JsonPropertyName("items")]
        public Items Items { get; set; }

        [JsonPropertyName("parent_folder")]
        public ParentFolder ParentFolder { get; set; }

        [JsonPropertyName("user_folder_access_grants")]
        public UserFolderAccessGrants UserFolderAccessGrants { get; set; }

        [JsonPropertyName("appearances")]
        public Appearances Appearances { get; set; }

        [JsonPropertyName("block")]
        public Block Block { get; set; }

        [JsonPropertyName("categories")]
        public Categories Categories { get; set; }

        [JsonPropertyName("connected_apps")]
        public ConnectedApps ConnectedApps { get; set; }

        [JsonPropertyName("feed")]
        public Feed Feed { get; set; }

        [JsonPropertyName("folders_root")]
        public FoldersRoot FoldersRoot { get; set; }

        [JsonPropertyName("followers")]
        public Followers Followers { get; set; }

        [JsonPropertyName("following")]
        public Following Following { get; set; }

        [JsonPropertyName("moderated_channels")]
        public ModeratedChannels ModeratedChannels { get; set; }

        [JsonPropertyName("portfolios")]
        public Portfolios Portfolios { get; set; }

        [JsonPropertyName("recommended_channels")]
        public RecommendedChannels RecommendedChannels { get; set; }

        [JsonPropertyName("recommended_users")]
        public RecommendedUsers RecommendedUsers { get; set; }

        [JsonPropertyName("shared")]
        public Shared Shared { get; set; }

        [JsonPropertyName("watched_videos")]
        public WatchedVideos WatchedVideos { get; set; }

        [JsonPropertyName("watchlater")]
        public Watchlater Watchlater { get; set; }
    }

}