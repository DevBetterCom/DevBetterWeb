using System;
using System.Net.Http;
using Ardalis.ApiClient;
using Autofac;
using DevBetterWeb.Vimeo.Constants;
using DevBetterWeb.Vimeo.Services.UserServices;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Vimeo.Extensions;

public class AutofacExtensions
{
  public static void RegisterVimeoServicesDependencies(ContainerBuilder builder, string token = null)
  {
    var httpClient = new HttpClient { BaseAddress = new Uri(ServiceConstants.VIMEO_URI) };
    httpClient.DefaultRequestHeaders.Add("Accept", ServiceConstants.VIMEO_HTTP_ACCEPT);
    httpClient.Timeout = TimeSpan.FromMinutes(60);
    if (!string.IsNullOrEmpty(token))
    {
      httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
    }

    builder.Register(ctx => httpClient)
      .SingleInstance();

    builder.RegisterType<HttpService>();

    builder.RegisterType<AccountDetailsService>();
    builder.RegisterType<LoginService>();
    builder.RegisterType<UserDetailsService>();

    builder.RegisterType<AddDomainToVideoService>();
    builder.RegisterType<CompleteUploadByCompleteUriService>();
    builder.RegisterType<CompleteUploadService>();
    builder.RegisterType<DeleteVideoService>();
    builder.RegisterType<GetAllVideosService>();
    builder.RegisterType<GetAttemptService>();
    builder.RegisterType<GetOEmbedVideoService>();
    builder.RegisterType<GetStreamingTicketService>();
    builder.RegisterType<GetVideoService>();
    builder.RegisterType<UpdateVideoDetailsService>();
    builder.RegisterType<UploadVideoService>();
    builder.RegisterType<ActiveTextTrackService>();
    builder.RegisterType<GetUploadLinkTextTrackService>();
    builder.RegisterType<UploadTextTrackFileService>();
    builder.RegisterType<UploadSubtitleToVideoService>();
    builder.RegisterType<GetAllTextTracksService>();
    builder.RegisterType<CreateAnimatedThumbnailsService>();
  }
}
