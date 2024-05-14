using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DevBetterWeb.Web;

public class ValidDiscordUsernameAttribute : ValidationAttribute
{
  public string GetErrorMessage() =>
  $"You must enter a valid Discord username with between 2 and 32 digits. Only Latin characters, numbers, underscore, and period";

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
  protected override ValidationResult IsValid(object value,
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            ValidationContext validationContext)
  {
    if (value == null)
    {
      return ValidationResult.Success!;
    }

    var username = value.ToString();

    if (string.IsNullOrEmpty(username))
    {
      return ValidationResult.Success!;
    }

		return Regex.IsMatch(username, "^[a-z0-9._]{2,32}$")
								&& !username.Contains("..")
						? ValidationResult.Success!
						: new ValidationResult(GetErrorMessage());
	}
}
