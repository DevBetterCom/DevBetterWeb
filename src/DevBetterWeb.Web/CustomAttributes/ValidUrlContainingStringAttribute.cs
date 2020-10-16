using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web
{
  public class ValidUrlContainingStringAttribute : ValidationAttribute
  {

    public string _inputString { get; set; }

    public ValidUrlContainingStringAttribute(string inputString)
    {
      _inputString = inputString;
    }

    public string GetErrorMessage() =>
    $"You must enter a valid " + _inputString + " URL";

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

      var containsInputString = url.ToLower().Contains(_inputString.ToLower());

      if (isValueUrl && containsInputString)
      {
        return ValidationResult.Success!;
      }

      return new ValidationResult(GetErrorMessage());
    }


  }
}
