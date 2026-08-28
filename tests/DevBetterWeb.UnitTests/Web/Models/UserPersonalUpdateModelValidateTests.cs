using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Web.Pages.User;
using Xunit;

namespace DevBetterWeb.UnitTests.Web.Models;

public class UserPersonalUpdateModelValidateTests
{
  private static List<ValidationResult> Validate(UserPersonalUpdateModel model)
  {
    var results = new List<ValidationResult>();
    var context = new ValidationContext(model);
    Validator.TryValidateObject(model, context, results, validateAllProperties: true);
    return results;
  }

  [Fact]
  public void NoAddressFields_NoValidationErrors()
  {
    var model = new UserPersonalUpdateModel
    {
      FirstName = "Jane",
      LastName = "Doe",
      Email = "jane@example.com"
    };

    var errors = Validate(model);

    Assert.Empty(errors);
  }

  [Fact]
  public void AllAddressFieldsProvided_NoValidationErrors()
  {
    var model = new UserPersonalUpdateModel
    {
      FirstName = "Jane",
      LastName = "Doe",
      Address = "123 Main St",
      City = "Springfield",
      Country = "US",
      PostalCode = "12345",
      State = "IL"
    };

    var errors = Validate(model);

    Assert.Empty(errors);
  }

  [Fact]
  public void OnlyCityProvided_ValidationErrorsForOtherAddressFields()
  {
    var model = new UserPersonalUpdateModel
    {
      FirstName = "Jane",
      LastName = "Doe",
      City = "Springfield"
    };

    var errors = Validate(model);

    Assert.Contains(errors, e => e.MemberNames != null && System.Linq.Enumerable.Contains(e.MemberNames, nameof(UserPersonalUpdateModel.Address)));
    Assert.Contains(errors, e => e.MemberNames != null && System.Linq.Enumerable.Contains(e.MemberNames, nameof(UserPersonalUpdateModel.Country)));
    Assert.Contains(errors, e => e.MemberNames != null && System.Linq.Enumerable.Contains(e.MemberNames, nameof(UserPersonalUpdateModel.PostalCode)));
  }

  [Fact]
  public void HasAnyAddressField_ReturnsFalseWhenAllEmpty()
  {
    var model = new UserPersonalUpdateModel();

    Assert.False(model.HasAnyAddressField());
  }

  [Fact]
  public void HasAnyAddressField_ReturnsTrueWhenAddressSet()
  {
    var model = new UserPersonalUpdateModel { Address = "123 Main St" };

    Assert.True(model.HasAnyAddressField());
  }

  [Fact]
  public void HasAnyAddressField_ReturnsTrueWhenOnlyCitySet()
  {
    var model = new UserPersonalUpdateModel { City = "Springfield" };

    Assert.True(model.HasAnyAddressField());
  }

  [Fact]
  public void HasAnyAddressField_ReturnsFalseForWhitespaceOnly()
  {
    var model = new UserPersonalUpdateModel { Address = "   ", City = "  " };

    Assert.False(model.HasAnyAddressField());
  }
}
