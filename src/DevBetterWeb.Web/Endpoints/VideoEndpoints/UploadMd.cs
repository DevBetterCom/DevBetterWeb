using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiClient;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Mvc;
using DevBetterWeb.Web.CustomAttributes;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Endpoints;

[UploaderApiOrRoleAuthorization(AuthConstants.Roles.ADMINISTRATORS)]
public class UploadMd : EndpointBaseAsync
	.WithRequest<UploadMdRequest>
	.WithResult<HttpResponse<bool>>
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;

	public UploadMd(IRepository<ArchiveVideo> archiveVideoRepository)
	{
		_archiveVideoRepository = archiveVideoRepository;
	}

	[HttpPost("videos/upload-md")]
	public override async Task<HttpResponse<bool>> HandleAsync([FromBody] UploadMdRequest request, CancellationToken cancellationToken = default)
	{
		var spec = new ArchiveVideoByVideoIdSpec(request.VideoId!);
		var existVideo = await _archiveVideoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		if (existVideo == null)
		{
			return new HttpResponse<bool>(false, HttpStatusCode.NotFound);
		}

		existVideo.Description = request.Md;
		await _archiveVideoRepository.UpdateAsync(existVideo, cancellationToken);

		return new HttpResponse<bool>(true, HttpStatusCode.OK);
	}
}
