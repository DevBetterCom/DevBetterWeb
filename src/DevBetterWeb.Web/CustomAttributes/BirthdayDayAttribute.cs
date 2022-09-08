using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Web.Pages.User;

namespace DevBetterWeb.Web;
public class BirthdayDayAttribute : ValidationAttribute
{
	public BirthdayDayAttribute()
	{

	}


	public string GetErrorMessage() =>
			$"A birthday in February can't have a date of more than 29.";

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
	protected override ValidationResult IsValid(object value,
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
			ValidationContext validationContext)
	{
		var model = (UserPersonalUpdateModel)validationContext.ObjectInstance;

		if (model.BirthdayMonth == 2 && (int)value > 29)
		{
			return new ValidationResult(GetErrorMessage());
		}

		return ValidationResult.Success;
	}
}