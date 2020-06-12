Created by DJ Zevenbergen

This application uses C#/.NET, S3, and MailKit to take in an Uploaded file, upload it to S3, and then email a link to that file to the email provided by the user.

##setup

##_View webpage  http://18.234.151.62/ _

##Bugs

It seems to be sending the link twice or three times. I'm sure it's fixable, I'll just need to spend more time working on it.


locate to the root directory

touch App.config
touch appsettings.json
touch appsettings.Development.json
mkdir uploads


populate App.config with:

"
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="AWSAccessKey" value="request-access-key-or-admin-policy-from-me"/>
    <add key="AWSSecretKey" value="request-access-key-or-admin-policy-from-me"/>
    <add key="AWSRegion" value="us-east-1"/>
  </appSettings>
</configuration>

"

populate appsettings.json with:

"
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
"

populate appsettings.Development.json with:

"
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}

"


dotnet build

MIT License 2020
