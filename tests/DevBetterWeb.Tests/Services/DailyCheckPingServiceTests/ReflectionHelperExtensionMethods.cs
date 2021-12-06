using System;
using System.Reflection;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests;

public static class ReflectionHelperExtensionMethods
{
  // from https://stackoverflow.com/a/1565766/13680266
  public static void SetPrivateDateTimePropertyValue(this Invitation invitation, string propName, DateTime newValue)
  {
    PropertyInfo? propertyInfo = typeof(Invitation).GetProperty(propName);
    if (propertyInfo == null) return;
    propertyInfo.SetValue(invitation, newValue);
  }
}
