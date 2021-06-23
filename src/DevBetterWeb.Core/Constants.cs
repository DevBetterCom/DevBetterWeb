namespace DevBetterWeb.Core
{
  public static class Constants
  {
    public const string AVATAR_IMGURL_FORMAT_STRING = "https://devbetter.blob.core.windows.net/photos/{0}.jpg";
    public const int MAX_UPLOAD_FILE_SIZE = 1_512_428_800; // 1500 MB
    public const string FILE_DATE_FORMAT_STRING = "yyyy-MM-dd";

    public const string MEMBER_ROLE_NAME = "Members";
    public const string ALUMNI_ROLE_NAME = "Alumni";


    public const string STRIPE_API_ENDPOINT = "api/stripecallback";

    public static class ConfigKeys
    {
      public const string FileStorageConnectionString = "storageconnectionstring";
    }
  }
}
