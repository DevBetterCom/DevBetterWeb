using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [RequestFormLimits(MultipartBodyLengthLimit = Constants.MAX_UPLOAD_FILE_SIZE)]
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class CreateModel : PageModel
    {
        private IConfiguration _configuration;
        private readonly IRepository _repository;

        public CreateModel(IRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ArchiveVideoCreateDTO ArchiveVideoModel { get; set; }

        public class ArchiveVideoCreateDTO
        {
            [Required]
            public string Title { get; set; }
            [DisplayName(DisplayConstants.ArchivedVideo.ShowNotes)]
            public string ShowNotes { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTimeOffset DateCreated { get; set; }
        }

        //[RequestSizeLimit(Constants.MAX_UPLOAD_FILE_SIZE)]
        public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var videoEntity = new ArchiveVideo()
            {
                DateCreated = ArchiveVideoModel.DateCreated,
                ShowNotes = ArchiveVideoModel.ShowNotes,
                Title = ArchiveVideoModel.Title,
                VideoUrl = ""
            };

            _repository.Add(videoEntity);

            var uploadSuccess = false;

            string dateString = videoEntity.DateCreated.ToString(Constants.FILE_DATE_FORMAT_STRING);
            string fileName = ""; 
            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                // NOTE: uncomment either OPTION A or OPTION B to use one approach over another

                // OPTION A: convert to byte array before upload
                //using (var ms = new MemoryStream())
                //{
                //    formFile.CopyTo(ms);
                //    var fileBytes = ms.ToArray();
                //    uploadSuccess = await UploadToBlob(formFile.FileName, fileBytes, null);
                //}

                // OPTION B: read directly from stream for blob upload      
                using (var stream = formFile.OpenReadStream())
                {
                    string extension = formFile.FileName.Split(".").Last();
                    fileName = $"{dateString}-{videoEntity.Id}.{extension}";
                    uploadSuccess = await UploadToBlob(fileName, null, stream);
                }

            }

            if (uploadSuccess)
            {
                videoEntity.VideoUrl = fileName;
                _repository.Update(videoEntity);
                return RedirectToPage("/ArchivedVideos/Index");
            }


            return RedirectToPage("./Index");
        }

        private async Task<bool> UploadToBlob(string filename, byte[] imageBuffer = null, Stream stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = _configuration["storageconnectionstring"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    cloudBlobContainer = cloudBlobClient.GetContainerReference("videos");

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                catch (StorageException ex)
                {
                    throw new Exception("Error uploading file.", ex);
                }
            }
            else
            {
                return false;
            }

        }

    }
}
