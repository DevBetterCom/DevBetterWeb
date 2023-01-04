using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities;

public class Member : BaseEntity, IAggregateRoot
{
	public Member()
	{
		UserId = "";
	}
	/// <summary>
	/// Members should only be created via the IMemberRegistrationService.
	/// This will fire off a NewMemberCreatedEvent
	/// TODO: It doesn't appear to fire off any event (see MemberRegistrationService.cs)
	/// TODO: Looks like new way is through NewMemberService.cs
	/// TODO: Seems this is being fired on login sometimes.
	/// </summary>
	/// <param name="userId"></param>
	internal Member(string userId)
	{
		UserId = userId;
		Events.Add(new NewMemberCreatedEvent(this));
	}

	internal Member(string userId, string firstName, string lastName)
	{
		UserId = userId;
		FirstName = firstName;
		LastName = lastName;
	}

	public string UserId { get; private set; }
	public string? FirstName { get; private set; }
	public string? LastName { get; private set; }
	public Birthday? Birthday { get; private set; }
	public string? AboutInfo { get; private set; }
	public string? Address { get; private set; }
	public Address? ShippingAddress { get; private set; }
	public Geolocation? CityLocation { get; private set; }

	public string? PEFriendCode { get; private set; }
	public string? PEUsername { get; private set; }

	public string? BlogUrl { get; private set; }
	public string? GitHubUrl { get; private set; }
	public string? Email { get; private set; }
	public string? LinkedInUrl { get; private set; }
	public string? OtherUrl { get; private set; }
	public string? TwitchUrl { get; private set; }
	public string? YouTubeUrl { get; private set; }
	public string? TwitterUrl { get; private set; }
	public string? CodinGameUrl { get; private set; }
	public string? DiscordUsername { get; private set; }
	public string? MastodonUrl { get; private set; }

	public List<Book> BooksRead { get; set; } = new List<Book>();
	public List<Book> UploadedBooks { get; set; } = new List<Book>();
	public List<MemberVideoProgress> MemberVideosProgress { get; set; } = new List<MemberVideoProgress>();
	public List<VideoComment> VideosComments { get; set; } = new List<VideoComment>();
	public List<Question> Questions { get; set; } = new List<Question>();
	public List<QuestionVote> QuestionVotes { get; set; } = new List<QuestionVote>();

	public DateTime DateCreated { get; private set; } = DateTime.UtcNow;
	public List<MemberSubscription> MemberSubscriptions { get; set; } = new List<MemberSubscription>();
	public decimal? CityLatitude { get; set; }
	public decimal? CityLongitude { get; set; }
	public List<BillingActivity> BillingActivities { get; set; } = new List<BillingActivity>();
	//public List<MemberVideoProgress> Videos { get; private set; } = new List<MemberVideoProgress>();
	private readonly List<MemberFavoriteArchiveVideo> _favoriteArchiveVideos = new();
	public IEnumerable<MemberFavoriteArchiveVideo> FavoriteArchiveVideos => _favoriteArchiveVideos.AsReadOnly();

	//public void AddVideoProgress(MemberVideoProgress videoProgress)
	//{
	//  Guard.Against.Null(videoProgress, nameof(videoProgress));
	//  Videos.Add(videoProgress);
	//}

	//public void AddVideoProgress(ArchiveVideo archiveVideo, int secondWatch)
	//{
	//  var video = new MemberVideoProgress(Id, archiveVideo, secondWatch);
	//  Videos.Add(video);
	//}

	public string UserFullName()
	{
		if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
		{
			return "[No Name Provided]";
		}
		else
		{
			return $"{FirstName} {LastName}";
		}
	}

	public void UpdateName(string? firstName, string? lastName, bool isEvent = true)
	{
		bool valueChanged = false;
		if (FirstName != firstName)
		{
			FirstName = firstName;
			valueChanged = true;
		}
		if (LastName != lastName)
		{
			LastName = lastName;
			valueChanged = true;
		}
		if (valueChanged && isEvent)
		{
			CreateOrUpdateUpdateEvent("Name");
		}
	}

	public void UpdateAddress(string? address, bool isEvent = true)
	{
		if (Address == address) return;

		Address = address;

		if (isEvent)
		{
			CreateOrUpdateAddressUpdateEvent(nameof(Address));
		}
	}

	public void UpdateBirthday(int? day, int? month, bool isEvent = true)
	{
		if (day == Birthday?.Day && month == Birthday?.Month) return;

		var birthday = (month == null || day == null) ? null : new Birthday(day.Value, month.Value);

		Birthday = birthday;

		if (isEvent)
		{
			CreateOrUpdateUpdateEvent(nameof(Birthday));
		}
	}

	public void UpdateEmail(string? email, bool isEvent = true)
	{
		if (Email == email) return;

		Email = email;

		if (isEvent)
		{
			CreateOrUpdateAddressUpdateEvent(nameof(Email));
		}
	}

	public void AddFavoriteArchiveVideo(ArchiveVideo archiveVideo)
	{
		if (FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == archiveVideo.Id))
		{
			return;
		}

