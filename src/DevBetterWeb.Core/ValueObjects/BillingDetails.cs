using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DevBetterWeb.Core.ValueObjects
{
  public class BillingDetails : ValueObject
  {
    public string Message { get; }
    public decimal Amount { get; }

    public BillingDetails(string message, decimal amount = 0)
    {
      Message = message;
      Amount = amount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Amount;
      yield return Message;
    }
  }
}
