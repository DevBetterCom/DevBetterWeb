using System;
using System.Net.Http;
using Ardalis.ApiCaller;
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
      var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
      httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
      httpClient.Timeout = TimeSpan.FromMinutes(60);
      services.AddScoped(sp => httpClient);

      services.AddScoped<HttpService>();

      services.AddScoped<AccountDetailsService>();
      services.AddScoped<LoginService>();
      services.AddScoped<UserDetailsService>();
      
      services.AddScoped<AddDomainToVideoService>();
      services.AddScoped<CompleteUploadByCompleteUriService>();
      services.AddScoped<CompleteUploadService>(); 
      services.AddScoped<DeleteVideoService>();
      services.AddScoped<GetAllVideosService>();
      services.AddScoped<GetAttemptService>(); 
      services.AddScoped<GetOEmbedVideoService>();
      services.AddScoped<GetStreamingTicketService>();
      services.AddScoped<GetVideoService>();
      services.AddScoped<UpdateVideoDetailsService>();       
      services.AddScoped<UploadVideoService>();

      return services;
    }
  }
}
