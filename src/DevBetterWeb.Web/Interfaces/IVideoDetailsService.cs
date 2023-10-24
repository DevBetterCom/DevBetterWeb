using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Vimeo.Models;
using NimblePros.ApiClient.Interfaces;
using NimblePros.Vimeo.Models;

namespace DevBetterWeb.Web.Interfaces;

public interface IVideoDetailsService
{
	Task<(IApiResponse<Video>, string, ArchiveVideo?, ApplicationUser)> GetDataAsync(
		string videoId,
		string? currentUserName,
		string currentVideoURL);

	Task IncrementViewsAndUpdate(ArchiveVideo archiveVideo);

	Task<string> GetTranscriptAsync(IEnumerable<TextTrack> textTracks, string videoUrl);
}
