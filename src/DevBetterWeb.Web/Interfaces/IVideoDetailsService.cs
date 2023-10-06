using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Web.Interfaces;

public interface IVideoDetailsService
{
	Task<(HttpResponse<Video>, string, ArchiveVideo?, ApplicationUser)> GetDataAsync(
		string videoId,
		string? currentUserName,
		string currentVideoURL);

	Task IncrementViewsAndUpdate(ArchiveVideo archiveVideo);

	Task<string> GetTranscriptAsync(IEnumerable<TextTrack> textTracks, string videoUrl);
}
