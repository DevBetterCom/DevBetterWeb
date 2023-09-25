using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Identity.Data;

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
