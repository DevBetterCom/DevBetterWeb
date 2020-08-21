using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using System;
using System.Linq;
using Xunit;
using static DevBetterWeb.Web.Pages.User.IndexModel;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberLinksDtoFromMemberEntity
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

        [Fact]
        public void ReturnsInputYouTubeUrlIfContainsQuestionMark()
        {
            var member = MemberHelpers.CreateWithDefaultConstructor();

            string youtubeInput = "https://www.youtube.com/ardalis?";

            member.UpdateLinks(null, null, null, null, null, youtubeInput, null);

            MemberLinksDTO dto = MemberLinksDTO.FromMemberEntity(member);

            var result = dto.YouTubeUrl;

            Assert.Equal(youtubeInput, result);
        }

        [Fact]
        public void ReturnsAlteredYouTubeUrlIfContainsNoQuestionMark()
        {
            var member = MemberHelpers.CreateWithDefaultConstructor();

            string youtubeInput = "https://www.youtube.com/ardalis";

            member.UpdateLinks(null, null, null, null, null, youtubeInput, null);

            MemberLinksDTO dto = MemberLinksDTO.FromMemberEntity(member);

            var result = dto.YouTubeUrl;

            Assert.Equal(youtubeInput + "?sub_confirmation=1", result);
        }

    }
}
