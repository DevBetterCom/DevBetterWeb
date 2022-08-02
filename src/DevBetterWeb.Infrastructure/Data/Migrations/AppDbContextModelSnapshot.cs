﻿// <auto-generated />
using System;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DevBetterWeb.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

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
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AnimatedThumbnailUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateUploaded")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("VideoId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VideoUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ArchiveVideos");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.BillingActivity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Author")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("BookCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

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

                    b.HasIndex("BookCategoryId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.BookCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Title")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("BookCategories");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.CoachingSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("CoachingSessions", (string)null);
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.DailyCheck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("TasksCompleted")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("DailyChecks");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfLastAdminPing")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfUserPing")
                        .HasColumnType("datetime2");

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
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

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
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("MemberSubscriptionPlanId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("MemberSubscriptions");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberSubscriptionPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.HasKey("Id");

                    b.ToTable("MemberSubscriptionPlan");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberVideoProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArchiveVideoId")
                        .HasColumnType("int");

                    b.Property<int>("CurrentDuration")
                        .HasColumnType("int");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<string>("VideoWatchedStatus")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("nvarchar(1)")
                        .HasColumnName("VideoWatchedStatus");

                    b.HasKey("Id");

                    b.HasIndex("ArchiveVideoId");

                    b.HasIndex("MemberId");

                    b.ToTable("MembersVideosProgress", (string)null);
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("ArchiveVideoId")
                        .HasColumnType("int");

                    b.Property<int>("CoachingSessionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("Votes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CoachingSessionId");

                    b.HasIndex("MemberId");

                    b.ToTable("Questions", (string)null);
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.QuestionVote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionVote");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.VideoComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Body")
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int?>("ParentCommentId")
                        .HasColumnType("int");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("VideoId");

                    b.ToTable("VideoComments");
                });

            modelBuilder.Entity("DevBetterWeb.Core.ValueObjects.MemberFavoriteArchiveVideo", b =>
                {
                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("ArchiveVideoId")
                        .HasColumnType("int");

                    b.HasKey("MemberId", "ArchiveVideoId");

                    b.HasIndex("ArchiveVideoId");

                    b.ToTable("MemberFavoriteArchiveVideos", (string)null);
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
                                .HasColumnType("int");

                            b1.Property<int>("ActionVerbPastTense")
                                .HasMaxLength(100)
                                .HasColumnType("int");

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

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Book", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.BookCategory", "BookCategory")
                        .WithMany("Books")
                        .HasForeignKey("BookCategoryId");

                    b.Navigation("BookCategory");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Member", b =>
                {
                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.Address", "ShippingAddress", b1 =>
                        {
                            b1.Property<int>("MemberId")
                                .HasColumnType("int");

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
                                .HasColumnType("int");

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
                        .WithMany("MemberSubscriptions")
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

                            b1.ToTable("MemberSubscriptionDates", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("MemberSubscriptionId");
                        });

                    b.Navigation("Dates")
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberSubscriptionPlan", b =>
                {
                    b.OwnsOne("DevBetterWeb.Core.ValueObjects.MemberSubscriptionPlanDetails", "Details", b1 =>
                        {
                            b1.Property<int>("MemberSubscriptionPlanId")
                                .HasColumnType("int");

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

                            b1.HasKey("MemberSubscriptionPlanId");

                            b1.ToTable("MemberSubscriptionPlan");

                            b1.WithOwner()
                                .HasForeignKey("MemberSubscriptionPlanId");
                        });

                    b.Navigation("Details")
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.MemberVideoProgress", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.ArchiveVideo", "Video")
                        .WithMany("MembersVideoProgress")
                        .HasForeignKey("ArchiveVideoId")
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.Member", "Member")
                        .WithMany("MemberVideosProgress")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Question", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.CoachingSession", "CoachingSession")
                        .WithMany("Questions")
                        .HasForeignKey("CoachingSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.Member", "MemberWhoCreate")
                        .WithMany("Questions")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.Navigation("CoachingSession");

                    b.Navigation("MemberWhoCreate");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.QuestionVote", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.Member", "Member")
                        .WithMany("QuestionVotes")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.Question", "Question")
                        .WithMany("QuestionVotes")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.VideoComment", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.Member", "MemberWhoCreate")
                        .WithMany("VideosComments")
                        .HasForeignKey("MemberId")
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.VideoComment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId");

                    b.HasOne("DevBetterWeb.Core.Entities.ArchiveVideo", "Video")
                        .WithMany("Comments")
                        .HasForeignKey("VideoId")
                        .IsRequired();

                    b.Navigation("MemberWhoCreate");

                    b.Navigation("ParentComment");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("DevBetterWeb.Core.ValueObjects.MemberFavoriteArchiveVideo", b =>
                {
                    b.HasOne("DevBetterWeb.Core.Entities.ArchiveVideo", null)
                        .WithMany("MemberFavorites")
                        .HasForeignKey("ArchiveVideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DevBetterWeb.Core.Entities.Member", null)
                        .WithMany("FavoriteArchiveVideos")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.ArchiveVideo", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("MemberFavorites");

                    b.Navigation("MembersVideoProgress");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.BookCategory", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.CoachingSession", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Member", b =>
                {
                    b.Navigation("BillingActivities");

                    b.Navigation("FavoriteArchiveVideos");

                    b.Navigation("MemberSubscriptions");

                    b.Navigation("MemberVideosProgress");

                    b.Navigation("QuestionVotes");

                    b.Navigation("Questions");

                    b.Navigation("VideosComments");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.Question", b =>
                {
                    b.Navigation("QuestionVotes");
                });

            modelBuilder.Entity("DevBetterWeb.Core.Entities.VideoComment", b =>
                {
                    b.Navigation("Replies");
                });
#pragma warning restore 612, 618
        }
    }
}
