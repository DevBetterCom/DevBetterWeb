using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web
{
  public class ValidTwitchUrlAttribute : ValidationAttribute
  {
    public string GetErrorMessage() =>
    $"You must enter a valid Twitch URL";

    protected override ValidationResult IsValid(object value,
        ValidationContext validationContext)
    {
      if (value == null)
      {
        return ValidationResult.Success;
      }

      var url = value.ToString();

      if (string.IsNullOrEmpty(url))
      {
        return ValidationResult.Success;
      }


      var isValueUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);

      var isTwitchUrl = url.ToLower().Contains("twitch");

      if (isValueUrl && isTwitchUrl)
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(GetErrorMessage());
    }


  }
}
