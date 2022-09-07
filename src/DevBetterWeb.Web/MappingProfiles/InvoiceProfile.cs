using AutoMapper;
using DevBetterWeb.Web.Models;
using Stripe;

namespace DevBetterWeb.Web.MappingProfiles;

public class InvoiceProfile : Profile
{
  public InvoiceProfile()
  {
	  CreateMap<Invoice, StripeInvoiceDto>()
		  .ForPath(dest => dest.IsPaid,
			  opt => opt.MapFrom(source => source.Paid))
		  .ForPath(dest => dest.IsPaidOutOfBand,
			  opt => opt.MapFrom(source => source.PaidOutOfBand))
			.ForPath(dest => dest.FinalizedAt,
			  opt => opt.MapFrom(source => source.StatusTransitions.FinalizedAt))
		  .ForPath(dest => dest.PaidAt,
			  opt => opt.MapFrom(source => source.StatusTransitions.PaidAt));
  }
}
