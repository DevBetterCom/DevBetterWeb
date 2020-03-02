using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevBetterWeb.Web
{
    public class ValidUrlAttribute : ValidationAttribute
    {
        public string GetErrorMessage() =>
        $"You must enter a valid URL";

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

            if (isValueUrl)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(GetErrorMessage());
        }


    }
}
