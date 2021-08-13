using System;
using System.Net.Http;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.DependencyInjection;

namespace DevBetterWeb.Vimeo.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddVimeoServices(this IServiceCollection services)
    {
      services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) });
      services.AddScoped<AccountDetailsService>();
      services.AddScoped<LoginService>();
      services.AddScoped<UserDetailsService>();

      services.AddScoped<CompleteUploadByCompleteUriService>();
      services.AddScoped<CompleteUploadService>();
      services.AddScoped<GetAttemptService>();
      services.AddScoped<GetStreamingTicketService>();
      services.AddScoped<UpdateVideoDetailsService>();
      services.AddScoped<UploadVideoService>();
      

      return services;
    }
  }
}
