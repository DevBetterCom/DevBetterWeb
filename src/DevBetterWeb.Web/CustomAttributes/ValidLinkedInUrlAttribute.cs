using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web
{
  public class ValidLinkedInUrlAttribute : ValidationAttribute
  {
    public string GetErrorMessage() =>
    $"You must enter a valid LinkedIn URL";

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

      var isLinkedInUrl = url.ToLower().Contains("linkedin");

      if (isValueUrl && isLinkedInUrl)
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(GetErrorMessage());
    }


  }
}
