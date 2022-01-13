using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace DevBetterWeb.Client;
public static class ConfigureCommonServices
{
  public static void Configure(IServiceCollection services)
  {
    //services.AddHttpClient("DevBetterWeb.ServerAPI", client => client.BaseAddress = new Uri(hostEnvironment.BaseAddress))
    //  .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

    //// Supply HttpClient instances that include access tokens when making requests to the server project
    //services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("DevBetterWeb.ServerAPI"));

    services.AddApiAuthorization();
  }
}
