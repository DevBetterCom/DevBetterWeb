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

      var containsInputString = url.ToLower().Contains(_inputString.ToLower());

      if (isValueUrl && containsInputString)
      {
        return ValidationResult.Success;
      }

      return new ValidationResult(GetErrorMessage());
    }


  }
}
