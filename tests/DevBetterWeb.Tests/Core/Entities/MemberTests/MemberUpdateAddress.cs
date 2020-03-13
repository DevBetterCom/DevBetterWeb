using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using System;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberUpdateAddress
    {
        private string _initialAddress = "";

        private Member GetMemberWithDefaultAddress()
        {
            _initialAddress = Guid.NewGuid().ToString();

            var member = MemberHelpers.CreateWithDefaultConstructor();
            member.UpdateAddress(_initialAddress);
            member.Events.Clear();

            return member;
        }

        [Fact]
        public void SetsAddress()
        {
            string newAddress = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAddress();
            member.UpdateAddress(newAddress);

            Assert.Equal(newAddress, member.Address);
        }

        [Fact]
        public void RecordsEventIfAddressChanges()
        {
            string newAddress = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAddress();
            member.UpdateAddress(newAddress);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("Address", eventCreated.UpdateDetails);
        }

        [Fact]
        public void RecordsNoEventIfAddressDoesNotChange()
        {
            var member = GetMemberWithDefaultAddress();
            member.UpdateAddress(_initialAddress);

            Assert.Empty(member.Events);
        }

        [Fact]
        public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
        {
            string newAddress = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultAddress();
            member.UpdateName("kylo", "ren");
            member.UpdateAddress(newAddress);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("Name,Address", eventCreated.UpdateDetails);
        }
    }
}