		_favoriteArchiveVideos.Add(new MemberFavoriteArchiveVideo(Id, archiveVideo.Id));
	}

	public void RemoveFavoriteArchiveVideo(ArchiveVideo archiveVideo)
	{
		if (FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == archiveVideo.Id))
		{
			var removal = FavoriteArchiveVideos.First(v => v.ArchiveVideoId == archiveVideo.Id);

			_favoriteArchiveVideos.Remove(removal);
		}
	}

	public void UpdateShippingAddress(Address newAddress, bool isEvent = true)
	{
		if (ShippingAddress == newAddress) return;

		if (isEvent)
		{
			var addressUpdatedEvent = new MemberAddressUpdatedEvent(this, ShippingAddress);
			Events.Add(addressUpdatedEvent);
		}

		ShippingAddress = newAddress;
	}

	public void UpdateAboutInfo(string? aboutInfo, bool isEvent = true)
	{
		if (AboutInfo == aboutInfo) return;

		AboutInfo = aboutInfo;
		if (isEvent)
		{
			CreateOrUpdateUpdateEvent(nameof(AboutInfo));
		}
	}
	public void UpdatePEInfo(string? peFriendCode, string? peUsername, bool isEvent = true)
	{
		bool valueChanged = false;

		if (PEFriendCode != peFriendCode)
		{
			PEFriendCode = peFriendCode;
			valueChanged = true;
		}
		if (PEUsername != peUsername)
		{
			PEUsername = peUsername;
			valueChanged = true;
		}

		if (valueChanged && isEvent)
		{
			CreateOrUpdateUpdateEvent("ProjectEuler");
		}
	}

	public void UpdateLinks(string? blogUrl,
		string? codinGameUrl,
			string? gitHubUrl,
			string? linkedInUrl,
			string? otherUrl,
			string? twitchUrl,
			string? youtubeUrl,
			string? twitterUrl,
			string? mastodonUrl,
			bool isEvent = true)
	{
		bool valueChanged = false;
		if (BlogUrl != blogUrl)
		{
			BlogUrl = blogUrl;
			valueChanged = true;
		}
		if (CodinGameUrl != codinGameUrl)
		{
			CodinGameUrl = codinGameUrl;
			valueChanged = true;
		}
		if (GitHubUrl != gitHubUrl)
		{
			GitHubUrl = gitHubUrl;
			valueChanged = true;
		}
		if (LinkedInUrl != linkedInUrl)
		{
			LinkedInUrl = linkedInUrl;
			valueChanged = true;
		}
		if (MastodonUrl != mastodonUrl)
		{
			MastodonUrl = mastodonUrl;
			valueChanged = true;
		}
		if (OtherUrl != otherUrl)
		{
			OtherUrl = otherUrl;
			valueChanged = true;
		}
		if (TwitchUrl != twitchUrl)
		{
			TwitchUrl = twitchUrl;
			valueChanged = true;
		}
		if (YouTubeUrl != youtubeUrl)
		{
			YouTubeUrl = youtubeUrl;
			valueChanged = true;
		}
		if (TwitterUrl != twitterUrl)
		{
			TwitterUrl = twitterUrl;
			valueChanged = true;
		}
		if (valueChanged && isEvent)
		{
			CreateOrUpdateUpdateEvent("Links");
		}
	}

	public void AddBookRead(Book book)
	{
		if (!(BooksRead!.Any(b => b.Id == book.Id)))
		{
			BooksRead.Add(book);
			CreateOrUpdateUpdateEvent("Books");
			var newBookReadEvent = new MemberAddedBookReadEvent(this, book);
			Events.Add(newBookReadEvent);
		}
	}

	public void AddBookForAdd(Book book)
	{
		if (!UploadedBooks!.Any())
		{
			UploadedBooks.Add(book);
			AddBookRead(book);
			var newBookAddedEvent = new MemberAddedBookAddEvent(this, book);
			Events.Add(newBookAddedEvent);
		}
		// TODO: throw an exception if somehow someone is trying to add more than one book
	}

	public void RemoveBookRead(Book book)
	{
		if (BooksRead!.Any(b => b.Id == book.Id))
		{
			BooksRead.Remove(book);
			CreateOrUpdateUpdateEvent("Books");
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="subscriptionDateTimeRange"></param>
	/// <param name="subscriptionPlanId">Defaults to Monthly (1)</param>
	public void AddSubscription(DateTimeRange subscriptionDateTimeRange, int subscriptionPlanId)
	{
		var subscription = new MemberSubscription(this.Id, subscriptionPlanId, subscriptionDateTimeRange);

		MemberSubscriptions.Add(subscription);

		Events.Add(new SubscriptionAddedEvent(this, subscription));
	}

	public void ExtendCurrentSubscription(DateTime newEndDate)
	{
		for (int i = 0; i < MemberSubscriptions.Count; i++)
		{
			MemberSubscription s = MemberSubscriptions[i];
			if (s.Dates.Contains(DateTime.Today))
			{
				s.Dates = new DateTimeRange(s.Dates.StartDate, newEndDate);
				Events.Add(new SubscriptionUpdatedEvent(this, s));
			}
		}
	}

	public void AddBillingActivity(string subscriptionPlanName, BillingActivityVerb actionVerbPastTense, BillingPeriod billingPeriod, decimal amount = 0)
	{
		var details = new BillingDetails(UserFullName(), subscriptionPlanName, actionVerbPastTense, billingPeriod, DateTime.Now, amount);
		var activity = new BillingActivity(Id, details);
		BillingActivities.Add(activity);
		Events.Add(new BillingActivityCreatedEvent(activity, this));
	}

	public void UpdateDiscord(string? discordUsername, bool isEvent = true)
	{
		if (DiscordUsername == discordUsername) return;

		DiscordUsername = discordUsername;

		if (isEvent)
		{
			CreateOrUpdateUpdateEvent("Discord Username");
		}
	}

	private void CreateOrUpdateUpdateEvent(string updateDetails)
	{
		MemberUpdatedEvent? updatedEvent = Events.Find(evt => evt is MemberUpdatedEvent) as MemberUpdatedEvent;

		if (updatedEvent != null)
		{
			updatedEvent.UpdateDetails += "," + updateDetails;
			return;
		}

		updatedEvent = new MemberUpdatedEvent(this, updateDetails);
		Events.Add(updatedEvent);
	}

	private void CreateOrUpdateAddressUpdateEvent(string updateDetails)
	{
		if (Events.Find(evt => evt is MemberHomeAddressUpdatedEvent) is MemberHomeAddressUpdatedEvent updatedEvent)
		{
			updatedEvent.UpdateDetails += "," + updateDetails;
			return;
		}

		updatedEvent = new MemberHomeAddressUpdatedEvent(this, updateDetails);
		Events.Add(updatedEvent);
	}

	public int TotalSubscribedDays()
	{
		Guard.Against.Null(MemberSubscriptions, nameof(MemberSubscriptions));

		if (!MemberSubscriptions.Any()) return 0;

		return MemberSubscriptions.Sum(s => s.Dates.ToDaysInRangeThroughPeriodEndDate(DateTime.Today));
	}

	public class MemberAddressUpdatedHandler : IHandle<MemberAddressUpdatedEvent>
	{
		private readonly IRepository<Member> _memberRepository;
		private readonly IAppLogger<MemberAddressUpdatedHandler> _logger;
		private readonly IJsonParserService _jsonParserService;

		public IMapCoordinateService _mapCoordinateService { get; }

		public MemberAddressUpdatedHandler(IMapCoordinateService mapCoordinateService,
			IRepository<Member> memberRepository,
			IAppLogger<MemberAddressUpdatedHandler> logger,
			IJsonParserService jsonParserService)
		{
			_mapCoordinateService = mapCoordinateService;
			_memberRepository = memberRepository;
			_logger = logger;
			_jsonParserService = jsonParserService;
		}

		public async Task Handle(MemberAddressUpdatedEvent addressUpdatedEvent)
		{
			var member = addressUpdatedEvent.Member;
			var oldAddress = addressUpdatedEvent.OldAddress;
			_logger.LogInformation($"Updating address and trying to compute lat/long for member {member.UserFullName()}");

			if (member.ShippingAddress == null)
			{
				member.CityLocation = null;
				await _memberRepository.UpdateAsync(member);
				return;
			}

			string oldCityStateCountry = oldAddress?.ToCityStateCountryString() ?? "";
			string newCityStateCountry = member.ShippingAddress.ToCityStateCountryString();

			if (oldCityStateCountry == newCityStateCountry) return;

			string responseString = await _mapCoordinateService
				.GetMapCoordinates(member.ShippingAddress.ToCityStateCountryString());

			if (string.IsNullOrEmpty(responseString)) return;

			var doc = this._jsonParserService.Parse(responseString);

			try
			{
				var rootElement = doc.RootElement;

				var results = rootElement.GetProperty("results");
				var firstItem = results[0];
				var geometry = firstItem.GetProperty("geometry");
				var location = geometry.GetProperty("location");
				var lat = location.GetProperty("lat");
				var lng = location.GetProperty("lng");

				decimal latdec;
				bool parseResult = decimal.TryParse(lat.ToString(), out latdec);
				decimal lngdec;
				parseResult = decimal.TryParse(lat.ToString(), out lngdec) && parseResult;

				if (!parseResult)
				{
					member.CityLocation = null;
				}
				else
				{
					var newLocation = new Geolocation(latdec, lngdec);
					member.CityLocation = newLocation;
				}
				await _memberRepository.UpdateAsync(member);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calculating geolocation", addressUpdatedEvent);
			}
		}
	}

	public static Member SeedData(string userId, string firstName, string lastName)
	{
		return new Member(userId, firstName, lastName);
	}

	public override string ToString()
	{
		return $"{FirstName} {LastName}";
	}
}
