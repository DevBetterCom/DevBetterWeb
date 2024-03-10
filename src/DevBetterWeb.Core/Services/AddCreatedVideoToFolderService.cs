using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using NimblePros.Vimeo.FolderServices;
using NimblePros.Vimeo.Models;

namespace DevBetterWeb.Core.Services;
public class AddCreatedVideoToFolderService : IAddCreatedVideoToFolderService
{
	private readonly GetFolderService _getFolderService;
	private readonly AddVideoToFolderService _addVideoToFolderService;
	private readonly IEmailService _emailService;

	public AddCreatedVideoToFolderService(GetFolderService getFolderService,
	 AddVideoToFolderService addVideoToFolderService,
	 IEmailService emailService ) // update this to get email interface
	{
		_getFolderService = getFolderService;
		_addVideoToFolderService = addVideoToFolderService;
		_emailService = emailService;
	}

	public async Task<bool> ExecuteAsync(bool isBaseFolder, long? folderId, ArchiveVideo archiveVideo, CancellationToken cancellationToken = default)
	{
		if (!ValidateInputs(folderId, archiveVideo))
		{
			return false;
		}

		var folder = await GetVimeoFolderAsync((int)folderId!.Value, cancellationToken);
		if (folder == null)
		{
			return false;
		}

		var isVideoAdded = await AddVideoToFolderInVimeoAsync((int)folderId.Value, int.Parse(archiveVideo.VideoId!), cancellationToken);
		if (!isVideoAdded)
		{
			return false;
		}

		// Send email logic here 
		return true;
		await _emailService.SendEmailAsync(
			email, "New video added to folder", $"New video added to folder {folder.Name}", cancellationToken);
	}

	private bool ValidateInputs(long? folderId, ArchiveVideo archiveVideo)
	{
		return folderId != null && !string.IsNullOrWhiteSpace(archiveVideo.VideoId);
	}

	private async Task<Folder?> GetVimeoFolderAsync(int folderId, CancellationToken cancellationToken)
	{
		var getFolderRequest = new GetFolderRequest(folderId);
		var getFolderResult = await _getFolderService.ExecuteAsync(getFolderRequest, cancellationToken);

		if (!getFolderResult.IsSuccess)
		{
			return null;
		}

		return new Folder().SetId((int)getFolderResult.Data.Id).SetName(getFolderResult.Data.Name);
	}

	private async Task<bool> AddVideoToFolderInVimeoAsync(int folderId, int videoId, CancellationToken cancellationToken)
	{
		var addVideoToFolderRequest = new AddVideoToFolderRequest(folderId, videoId);
		var addVideoToFolder = await _addVideoToFolderService.ExecuteAsync(addVideoToFolderRequest, cancellationToken);

		return addVideoToFolder.IsSuccess;
	}
}
