using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services
{
  public class DailyCheckSubscriptionPlanCountService : IDailyCheckSubscriptionPlanCountService
  {
    private readonly IRepository<MemberSubscriptionPlan> _repository;
    private readonly IPaymentHandlerPrice _paymentHandlerPrice;

    private const int EXPECTED_NUMBER_OF_SUBSCRIPTION_PLANS_NOT_IN_PAYMENT_PROVIDER = 2;

    public DailyCheckSubscriptionPlanCountService(IRepository<MemberSubscriptionPlan> repository,
      IPaymentHandlerPrice paymentHandlerPrice)
    {
      _repository = repository;
      _paymentHandlerPrice = paymentHandlerPrice;
    }

    public async Task WarnIfNumberOfMemberSubscriptionPlansDifferentThanExpected(AppendOnlyStringList messages)
    {
      var subscriptionPlans = await _repository.ListAsync();
      int numberOfSubscriptionPlans = subscriptionPlans.Count();

      var numberOfPaymentProviderSubscriptionPlans = await _paymentHandlerPrice.GetPriceCount();

      if(numberOfSubscriptionPlans != numberOfPaymentProviderSubscriptionPlans + EXPECTED_NUMBER_OF_SUBSCRIPTION_PLANS_NOT_IN_PAYMENT_PROVIDER)
      {
        messages.Append("THE NUMBER OF MEMBERSUBSCRIPTIONPLANS DOES NOT MATCH THE EXPECTED VALUE");
      }
    }
  }
}
