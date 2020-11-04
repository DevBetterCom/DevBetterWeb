﻿using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DevBetterWeb.Web
{
  public class ValidDiscordUsernameAttribute : ValidationAttribute
  {
    public string GetErrorMessage() =>
    $"You must enter a valid Discord username with 4-digit tag";

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

      if (HasHashtagFourFromEnd(username) && HasFourDigitNumAtEnd(username))
      {
        return ValidationResult.Success!;
      }

      return new ValidationResult(GetErrorMessage());
    }

    private static bool HasFourDigitNumAtEnd(string username)
    {
      string lastFour = username.Substring(username.Length - 4);
      return int.TryParse(lastFour, out _);
    }

    private static bool HasHashtagFourFromEnd(string username)
    {
      string hopefullyHashtag = username.Substring(username.Length - 5, 1);

      return hopefullyHashtag.Equals("#");
    }
  }
}
