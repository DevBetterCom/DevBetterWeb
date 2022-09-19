using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;

namespace DevBetterWeb.Web.Interfaces;

public interface IVideoDetailsService
{
	Task<(HttpResponse<Video>, HttpResponse<GetAllTextTracksResponse>, ArchiveVideo?, ApplicationUser)> GetDataAsync(
    string videoId, 
    string? currentUserName);

	Task IncrementViewsAndUpdate(ArchiveVideo archiveVideo);

    Task<string> GetTranscript(IEnumerable<TextTrack> textTracks, string videoUrl);
}
