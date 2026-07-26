using System;
using System.Collections.Generic;
using AutoMapper;
using DevBetterWeb.Web.MappingProfiles;
using DevBetterWeb.Web.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.MappingProfiles;

public class SubscriptionProfileTests
{
	private readonly IMapper _mapper;

	public SubscriptionProfileTests()
	{
		var configuration = new MapperConfiguration(
			cfg => cfg.AddProfile<SubscriptionProfile>(),
			NullLoggerFactory.Instance);
		_mapper = configuration.CreateMapper();
	}

	[Fact]
	public void MapsSubscriptionToDto()
	{
		var subscription = new Subscription
		{
			Id = "sub_123",
			Status = "active",
			CustomerId = "cus_123",
			Customer = new Customer { Id = "cus_123", Email = "member@example.com" },
			CancelAtPeriodEnd = true,
			PauseCollection = new SubscriptionPauseCollection { Behavior = "void" },
			Items = new StripeList<SubscriptionItem>
			{
				Data = new List<SubscriptionItem>
				{
					new SubscriptionItem
					{
						CurrentPeriodEnd = new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc),
						Price = new Price
						{
							Nickname = "Monthly",
							UnitAmountDecimal = 20000m,
							Currency = "usd",
							Recurring = new PriceRecurring { Interval = "month" },
						},
					},
				},
			},
		};

		var dto = _mapper.Map<StripeSubscriptionDto>(subscription);

		Assert.Equal("sub_123", dto.Id);
		Assert.Equal("active", dto.Status);
		Assert.Equal("cus_123", dto.CustomerId);
		Assert.Equal("member@example.com", dto.CustomerEmail);
		Assert.Equal("Monthly", dto.PlanName);
		Assert.Equal(200m, dto.Amount);
		Assert.Equal("usd", dto.Currency);
		Assert.Equal("month", dto.Interval);
		Assert.Equal(new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc), dto.CurrentPeriodEnd);
		Assert.True(dto.CancelAtPeriodEnd);
		Assert.True(dto.IsPaused);
	}

	[Fact]
	public void MapsSubscriptionWithMissingOptionalDataWithoutThrowing()
	{
		var subscription = new Subscription { Id = "sub_bare", Status = "active" };

		var dto = _mapper.Map<StripeSubscriptionDto>(subscription);

		Assert.Equal("sub_bare", dto.Id);
		Assert.Equal(string.Empty, dto.CustomerEmail);
		Assert.False(dto.IsPaused);
		Assert.Equal(0m, dto.Amount);
	}
}
