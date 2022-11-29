using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Endpoints;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class VideoListDataTable : EndpointBaseAsync
	.WithRequest<DataTableParameterModel>
	.WithActionResult
{
	private readonly IRepository<ArchiveVideo> _archiveVideoRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;

	public VideoListDataTable(IRepository<ArchiveVideo> archiveVideoRepository, UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository)
	{
		_archiveVideoRepository = archiveVideoRepository;
		_userManager = userManager;
		_memberRepository = memberRepository;
	}

	[HttpPost("videos/list")]
	public override async Task<ActionResult> HandleAsync([FromForm] DataTableParameterModel dataTableParameterModel, CancellationToken cancellationToken = default)
	{
		var draw = dataTableParameterModel.Draw;
		var length = dataTableParameterModel.Length;
		int pageSize = length != null ? Convert.ToInt32(length) : 20;
		var startIndex = Convert.ToInt32(dataTableParameterModel.Start);

		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser!.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec, cancellationToken);

		if (member is null)
		{
			return Unauthorized();
		}

		var pagedSpec = new ArchiveVideoByPageSpec(startIndex, pageSize, dataTableParameterModel.Search, dataTableParameterModel.FilterFavorites, member.Id);
		var totalRecords = await _archiveVideoRepository.CountAsync(pagedSpec, cancellationToken);
		var archiveVideos = await _archiveVideoRepository.ListAsync(pagedSpec, cancellationToken);

		var archiveVideosDto = archiveVideos.Select(av => new ArchiveVideoDto
		{
			AnimatedThumbnailUri = av.AnimatedThumbnailUri,
			DateCreated = av.DateCreated,
			DateUploaded = av.DateUploaded,
			Description = av.Description,
			Duration = av.Duration,
			IsMemberFavorite = member.FavoriteArchiveVideos.Any(fav => fav.ArchiveVideoId == av.Id),
			MemberFavoritesCount = av.MemberFavorites.Count(),
			Status = av.Status,
			Title = av.Title,
			VideoId = av.VideoId,
			VideoUrl = av.VideoUrl,
			Views = av.Views
		}).ToList();

		var currentPage = Math.Ceiling((decimal)(startIndex + 1) / pageSize);
		
		return Ok(new VideoListDataTableResponse(draw, currentPage, totalRecords, totalRecords, archiveVideosDto));
	}
}
