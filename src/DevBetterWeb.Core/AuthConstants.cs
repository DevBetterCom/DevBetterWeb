namespace DevBetterWeb.Core;

public class AuthConstants
{
  public static class Roles
  {
    public const string ADMINISTRATORS = "Administrators";
    public const string MEMBERS = "Members";
    public const string ALUMNI = "Alumni";

    public const string ADMINISTRATORS_MEMBERS = "Administrators,Members";
    public const string ADMINISTRATORS_MEMBERS_ALUMNI = "Administrators,Members,Alumni";
    public const string MEMBERS_ALUMNI = "Members,Alumni";
  }

  public const string DEFAULT_PASSWORD = "Pass@word1";
}
