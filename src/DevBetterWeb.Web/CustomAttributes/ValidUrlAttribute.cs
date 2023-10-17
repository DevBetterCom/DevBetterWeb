using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web;

//TODO get this to work without being commented out
public class ValidUrlAttribute : ValidationAttribute
{
  public string GetErrorMessage() =>
  $"You must enter a valid URL";

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
  protected override ValidationResult IsValid(object value,
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
            ValidationContext validationContext)
  {
    if (value == null)
    {
      return ValidationResult.Success!;
    }

    var url = value.ToString();

    if (string.IsNullOrEmpty(url))
    {
      return ValidationResult.Success!;
    }


    var isValueUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri);

    if (isValueUrl)
    {
      return ValidationResult.Success!;
    }

    return new ValidationResult(GetErrorMessage());
  }


}
