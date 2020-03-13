using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using System;
using System.Linq;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberUpdateLinks
    {
        private string _initialBlogUrl = "";
        private string _initialGitHubUrl = "";
        private string _initialLinkedInUrl = "";
        private string _initialOtherUrl = "";
        private string _initialTwitchUrl = "";
        private string _initialTwitterUrl = "";

        private Member GetMemberWithDefaultLinks()
        {
            _initialBlogUrl = Guid.NewGuid().ToString();

            var member = MemberHelpers.CreateWithDefaultConstructor();
            member.UpdateLinks(_initialBlogUrl,
                _initialGitHubUrl,
                _initialLinkedInUrl,
                _initialOtherUrl,
                _initialTwitchUrl,
                _initialTwitterUrl);
            member.Events.Clear();

            return member;
        }

        [Fact]
        public void SetsBlogLink()
        {
            string newLink = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultLinks();
            member.UpdateLinks(newLink,
                _initialGitHubUrl,
                _initialLinkedInUrl,
                _initialOtherUrl,
                _initialTwitchUrl,
                _initialTwitterUrl);

            Assert.Equal(newLink, member.BlogUrl);
        }

        [Fact]
        public void RecordsEventIfLinksChanges()
        {
            string newLink = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultLinks();
            member.UpdateLinks(newLink,
                _initialGitHubUrl,
                _initialLinkedInUrl,
                _initialOtherUrl,
                _initialTwitchUrl,
                _initialTwitterUrl);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("Links", eventCreated.UpdateDetails);
        }

        [Fact]
        public void RecordsNoEventIfLinksDoesNotChange()
        {
            var member = GetMemberWithDefaultLinks();
            member.UpdateLinks(_initialBlogUrl,
                _initialGitHubUrl,
                _initialLinkedInUrl,
                _initialOtherUrl,
                _initialTwitchUrl,
                _initialTwitterUrl);

            Assert.Empty(member.Events);
        }

        [Fact]
        public void RecordsEventWithAppendedDetailsIfOtherThingsChanged()
        {
            string newLink = Guid.NewGuid().ToString();

            var member = GetMemberWithDefaultLinks();
            member.UpdateName("kylo", "ren");
            member.UpdateAboutInfo("About kylo");
            member.UpdateAddress("123 main street");
            member.UpdateLinks(newLink,
                _initialGitHubUrl,
                _initialLinkedInUrl,
                _initialOtherUrl,
                _initialTwitchUrl,
                _initialTwitterUrl);
            var eventCreated = (MemberUpdatedEvent)member.Events.First();

            Assert.Same(member, eventCreated.Member);
            Assert.Equal("Name,AboutInfo,Address,Links", eventCreated.UpdateDetails);
            Assert.Single(member.Events);
        }

    }
}
