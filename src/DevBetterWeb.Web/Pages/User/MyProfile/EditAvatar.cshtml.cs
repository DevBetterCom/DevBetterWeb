using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Services;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class EditAvatarModel : PageModel
{
#nullable disable
  public string AvatarUrl { get; set; }

  // TODO: Consider these attributes - https://stackoverflow.com/a/56592790/13729
  [BindProperty]
  public IFormFile ImageFile { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IConfiguration _configuration;
  private readonly ILogger<EditAvatarModel> _logger;
  private readonly IRepository<Member> _memberRepository;
	private readonly IMediator _mediator;

	public EditAvatarModel(UserManager<ApplicationUser> userManager,
      IConfiguration configuration,
      ILogger<EditAvatarModel> logger,
      IRepository<Member> memberRepository,
			IMediator mediator)
  {
    _userManager = userManager;
    _configuration = configuration;
    _logger = logger;
    _memberRepository = memberRepository;
		_mediator = mediator;
	}

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser!.Id);
    _logger.LogInformation($"Setting AvatarUrl to {AvatarUrl}.");
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser!.Id);

    if (ImageFile is null || ImageFile.Length == 0)
    {
      ModelState.AddModelError(nameof(ImageFile), "Please choose an image file.");
    }

    if (!ModelState.IsValid)
    {
      return Page();
    }

    string fileName = applicationUser.Id + ".jpg";

    MemoryStream resizedImage;
    try
    {
      using var uploadStream = ImageFile!.OpenReadStream();
      resizedImage = AvatarImageResizer.ResizeToJpeg(uploadStream);
    }
    catch (InvalidImageException ex)
    {
      _logger.LogWarning(ex, "Rejected avatar upload for user {UserId}.", applicationUser.Id);
      ModelState.AddModelError(nameof(ImageFile), "Please upload a JPG or PNG image no larger than 10,000 pixels on a side.");
      return Page();
    }

    using (resizedImage)
    {
      var uploadSuccess = await UploadToBlob(fileName, resizedImage);

      if (uploadSuccess)
      {
        var spec = new MemberByUserIdSpec(applicationUser.Id);
        var member = await _memberRepository.FirstOrDefaultAsync(spec);
        if (member is null) throw new MemberNotFoundException(applicationUser.Id);

        var memberAvatarUpdatedEvent = new MemberAvatarUpdatedEvent(member);
        await _mediator.Publish(memberAvatarUpdatedEvent);
      }
    }
    return RedirectToPage("./Index");
  }

  private async Task<bool> UploadToBlob(string filename,
    Stream stream)
  {
    _logger.LogInformation($"Uploading file to blob storage...");

    string storageConnectionString = _configuration[Constants.ConfigKeys.FileStorageConnectionString]!;

    if (!CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
    {
      _logger.LogWarning($"Invalid storage connection string.");
      return false;
    }

    try
    {
      // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
      CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

      // TODO: get container reference name from config
      var cloudBlobContainer = cloudBlobClient.GetContainerReference("photos");

      // Get a reference to the blob address, then upload the file to the blob.
      CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

      _logger.LogInformation($"Uploading from Stream.");
      await cloudBlockBlob.UploadFromStreamAsync(stream);

      return true;
    }
    catch (StorageException ex)
    {
      _logger.LogError(ex, $"Error uploading file");
      throw new Exception("Error uploading file.", ex);
    }
  }
}
