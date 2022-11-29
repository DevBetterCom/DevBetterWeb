using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DevBetterWeb.Web.Pages.User;

namespace DevBetterWeb.Web;
public class BirthdayDayAttribute : ValidationAttribute
{

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
	protected override ValidationResult IsValid(object value,
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
			ValidationContext validationContext)
	{
		var model = (UserPersonalUpdateModel)validationContext.ObjectInstance;

		if (value is null) return ValidationResult.Success!;
		if (model.BirthdayMonth is null) return ValidationResult.Success!;

		try
		{
			const int LEAP_YEAR = 2020;
			new DateOnly(LEAP_YEAR, model.BirthdayMonth.Value, (int)value);
			return ValidationResult.Success;
		}
		catch (ArgumentOutOfRangeException)
		{
			string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.BirthdayMonth.Value);
			return new ValidationResult($"Invalid day for month of {monthName}");
		}
	}
}
