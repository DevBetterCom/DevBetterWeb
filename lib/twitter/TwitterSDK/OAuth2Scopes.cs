using Ardalis.SmartEnum;

namespace TwitterSDK.Examples;

public sealed class OAuth2Scope : SmartEnum<OAuth2Scope>
{
	public static readonly OAuth2Scope TweetRead = new OAuth2Scope("tweet.read", 1);
	public static readonly OAuth2Scope TweetWrite = new OAuth2Scope("tweet.write", 2);
	public static readonly OAuth2Scope TweetModerateWrite = new OAuth2Scope("tweet.moderate.write", 3);
	public static readonly OAuth2Scope UsersRead = new OAuth2Scope("users.read", 4);
	public static readonly OAuth2Scope FollowsRead = new OAuth2Scope("follows.read", 5);
	public static readonly OAuth2Scope FollowsWrite = new OAuth2Scope("follows.write", 6);
	public static readonly OAuth2Scope OfflineAccess = new OAuth2Scope("offline.access", 7);
	public static readonly OAuth2Scope SpaceRead = new OAuth2Scope("space.read", 8);
	public static readonly OAuth2Scope MuteRead = new OAuth2Scope("mute.read", 9);
	public static readonly OAuth2Scope MuteWrite = new OAuth2Scope("mute.write", 10);
	public static readonly OAuth2Scope LikeRead = new OAuth2Scope("like.read", 11);
	public static readonly OAuth2Scope LikeWrite = new OAuth2Scope("like.write", 12);
	public static readonly OAuth2Scope ListRead = new OAuth2Scope("list.read", 13);
	public static readonly OAuth2Scope ListWrite = new OAuth2Scope("list.write", 14);
	public static readonly OAuth2Scope BlockRead = new OAuth2Scope("block.read", 15);
	public static readonly OAuth2Scope BlockWrite = new OAuth2Scope("block.write", 16);
	public static readonly OAuth2Scope BookmarkRead = new OAuth2Scope("bookmark.read", 17);
	public static readonly OAuth2Scope BookmarkWrite = new OAuth2Scope("bookmark.write", 18);

	private OAuth2Scope(string name, int value): base(name, value) { }
}
