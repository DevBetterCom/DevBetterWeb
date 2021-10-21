using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Vimeo.Models;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.Videos
{
  [RequestFormLimits(MultipartBodyLengthLimit = Constants.MAX_UPLOAD_FILE_SIZE)]
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class CreateModel : PageModel
  {
    private readonly IMapper _mapper;
    private readonly UploadVideoService _uploadVideoService;

    public CreateModel(IMapper mapper, UploadVideoService uploadVideoService)
    {
      _mapper = mapper;
      _uploadVideoService = uploadVideoService;
    }

    public IActionResult OnGet()
    {
      return Page();
    }

    [BindProperty]
    public VideoModel VideoModel { get; set; } = new VideoModel();

    public async Task<IActionResult> OnPostAsync(IFormFile videoFile)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }
      if (videoFile.Length <= 0)
      {
        return BadRequest();
      }

      var videoEntity = _mapper.Map<Video>(VideoModel);

      var fileData = ReadFromFormFile(videoFile);

      videoEntity
        .SetCreatedTime(videoEntity.GetEncodedDate(fileData))
        .SetReleaseTime(videoEntity.GetEncodedDate(fileData))
        .SetEmbedProtecedPrivacy()
        .SetEmbed();

      var request = new UploadVideoRequest("me", fileData, videoEntity, Constants.VIMEO_ALLOWED_DOMAIN);
      var response = await _uploadVideoService.ExecuteAsync(request);

      if (response.Data > 0)
      {
        return RedirectToPage("/Videos/Index");
      }

      return RedirectToPage("./Index");
    }

    private static byte[] ReadFromFormFile(IFormFile file)
    {
      using (var stream = file.OpenReadStream())
      {
        return ReadToEnd(stream);
      }
    }

    private static byte[] ReadToEnd(Stream stream)
    {
      long originalPosition = 0;

      if (stream.CanSeek)
      {
        originalPosition = stream.Position;
        stream.Position = 0;
      }

      try
      {
        byte[] readBuffer = new byte[4096];

        int totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
        {
          totalBytesRead += bytesRead;

          if (totalBytesRead == readBuffer.Length)
          {
            int nextByte = stream.ReadByte();
            if (nextByte != -1)
            {
              byte[] temp = new byte[readBuffer.Length * 2];
              Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
              Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
              readBuffer = temp;
              totalBytesRead++;
            }
          }
        }

        byte[] buffer = readBuffer;
        if (readBuffer.Length != totalBytesRead)
        {
          buffer = new byte[totalBytesRead];
          Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
        }
        return buffer;
      }
      finally
      {
        if (stream.CanSeek)
        {
          stream.Position = originalPosition;
        }
      }
    }
  }
}
