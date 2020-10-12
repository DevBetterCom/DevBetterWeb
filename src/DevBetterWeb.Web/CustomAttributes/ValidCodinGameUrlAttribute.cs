using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web
{
  public class ValidCodinGameUrlAttribute : ValidationAttribute
  {
    public string GetErrorMessage() =>
    $"You must enter a valid CodinGame URL";

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

      var isCodinGameUrl = url.ToLower().Contains("codingame");

      if (isValueUrl && isCodinGameUrl)
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(GetErrorMessage());
    }


  }
}
