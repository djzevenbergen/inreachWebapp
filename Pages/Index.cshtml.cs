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
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
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
      Console.WriteLine(email);
      UserEmail = email;
      s3Client = new AmazonS3Client(bucketRegion);
      var url = GeneratePresignedUrl();
      Console.WriteLine(url[0]);
      Console.WriteLine(url[1]);
      await UploadObject(url[0]);
      SendMail(url[1]);
    }
    private async Task UploadObject(string url)
    {
      var fileTransferUtility =
           new TransferUtility(s3Client);

      // Option 3. Upload data from a type of System.IO.Stream.
      var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
      using (FileStream stream = new FileStream(file, FileMode.Create))
      {
        await Upload.CopyToAsync(stream);
        Console.WriteLine("File hosted in docker");
      }
      using (var fileToUpload =
          new FileStream(file, FileMode.Open, FileAccess.Read))
      {
        await fileTransferUtility.UploadAsync(fileToUpload,
                                   bucketName, Upload.FileName);
        Console.WriteLine("File uploaded to s3");
      }
      FileInfo fileToDelete = new FileInfo(file);
      fileToDelete.Delete();
      Console.WriteLine("File deleted");
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
      // Console.WriteLine(url2);
      return urls;
    }
    protected void SendMail(string url)
    {

      using (var sesClient = new AmazonSimpleEmailServiceClient(bucketRegion))
      {
        var sendRequest = new SendEmailRequest
        {
          Source = "djzevenbergen@gmail.com",
          Destination = new Destination { ToAddresses = new List<string> { UserEmail } },
          Message = new Message
          {
            Subject = new Amazon.SimpleEmail.Model.Content("Here's your file!"),
            Body = new Body { Text = new Amazon.SimpleEmail.Model.Content("Your download link!" + url) }
          }
        };
        try
        {
          var response = sesClient.SendEmailAsync(sendRequest);
          Console.WriteLine("email sent to: " + UserEmail);
        }
        catch (Exception ex)
        {
          Console.WriteLine("error message: " + ex.Message);
        }
      }
    }
  }
}


