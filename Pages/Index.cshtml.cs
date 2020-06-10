using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System;
using System.Net;
using System.Net.Mail;
using MimeKit;
using Amazon.SimpleEmail.Internal;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.Runtime;


namespace InreachWebapp.Pages
{
  public class UploadFileModel : PageModel
  {
    private const string bucketName = "inreachapplicationtest";
    // private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
    private static IAmazonS3 s3Client;

    public static string URLString { get; set; }
    public static string UserEmail { get; set; }
    private IHostingEnvironment _environment;
    public UploadFileModel(IHostingEnvironment environment)
    {
      _environment = environment;
    }

    [BindProperty]
    public IFormFile Upload { get; set; }
    public async Task OnPostAsync(string email)
    {
      s3Client = new AmazonS3Client();
      var url = GeneratePresignedUrl();
      UploadObject(url[0]);
      // UploadFileAsync().Wait();
    }

    private async Task UploadObject(string url)
    {
      HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
      httpRequest.Method = "PUT";
      using (Stream dataStream = httpRequest.GetRequestStream())
      {
        var buffer = new byte[8000];
        var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
        using (var stream = System.IO.File.Create(file))
        {
          int bytesRead = 0;
          while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
          {
            dataStream.Write(buffer, 0, bytesRead);
          }
        }
      }
      HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;
    }

    private string[] GeneratePresignedUrl()
    {
      string[] urls = { "", "" };
      var request = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = Upload.FileName,
        Verb = HttpVerb.PUT,
        Expires = DateTime.Now.AddMinutes(5)

      };

      var request2 = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = Upload.FileName,
        Verb = HttpVerb.GET,
        Expires = DateTime.Now.AddMinutes(5)

      };

      string url = s3Client.GetPreSignedURL(request);
      string url2 = s3Client.GetPreSignedURL(request2);
      urls[0] = url;
      urls[1] = url2;
      Console.WriteLine(url2);
      return urls;
    }
  }
}


