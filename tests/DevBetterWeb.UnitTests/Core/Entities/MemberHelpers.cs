using DevBetterWeb.Core.Entities;
using System;
using System.Reflection;

namespace DevBetterWeb.UnitTests.Core.Entities
{
    public static class MemberHelpers
    {
        public const string TEST_USER_ID = "TestUserId";

#nullable disable
        public static Member CreateWithDefaultConstructor()
        {
            return (Member)Activator.CreateInstance(typeof(Member), true);
        }

        public static Member CreateWithInternalConstructor()
        {
            var constructor = typeof(Member).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[1] { typeof(System.String) }, null);
            return (Member)constructor.Invoke(new[] { TEST_USER_ID });
        }
    }
}
