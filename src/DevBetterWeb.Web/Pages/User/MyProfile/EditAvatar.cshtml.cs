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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

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
  private readonly IDomainEventDispatcher _dispatcher;
  private readonly IRepository<Member> _memberRepository;

  public EditAvatarModel(UserManager<ApplicationUser> userManager,
      IConfiguration configuration,
      ILogger<EditAvatarModel> logger,
      IDomainEventDispatcher dispatcher,
      IRepository<Member> memberRepository)
  {
    _userManager = userManager;
    _configuration = configuration;
    _logger = logger;
    _dispatcher = dispatcher;
    _memberRepository = memberRepository;
  }

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser.Id);
    _logger.LogInformation($"Setting AvatarUrl to {AvatarUrl}.");
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser.Id);

    string fileName = applicationUser.Id + ".jpg";

    using (var stream = ImageFile.OpenReadStream())
    {
      string extension = ImageFile.FileName.Split(".").Last();
      // TODO: validate if extension is correct

      var uploadSuccess = await UploadToBlob(fileName, stream);

      if (uploadSuccess)
      {
        var spec = new MemberByUserIdSpec(applicationUser.Id);
        var member = await _memberRepository.FirstOrDefaultAsync(spec);
        if (member is null) throw new MemberNotFoundException(applicationUser.Id);

        var memberAvatarUpdatedEvent = new MemberAvatarUpdatedEvent(member);
        await _dispatcher.Dispatch(memberAvatarUpdatedEvent);
      }
    }
    return RedirectToPage("./Index");
  }

  private async Task<bool> UploadToBlob(string filename,
    Stream stream)
  {
    _logger.LogInformation($"Uploading file to blob storage...");

    string storageConnectionString = _configuration[Constants.ConfigKeys.FileStorageConnectionString];

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

      if (stream != null)
      {
        var modifiedImageStream = ResizeStream(stream);

        // OPTION B: pass in memory stream directly
        _logger.LogInformation($"Uploading from Stream.");
        await cloudBlockBlob.UploadFromStreamAsync(modifiedImageStream);
      }
      else
      {
        _logger.LogWarning($"Stream is null.");
        return false;
      }

      return true;
    }
    catch (StorageException ex)
    {
      _logger.LogError(ex, $"Error uploading file");
      throw new Exception("Error uploading file.", ex);
    }
  }

  private Stream ResizeStream(Stream stream)
  {
    // see: https://stackoverflow.com/a/55395009/13729
    var cloneStream = new MemoryStream();
    stream.Position = 0;
    var image = Image.Load(stream);
    _logger.LogWarning($"Resizing image. {image.Width} x {image.Height}");

    var jpegEncoder = new JpegEncoder { Quality = 80 };
    var clone = image.Clone(context => context.Resize(new ResizeOptions
    {
      Mode = ResizeMode.Crop,
      Size = new Size(500, 500)
    }));
    clone.Save(cloneStream, jpegEncoder);
    cloneStream.Position = 0;
    return cloneStream;
  }
}
