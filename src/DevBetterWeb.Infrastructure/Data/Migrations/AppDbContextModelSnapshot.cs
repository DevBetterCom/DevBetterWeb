﻿// <auto-generated />
using System;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("BookMember", b =>
                {
                    b.Property<int>("BooksReadId")
                        .HasColumnType("int");

                    b.Property<int>("MembersWhoHaveReadId")
                        .HasColumnType("int");

                    b.HasKey("BooksReadId", "MembersWhoHaveReadId");

                    b.HasIndex("MembersWhoHaveReadId");

                    b.ToTable("BookMember");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.ArchiveVideo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ShowNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("VideoUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("ArchiveVideos");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.BillingActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("MemberId")
                        .HasMaxLength(500)
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("BillingActivities");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Author")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Details")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("PurchaseUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Title")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("InviteCode")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PaymentHandlerSubscriptionId")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AboutInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("BlogUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<decimal?>("CityLatitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("CityLongitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CodinGameUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("DiscordUsername")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GitHubUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LinkedInUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("OtherUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PEFriendCode")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PEUsername")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TwitchUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("TwitterUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("YouTubeUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionPlanId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ArchiveVideoId")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("TimestampSeconds")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ArchiveVideoId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.SubscriptionPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.HasKey("Id");

                    b.ToTable("SubscriptionPlan");
                });

            modelBuilder.Entity("BookMember", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.Book", null)
                        .WithMany()
                        .HasForeignKey("BooksReadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.Member", null)
                        .WithMany()
                        .HasForeignKey("MembersWhoHaveReadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.BillingActivity", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.Member", null)
                        .WithMany("BillingActivities")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.BillingDetails", "Details", b1 =>
                        {
                            b1.Property<int>("BillingActivityId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .UseIdentityColumn();

                            b1.Property<string>("ActionVerbPastTense")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<decimal>("Amount")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("decimal(18,2)")
                                .HasDefaultValue(0m);

                            b1.Property<int>("BillingPeriod")
                                .HasMaxLength(100)
                                .HasColumnType("int");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.Property<string>("MemberName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<string>("SubscriptionPlanName")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.HasKey("BillingActivityId");

                            b1.ToTable("BillingActivities");

                            b1.WithOwner()
                                .HasForeignKey("BillingActivityId");
                        });

                    b.Navigation("Details")
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Member", b =>
                {
                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.Address", "ShippingAddress", b1 =>
                        {
                            b1.Property<int>("MemberId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .UseIdentityColumn();

                            b1.Property<string>("City")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(12)
                                .HasColumnType("nvarchar(12)")
                                .HasDefaultValue("");

                            b1.Property<string>("State")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)")
                                .HasDefaultValue("");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .ValueGeneratedOnAdd()
                                .HasMaxLength(500)
                                .HasColumnType("nvarchar(500)")
                                .HasDefaultValue("");

                            b1.HasKey("MemberId");

                            b1.ToTable("Members");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.Geolocation", "CityLocation", b1 =>
                        {
                            b1.Property<int>("MemberId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .UseIdentityColumn();

                            b1.Property<decimal>("Latitude")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal>("Longitude")
                                .HasColumnType("decimal(18,2)");

                            b1.HasKey("MemberId");

                            b1.ToTable("Members");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.Navigation("CityLocation");

                    b.Navigation("ShippingAddress");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberSubscription", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.Member", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.DateTimeRange", "Dates", b1 =>
                        {
                            b1.Property<int>("MemberSubscriptionId")
                                .HasColumnType("int");

                            b1.Property<DateTime?>("EndDate")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("StartDate")
                                .HasColumnType("datetime2");

                            b1.HasKey("MemberSubscriptionId");

                            b1.ToTable("SubscriptionDates");

                            b1.WithOwner()
                                .HasForeignKey("MemberSubscriptionId");
                        });

                    b.Navigation("Dates")
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Question", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.ArchiveVideo", null)
                        .WithMany("Questions")
                        .HasForeignKey("ArchiveVideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.SubscriptionPlan", b =>
                {
                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.SubscriptionPlanDetails", "Details", b1 =>
                        {
                            b1.Property<int>("SubscriptionPlanId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .UseIdentityColumn();

                            b1.Property<int>("BillingPeriod")
                                .HasMaxLength(100)
                                .HasColumnType("int");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("nvarchar(100)");

                            b1.Property<decimal>("PricePerBillingPeriod")
                                .HasMaxLength(100)
                                .HasColumnType("decimal(18,2)");

                            b1.HasKey("SubscriptionPlanId");

                            b1.ToTable("SubscriptionPlan");

                            b1.WithOwner()
                                .HasForeignKey("SubscriptionPlanId");
                        });

                    b.Navigation("Details");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.ArchiveVideo", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Member", b =>
                {
                    b.Navigation("BillingActivities");

                    b.Navigation("Subscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
