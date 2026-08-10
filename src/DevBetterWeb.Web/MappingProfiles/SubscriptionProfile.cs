using System.Linq;
using AutoMapper;
using DevBetterWeb.Web.Models;
using Stripe;

namespace DevBetterWeb.Web.MappingProfiles;

public class SubscriptionProfile : Profile
{
	public SubscriptionProfile()
	{
		CreateMap<Subscription, StripeSubscriptionDto>()
			.ForMember(dest => dest.CustomerEmail,
				opt => opt.MapFrom((src, _) => src.Customer?.Email ?? string.Empty))
			.ForMember(dest => dest.PlanName,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Nickname ?? string.Empty))
			.ForMember(dest => dest.Amount,
				opt => opt.MapFrom((src, _) => (src.Items?.Data?.FirstOrDefault()?.Price?.UnitAmountDecimal ?? 0m) / 100m))
			.ForMember(dest => dest.Currency,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Currency ?? string.Empty))
			.ForMember(dest => dest.Interval,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Recurring?.Interval ?? string.Empty))
			.ForMember(dest => dest.CurrentPeriodEnd,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd ?? default))
			.ForMember(dest => dest.IsPaused,
				opt => opt.MapFrom((src, _) => src.PauseCollection != null));
	}
}
