using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CleanArchitecture.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [RequestFormLimits(MultipartBodyLengthLimit = Constants.MAX_UPLOAD_FILE_SIZE)]
    public class UploadModel : PageModel
    {
        private IConfiguration _configuration;

        public UploadModel(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public void OnGet()
        {
        }

        //[RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        //[RequestSizeLimit(209715200)]
        public async Task<IActionResult> OnPost2()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                new FormOptions().MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            return Page();
        }

        [RequestSizeLimit(Constants.MAX_UPLOAD_FILE_SIZE)]
        public async Task<IActionResult> OnPost(List<IFormFile> files)
        {
            var uploadSuccess = false;

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
                    uploadSuccess = await UploadToBlob(formFile.FileName, null, stream);
                }

            }

            if (uploadSuccess)
            {
                return RedirectToPage("/ArchivedVideos/Index");
            }
            return Page();
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
                    throw;

                    return false;
                }
                finally
                {
                }
            }
            else
            {
                return false;
            }

        }
    }

    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public static string GetBoundary(Microsoft.Net.Http.Headers.MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (boundary.HasValue)
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary.Value;
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(Microsoft.Net.Http.Headers.ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && !contentDisposition.FileName.HasValue
                   && !contentDisposition.FileNameStar.HasValue;
        }

        public static bool HasFileContentDisposition(Microsoft.Net.Http.Headers.ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!contentDisposition.FileName.HasValue
                       || contentDisposition.FileNameStar.HasValue);
        }
    }

}