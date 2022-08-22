[![CI/CD Pipeline](https://github.com/DevBetterCom/DevBetterWeb/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/ardalis/DevBetterWeb/actions)
[![UptimeRobot Status](https://img.shields.io/uptimerobot/status/m781614473-ee8694b6320f5a9ae94ffd29)](https://devbetter.com/)
![UptimeRobot 30-day Uptime](https://img.shields.io/uptimerobot/ratio/m781614473-ee8694b6320f5a9ae94ffd29)

# DevBetterWeb

A web application for devBetter.com, a developer coaching program web site and application.

## What is devBetter?

Head over to [devBetter.com](https://devbetter.com) to see the live site. Scroll through the home page and read the testimonials. Essentially devBetter is a group dedicated to improving professional software developers of all stripes. We have a virtual community (currently using Discord) and we meet for live group Q&A sessions about once a week (currently using Zoom). We challenge and promote one another, answer tough code and software design questions, work through exercises, and more. This site is used as a playground by some members and its owner, Steve, to provide a real, working example of some of the coding techniques and practices we discuss. This is in contrast to labs, katas, and exercises that, while also valuable, are not the same as solving real world problems with real software in a production environment.

## Features

- Register
- Login
- View Public Questions/Topics
- Validate Accounts via Email (SendGrid)

### Members Only

- Update Profile
- View Member List
- View Recorded Coaching Sessions (backlog)
- View Book Leaderboard (who has read more of the books the group agrees are worth reading)

### Administrators Only

- View Users
- View Roles
- Add/Remove Users to Role
- Add/Remove Roles to User
- Add/Remove Books
- Update Member Subscription Dates
- Update User email confirmation

## Development Links

- [Production Site](https://devbetter.com/)

### Building and Running the App Locally

- Clone (or Fork and Clone) the repository locally
- Run migrations for both AppDbContext and IdentityDbContext

```powershell
# RUN THIS FROM THE WEB PROJECT FOLDER
dotnet ef database update -c appdbcontext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj

# RUN THIS FROM THE INFRASTRUCTURE PROJECT FOLDER
dotnet ef database update -c IdentityDbContext -s ..\devbetterweb.web\DevBetterWeb.Web.csproj
```

- Modify Program.cs so that it seeds the database the first time you run (uncomment [this line](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Web/Program.cs#L35))

You should be able to run the application at this point. The default password for seeded accounts is [here](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Core/AuthConstants.cs#L13). The default users created are [here](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Infrastructure/Identity/Data/AppIdentityDbContextSeed.cs). Members are created [the first time they visit their edit profile page](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Web/Pages/User/MyProfile/Index.cshtml.cs#L64).

Some actions, such as registering a member, send email notifications. You should run a [local email emulator like SMTP4Dev or Papercut](https://ardalis.com/configuring-a-local-test-email-server/) to capture these, or configure your local environment to use a fake email sender class.

You should create an **appsettings.development.json** file to hold your other connection strings such as for Azure Storage. You can use [Azurite](https://github.com/Azure/Azurite) as a local emulator for this.

You will need to set up a Discord server - see [here](https://ardalis.com/add-discord-notifications-to-asp-net-core-apps/) for a walkthrough -  and add the url to  appsettings.development.json in the Discord Webhooks section that you can copy from appsettings.json. Alternatively you can create a mock server which will provide you with a url to use - an example is mocky.io

## EF Migrations Commands

Add a new migration (from the DevBetter.Web folder):

```powershell
dotnet ef migrations add MIGRATIONNAME -c appdbcontext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj -o Data/Migrations
```

If changes on the Identity then you need to Add a new migration (from the DevBetter.Web folder):
```powershell
 dotnet ef migrations add MIGRATIONNAME -c IdentityDbContext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj -o Identity/Data/Migrations
```

Update AppDbContext model (from the DevBetter.Web folder):

```powershell
dotnet ef database update -c appdbcontext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj
```

Generate Idempotent Update Script (for production)(from the DevBetter.Web folder):

```powershell
dotnet ef migrations script -c AppDbContext -i -o migrate.sql -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj
```

## Video Upload Instructions (admin only)

Put the video files and their associated markdown files in a folder you wish to upload from. Specify the Vimeo token and devBetter API key.

For the API link, the production link should be the root web site, https://devbetter.com/ 

```powershell
.\DevBetterWeb.UploaderApp.exe -d [folder] -t [Vimeo token] -a [api link] -akey [api key]
```
