using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.Services
{
  public class DailyCheckSubscriptionPlanCountService : IDailyCheckSubscriptionPlanCountService
  {
    private readonly IRepository<MemberSubscriptionPlan> _repository;
    private readonly IPaymentHandlerPrice _paymentHandlerPrice;
    private readonly IOptions<SubscriptionPlanOptions> _optionsAccessor;

    private int? _expectedNumberOfSubscriptionPlansNotInPaymentProvider;

    public DailyCheckSubscriptionPlanCountService(IRepository<MemberSubscriptionPlan> repository,
      IPaymentHandlerPrice paymentHandlerPrice,
      IOptions<SubscriptionPlanOptions> options)
    {
      _repository = repository;
      _paymentHandlerPrice = paymentHandlerPrice;
      _optionsAccessor = options;
      _expectedNumberOfSubscriptionPlansNotInPaymentProvider = _optionsAccessor.Value.expectedNumberOfSubscriptionPlansNotInPaymentProvider;
    }

    public async Task WarnIfNumberOfMemberSubscriptionPlansDifferentThanExpected(AppendOnlyStringList messages)
    {
      // TODO this can be removed once stripe is live
      // if(!EXPECTED_NUMBER_OF_SUBSCRIPTION_PLANS_NOT_IN_PAYMENT_PROVIDER.HasValue || EXPECTED_NUMBER_OF_SUBSCRIPTION_PLANS_NOT_IN_PAYMENT_PROVIDER <= 0)
      // {
      //   return;
      // }

      var subscriptionPlans = await _repository.ListAsync();
      int numberOfSubscriptionPlans = subscriptionPlans.Count();

      var numberOfPaymentProviderSubscriptionPlans = await _paymentHandlerPrice.GetPriceCount();

      if(numberOfSubscriptionPlans != numberOfPaymentProviderSubscriptionPlans + _expectedNumberOfSubscriptionPlansNotInPaymentProvider)
      {
        messages.Append($"THE NUMBER OF MEMBERSUBSCRIPTIONPLANS ON DevBetter ({numberOfSubscriptionPlans} DOES NOT MATCH THE EXPECTED VALUE IN STRIPE ({numberOfPaymentProviderSubscriptionPlans}) PLUS CONFIGURED VALUE OF PLANS NOT IN STRIPE ({_expectedNumberOfSubscriptionPlansNotInPaymentProvider})!");
      }
    }
  }
}
