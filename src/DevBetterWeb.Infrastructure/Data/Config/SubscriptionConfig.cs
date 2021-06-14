using DevBetterWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Security.Cryptography.X509Certificates;

namespace DevBetterWeb.Infrastructure.Data.Config
{
    class SubscriptionConfig : IEntityTypeConfiguration<MemberSubscription>
    {
        public void Configure(EntityTypeBuilder<MemberSubscription> builder)
        {
            builder.OwnsOne(x => x.Dates)
                   .ToTable("SubscriptionDates");
        }
    }
}
