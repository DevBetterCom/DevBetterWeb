namespace DevBetterWeb.Core;

public class AuthConstants
{
	public static class Users
	{
		public static class Admin
		{
			public const string EMAIL = "admin@test.com";
			public const string FIRST_NAME = "Admin";
			public const string LAST_NAME = "User";
		}

		public static class Demo
		{
			public const string EMAIL = "demouser@microsoft.com";
			public const string FIRST_NAME = "Demo";
			public const string LAST_NAME = "User";
		}

		public static class Demo2
		{
			public const string EMAIL = "demouser2@microsoft.com";
			public const string FIRST_NAME = "Demo2";
			public const string LAST_NAME = "User2";
		}

		public static class Demo3
		{
			public const string EMAIL = "demouser3@microsoft.com";
			public const string FIRST_NAME = "Demo3";
			public const string LAST_NAME = "User3";
		}

		public static class Demo4
		{
			public const string EMAIL = "demouser4@microsoft.com";
			public const string FIRST_NAME = "Demo4";
			public const string LAST_NAME = "User4";
		}

		public static class NonMember
		{
			public const string EMAIL = "non-member@microsoft.com";
			public const string FIRST_NAME = "non-member";
			public const string LAST_NAME = "non-member";
		}

		public static class Alumni
		{
			public const string EMAIL = "alumni@test.com";
			public const string FIRST_NAME = "Alumni";
			public const string LAST_NAME = "User";
		}

		public static class Alumni2
		{
			public const string EMAIL = "alumni2@test.com";
			public const string FIRST_NAME = "Alumni2";
			public const string LAST_NAME = "User2";
		}
	}
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
