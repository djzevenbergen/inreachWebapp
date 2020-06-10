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
      var accessKey = EnvironmentVariables.AccessKey;// Get access key from a secure store
      var secretKey = EnvironmentVariables.SecretKey;// Get secret key from a secure store
      s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
      UploadFileAsync().Wait();

      Email.DoEmail(email, URLString).Wait();
    }

    private async Task UploadFileAsync()
    {
      GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
      {
        BucketName = bucketName,
        Key = Upload.FileName,
        Expires = DateTime.Now.AddMinutes(10)

      };

      URLString = s3Client.GetPreSignedURL(request1);

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
  public class Email
  {

    // Replace sender@example.com with your "From" address.
    // This address must be verified with Amazon SES.
    static readonly string senderAddress = "djzevenbergen@gmail.com";

    // Replace recipient@example.com with a "To" address. If your account
    // is still in the sandbox, this address must be verified.
    static readonly string receiverAddress = UploadFileModel.UserEmail;


    // The subject line for the email.
    static readonly string subject = "Test";

    // The email body for recipients with non-HTML email clients.
    static readonly string textBody = UploadFileModel.URLString;

    // The HTML body of the email.
    static readonly string htmlBody = @"<html>
        <head></head>
        <body>
          <h1>Amazon SES Test (AWS SDK for .NET)</h1>
          <p>This email was sent with
            <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
            <a href='https://aws.amazon.com/sdk-for-net/'>
              AWS SDK for .NET</a>.</p>
        </body>
        </html>";


    public static async Task DoEmail(string email, string URL)
    {
      // Replace USWest2 with the AWS Region you're using for Amazon SES.
      // Acceptable values are EUWest1, USEast1, and USWest2.
      using (var client = new AmazonSimpleEmailServiceClient(EnvironmentVariables.AccessKey, EnvironmentVariables.SecretKey, RegionEndpoint.USEast1))
      {
        var sendRequest = new SendEmailRequest
        {
          Source = senderAddress,
          Destination = new Destination
          {
            ToAddresses =
                new List<string> { receiverAddress }
          },
          Message = new Message
          {
            Subject = new Content(subject),
            Body = new Body
            {
              Html = new Content
              {
                Charset = "UTF-8",
                Data = htmlBody
              },
              Text = new Content
              {
                Charset = "UTF-8",
                Data = textBody
              }
            }
          },
        };
        try
        {
          Console.WriteLine("Sending email using Amazon SES...");
          var response = client.SendEmail(sendRequest);
          Console.WriteLine("The email was sent successfully.");
        }
        catch (Exception ex)
        {
          Console.WriteLine("The email was not sent.");
          Console.WriteLine("Error message: " + ex.Message);

        }
      }
    }
  }
}
