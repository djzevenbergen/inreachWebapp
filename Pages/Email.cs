// using Amazon;
// using System;
// using System.Collections.Generic;
// using Amazon.SimpleEmail;
// using Amazon.SimpleEmail.Model;

// namespace InreachWebapp.Pages
// {
//   class Program
//   {
//     // Replace sender@example.com with your "From" address.
//     // This address must be verified with Amazon SES.
//     static readonly string senderAddress = "sender@example.com";

//     // Replace recipient@example.com with a "To" address. If your account
//     // is still in the sandbox, this address must be verified.
//     static readonly string receiverAddress = "recipient@example.com";

//     // The configuration set to use for this email. If you do not want to use a
//     // configuration set, comment out the following property and the
//     // ConfigurationSetName = configSet argument below. 
//     static readonly string configSet = "ConfigSet";

//     // The subject line for the email.
//     static readonly string subject = "Amazon SES test (AWS SDK for .NET)";

//     // The email body for recipients with non-HTML email clients.
//     static readonly string textBody = "Amazon SES Test (.NET)\r\n"
//                                     + "This email was sent through Amazon SES "
//                                     + "using the AWS SDK for .NET.";

//     // The HTML body of the email.
//     static readonly string htmlBody = @"<html>
// <head></head>
// <body>
//   <h1>Amazon SES Test (AWS SDK for .NET)</h1>
//   <p>This email was sent with
//     <a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
//     <a href='https://aws.amazon.com/sdk-for-net/'>
//       AWS SDK for .NET</a>.</p>
// </body>
// </html>";

//     static void Main(string[] args)
//     {
//       var response = client.SendEmail(new SendEmailRequest
//       {
//         Destination = new Destination
//         {
//           BccAddresses = new List<string>
//           {

//           },
//           CcAddresses = new List<string> {
//             "recipient3@example.com"
//         },
//           ToAddresses = new List<string> {
//             "recipient1@example.com",
//             "recipient2@example.com"
//         }
//         },
//         Message = new Message
//         {
//           Body = new Body
//           {
//             Html = new Content
//             {
//               Charset = "UTF-8",
//               Data = "This message body contains HTML formatting. It can, for example, contain links like this one: <a class=\"ulink\" href=\"http://docs.aws.amazon.com/ses/latest/DeveloperGuide\" target=\"_blank\">Amazon SES Developer Guide</a>."
//             },
//             Text = new Content
//             {
//               Charset = "UTF-8",
//               Data = "This is the message body in text format."
//             }
//           },
//           Subject = new Content
//           {
//             Charset = "UTF-8",
//             Data = "Test email"
//           }
//         },
//         ReplyToAddresses = new List<string>
//         {

//         },
//         ReturnPath = "",
//         ReturnPathArn = "",
//         Source = "sender@example.com",
//         SourceArn = ""
//       });

//       string messageId = response.MessageId;
//     }
//   }
// }