using AutoMapper;
using DevBetterWeb.Web.Models;
using Stripe.Issuing;

namespace DevBetterWeb.Web.MappingProfiles;

public class TransactionProfile : Profile
{
  public TransactionProfile()
  {
	  CreateMap<Transaction, StripeTransactionDto>()
		  .ForPath(dest => dest.CardHolder,
			  opt => opt.MapFrom(source => source.Cardholder.Name))
		  .ForPath(dest => dest.CreatedDate,
			  opt => opt.MapFrom(source => source.Created))
		  .ForPath(dest => dest.Amount,
		  opt => opt.MapFrom(source => source.Amount));

	}
}
