Created by DJ Zevenbergen

This application uses C#/.NET, S3, and MailKit to take in an Uploaded file, upload it to S3, and then email a link to that file to the email provided by the user. It is deployed to EC2 using Docker and ECR.

## setup

## _View webpage  http://34.202.230.95 _

## Bugs

It seems to be sending the link twice or three times. I'm sure it's fixable, I'll just need to spend more time working on it.


## Some (not all) Directions
locate to the root directory


**haven't tested to see if these are still necessary since 
touch App.config
touch appsettings.json
touch appsettings.Development.json


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

once everything builds

set up a ECR repo



aws ecr get-login-password --region <region> | docker login --username AWS --password-stdin <account-id>.dkr.ecr.<region>.amazonaws.com
  
docker tag (source-image) (account-id).dkr.ecr.(region).amazonaws.com/(repo-name)/(any-tag)
docker push (account-id).dkr.ecr.(region).amazonaws.com/(repo-name)/(any-tag)

I referenced this blog post to set up my Fargate cluster
https://itnext.io/run-your-containers-on-aws-fargate-c2d4f6a47fda



Inside my container definitions I used the environmental variables

AWS_ACCESS_KEY_ID=<key>
AWS_SECRET_ACCESS_KEY=<secret-key>
AWS_DEFAULT_REGION=<region>


MIT License 2020
