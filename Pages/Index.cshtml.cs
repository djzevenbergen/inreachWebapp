using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System;

namespace InreachWebapp.Pages
{
  public class UploadFileModel : PageModel
  {
    private const string bucketName = "inreachapplicationtest";
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
    private static IAmazonS3 s3Client;

    private IHostingEnvironment _environment;
    public UploadFileModel(IHostingEnvironment environment)
    {
      _environment = environment;
    }

    [BindProperty]
    public IFormFile Upload { get; set; }
    public async Task OnPostAsync()
    {
      var accessKey = EnvironmentVariables.AccessKey;// Get access key from a secure store
      var secretKey = EnvironmentVariables.SecretKey;// Get secret key from a secure store
      s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
      UploadFileAsync().Wait();
    }

    private async Task UploadFileAsync()
    {
      var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
      using (var fileStream = new FileStream(file, FileMode.Create))
      {
        await Upload.CopyToAsync(fileStream);
      }
      try
      {
        var fileTransferUtility = new TransferUtility(s3Client);

        // using (var fileToUpload =
        //     new FileStream(Upload, FileMode.Create))
        // {
        //   await fileTransferUtility.UploadAsync(fileToUpload, bucketName, Upload.FileName);
        // }

        await fileTransferUtility.UploadAsync(file, bucketName, Upload.FileName);
        Console.WriteLine("Upload Complete");
      }
      catch (AmazonS3Exception e)
      {
        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
      }
      catch (Exception e)
      {
        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
      }

    }

  }
}