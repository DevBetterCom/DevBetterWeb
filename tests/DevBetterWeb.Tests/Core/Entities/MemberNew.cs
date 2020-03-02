using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities
{
    public class MemberNew
    {
        private const string TEST_USER_ID = "TestUserId";

        private Member CreateWithDefaultConstructor()
        {
            return (Member)Activator.CreateInstance(typeof(Member), true);
        }

        private Member CreateWithInternalConstructor()
        {
            var constructor = typeof(Member).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[1] { typeof(System.String) }, null);
            return (Member)constructor.Invoke(new[] { TEST_USER_ID });
        }

        [Fact]
        public void IsPrivateForDefaultConstructor()
        {
            var member = CreateWithDefaultConstructor();

            Assert.NotNull(member);
        }

        [Fact]
        public void CreatesNoEventsWithDefaultConstructor()
        {
            var member = CreateWithDefaultConstructor();

            Assert.Empty(member.Events);
        }

        [Fact]
        public void RequiresUserIdForInternalUsage()
        {
            var member = CreateWithInternalConstructor();

            Assert.Equal(TEST_USER_ID, member.UserId);
        }

        [Fact]
        public void WithUserIdRecordsNewMemberCreatedEvent()
        {
            var member = CreateWithInternalConstructor();

            var eventCreated = member.Events.FirstOrDefault() as NewMemberCreatedEvent;

            Assert.Same(member, eventCreated.Member);
        }
    }
}
