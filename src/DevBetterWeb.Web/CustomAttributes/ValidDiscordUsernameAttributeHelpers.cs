namespace DevBetterWeb.Web;

public class ValidDiscordUsernameAttributeHelpers
{
  public static bool HasFourDigitNumAtEnd(string username)
  {
    string lastFour = username.Substring(username.Length - 4);
    return int.TryParse(lastFour, out _);
  }

  public static bool HasHashtagFourFromEnd(string username)
  {
    string hopefullyHashtag = username.Substring(username.Length - 5, 1);

    return hopefullyHashtag.Equals("#");
  }
}
