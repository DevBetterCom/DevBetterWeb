using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Handlers;
using DevBetterWeb.Tests.Core.Entities.MemberTests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DevBetterWeb.Tests.Integration.Web
{
    public class DiscordLogUserUpdateHandlerShould
    {

        [Fact]
        public void ReturnsProperMessageString()
        {
            Member member = MemberHelpers.CreateWithDefaultConstructor();
            MemberUpdatedEvent memberEvent = new MemberUpdatedEvent(member, "Links");

            var output = DiscordLogUserUpdateHandler.returnWebhookMessageString(memberEvent);
            var expected = $"User {memberEvent.Member.FirstName} {memberEvent.Member.LastName} just updated their profile. Check it out here: https://devbetter.com/User/Details/{memberEvent.Member.Id}.";

            Assert.Equal(expected, output);

        }
    }
}
