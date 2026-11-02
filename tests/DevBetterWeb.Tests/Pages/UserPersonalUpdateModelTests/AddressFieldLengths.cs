using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Web.Pages.User;
using Xunit;

namespace DevBetterWeb.Tests.Pages.UserPersonalUpdateModelTests;

public class AddressFieldLengths
{
  private static UserPersonalUpdateModel ValidModel() => new()
  {
    FirstName = "Kajan",
    LastName = "Smith",
    Address = new string('a', 500),
    City = new string('a', 100),
    State = new string('a', 100),
    Country = new string('a', 100),
    PostalCode = new string('a', 12)
  };

  private static List<ValidationResult> Validate(UserPersonalUpdateModel model)
  {
    var results = new List<ValidationResult>();
    Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
    return results;
  }

  [Fact]
  public void AcceptsValuesAtTheColumnWidths()
  {
    Assert.Empty(Validate(ValidModel()));
  }

  [Theory]
  [InlineData(nameof(UserPersonalUpdateModel.Address), 501)]
  [InlineData(nameof(UserPersonalUpdateModel.City), 101)]
  [InlineData(nameof(UserPersonalUpdateModel.State), 101)]
  [InlineData(nameof(UserPersonalUpdateModel.Country), 101)]
  [InlineData(nameof(UserPersonalUpdateModel.PostalCode), 13)]
  public void RejectsValuesOverTheColumnWidths(string field, int length)
  {
    var model = ValidModel();
    typeof(UserPersonalUpdateModel).GetProperty(field)!.SetValue(model, new string('a', length));

    var error = Assert.Single(Validate(model));

    Assert.Contains(field, error.MemberNames);
  }
}
