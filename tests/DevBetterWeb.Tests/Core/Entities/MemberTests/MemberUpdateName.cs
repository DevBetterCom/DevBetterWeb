using System;
using Xunit;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public class MemberUpdateName
    {
        [Fact]
        public void SetsFirstNameAndLastName()
        {
            string newFirstName = Guid.NewGuid().ToString();
            string newLastName = Guid.NewGuid().ToString();

            var member = MemberHelpers.CreateWithDefaultConstructor();

            member.UpdateName(newFirstName, newLastName);

            Assert.Equal(newFirstName, member.FirstName);
            Assert.Equal(newLastName, member.LastName);
        }
    }
}
