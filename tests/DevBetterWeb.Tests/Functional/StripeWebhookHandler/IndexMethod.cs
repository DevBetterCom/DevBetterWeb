using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Tests.Integration.Web;
using DevBetterWeb.Web;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Functional.StripeWebhookHandler
{
  //public class IndexMethod : IClassFixture<StripeWebhookTestWebApplicationFactory<Startup>>
  //{
  //  private readonly HttpClient _client;

  //  private string _httpRequestBody = "{ 'id': 'evt_1IJTTjJxWL4MzbyjTB4fzvLC', 'object': 'event',  'api_version': '2020-08-27',  'created': 1613004979,  'data': { 'object': { 'id': 'sub_IuWb2G84PCEcvV', 'object': 'subscription', 'application_fee_percent': null, 'billing_cycle_anchor': 1612820723, 'billing_thresholds': null, 'cancel_at': null, 'cancel_at_period_end': false, 'canceled_at': null, 'collection_method': 'charge_automatically', 'created': 1612820723, 'current_period_end': 1615239923, 'current_period_start': 1612820723, 'customer': 'cus_IuWbmpXNfiznUM', 'days_until_due': null, 'default_payment_method': null, 'default_source': null, 'default_tax_rates': [    ], 'discount': null, 'ended_at': null, 'items': { 'object': 'list', 'data': [ { 'id': 'si_IuWbChjGpgYxwA', 'object': 'subscription_item', 'billing_thresholds': null, 'created': 1612820723, 'metadata': { }, 'plan': { 'id': 'price_1IDbLDJxWL4MzbyjIJh8M8yw', 'object': 'plan', 'active': true, 'aggregate_usage': null, 'amount': 20000, 'amount_decimal': '20000', 'billing_scheme': 'per_unit',  'created': 1611605235, 'currency': 'usd', 'interval': 'month', 'interval_count': 1, 'livemode': false, 'metadata': { }, 'nickname': null, 'product': 'prod_IpFqRz0PqpK6lW', 'tiers_mode': null, 'transform_usage': null, 'trial_period_days': null, 'usage_type': 'licensed' }, 'price': { 'id': 'price_1IDbLDJxWL4MzbyjIJh8M8yw', 'object': 'price', 'active': true, 'billing_scheme': 'per_unit',            'created': 1611605235,            'currency': 'usd',            'livemode': false,            'lookup_key': null,            'metadata': { },            'nickname': null,            'product': 'prod_IpFqRz0PqpK6lW',            'recurring': { 'aggregate_usage': null,              'interval': 'month',              'interval_count': 1,              'trial_period_days': null,              'usage_type': 'licensed'            },            'tiers_mode': null,            'transform_quantity': null,            'type': 'recurring',            'unit_amount': 20000,            'unit_amount_decimal': '20000'          },          'quantity': 1,          'subscription': 'sub_IuWb2G84PCEcvV',          'tax_rates': [          ]        }      ],      'has_more': false,      'total_count': 1,      'url': '/v1/subscription_items?subscription=sub_IuWb2G84PCEcvV'    },    'latest_invoice': 'in_1IIhXrJxWL4MzbyjNoO0KekM',    'livemode': false,    'metadata': { },    'next_pending_invoice_item_invoice': null,    'pause_collection': null,    'pending_invoice_item_interval': null,    'pending_setup_intent': null,    'pending_update': null,    'plan': { 'id': 'price_1IDbLDJxWL4MzbyjIJh8M8yw',      'object': 'plan',      'active': true,      'aggregate_usage': null,      'amount': 20000,      'amount_decimal': '20000',      'billing_scheme': 'per_unit',      'created': 1611605235,      'currency': 'usd',      'interval': 'month',      'interval_count': 1,      'livemode': false,      'metadata': { },      'nickname': null,      'product': 'prod_IpFqRz0PqpK6lW',      'tiers_mode': null,      'transform_usage': null,      'trial_period_days': null,      'usage_type': 'licensed'    },    'quantity': 1,    'schedule': null,    'start_date': 1612820723,    'status': 'active',    'transfer_data': null,    'trial_end': null,    'trial_start': null  }},  'type': 'customer.subscription.created'}";
  //  private string _httpRequestUri = "/api/stripecallback";

  //  private HttpContent _httpContent;

  //  private string _email = "testemail@testemail.com";
  //  private string _stripeEventId = "TestStripeId";
  //  private string _inviteCode = "TestInviteCode";
  //  // pulled from _httpRequestBody above
  //  private string _customerId = "cus_IuWbmpXNfiznUM";

  //  private Invitation _invitation;

  //  private StripeWebhookTestWebApplicationFactory<Startup> _stripeWebhookTestWebApplicationFactory;

  //  public IndexMethod(StripeWebhookTestWebApplicationFactory<Startup> factory)
  //  {
  //    _client = factory.CreateClient();
      
  //    _httpContent = new StringContent(_httpRequestBody);
  //    _invitation = new Invitation(_email, _inviteCode, _stripeEventId);

  //    _stripeWebhookTestWebApplicationFactory = new StripeWebhookTestWebApplicationFactory<Startup>();
  //  }

  //  [Fact]
  //  public async Task Gets200CodeOnPost()
  //  {
  //    var responseMessage = await _client.PostAsync(_httpRequestUri, _httpContent);

  //    responseMessage.EnsureSuccessStatusCode();
  //  }

  //}
}
