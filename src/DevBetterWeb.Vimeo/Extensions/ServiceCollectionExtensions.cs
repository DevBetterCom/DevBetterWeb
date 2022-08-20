using System;
using System.Net.Http;
using Ardalis.ApiClient;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Interfaces;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.UtilityServices;
using DevBetterWeb.Vimeo.Services.VideoServices;
using Microsoft.Extensions.DependencyInjection;

namespace DevBetterWeb.Vimeo.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddVimeoServices(this IServiceCollection services, string token = null)
  {
    var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
    httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
    httpClient.Timeout = TimeSpan.FromMinutes(60);
    if (!string.IsNullOrEmpty(token))
    {
      httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
    }
    services.AddScoped(sp => httpClient);

    services.AddScoped<HttpService>();

    services.AddScoped<AccountDetailsService>();
    services.AddScoped<LoginService>();
    services.AddScoped<UserDetailsService>();

    services.AddScoped<AddDomainToVideoService>();
    services.AddScoped<CompleteUploadByCompleteUriService>();
    services.AddScoped<CompleteUploadService>();
    services.AddScoped<DeleteVideoService>();
    services.AddScoped<GetPagedVideosService>();
    services.AddScoped<GetAllVideosService>();
    services.AddScoped<GetAttemptService>();
    services.AddScoped<GetOEmbedVideoService>();
    services.AddScoped<GetStreamingTicketService>();
    services.AddScoped<GetVideoService>();
    services.AddScoped<UpdateVideoDetailsService>();
    services.AddScoped<UploadVideoService>();
    services.AddScoped<ActiveTextTrackService>();
    services.AddScoped<GetUploadLinkTextTrackService>();
    services.AddScoped<UploadTextTrackFileService>();
    services.AddScoped<UploadVideoSubtitleService>();
    services.AddScoped<GetAllTextTracksService>();
    services.AddScoped<CreateAnimatedThumbnailsService>();
    services.AddScoped<GetAnimatedThumbnailService>();
    services.AddScoped<GetAllAnimatedThumbnailService>();
    services.AddScoped<GetStatusAnimatedThumbnailService>();
    services.AddScoped<AddAnimatedThumbnailsToVideoService>();
    services.AddScoped<UploadResumableVideoService>();
    services.AddScoped<UploadResumableCreateVideoLinkService>();
		services.AddScoped<ISleepService, SleepService>();

    return services;
  }
}
