using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using System;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberUpdateAboutInfo
    {
        private string _initialAboutInfo = "";

        private Member GetMemberWithDefaultAboutInfo()
        {
            _initialAboutInfo = Guid.NewGuid().ToString();

            Member? member = MemberHelpers.CreateWithDefaultConstructor();
            member.UpdateAboutInfo(_initialAboutInfo);
            member.Events.Clear();

            return member;
        }

        [Fact]
        public void SetsAboutInfo()
        {
            string newAboutInfo = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAboutInfo();
            member.UpdateAboutInfo(newAboutInfo);

            Assert.Equal(newAboutInfo, member.AboutInfo);
        }

        [Fact]
        public void RecordsEventIfAboutInfoChanges()
        {
            string newAboutInfo = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAboutInfo();
            member.UpdateAboutInfo(newAboutInfo);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("AboutInfo", eventCreated.UpdateDetails);
        }

        [Fact]
        public void RecordsNoEventIfAboutInfoDoesNotChange()
        {
            var member = GetMemberWithDefaultAboutInfo();
            member.UpdateAboutInfo(_initialAboutInfo);

            Assert.Empty(member.Events);
        }

        [Fact]
        public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
        {
            string newAboutInfo = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAboutInfo();
            member.UpdateName("kylo", "ren");
            member.UpdateAboutInfo(newAboutInfo);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("Name,AboutInfo", eventCreated.UpdateDetails);
        }

    }
}
