[![CI/CD Pipeline](https://github.com/DevBetterCom/DevBetterWeb/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/ardalis/DevBetterWeb/actions)
[![UptimeRobot Status](https://img.shields.io/uptimerobot/status/m781614473-ee8694b6320f5a9ae94ffd29)](https://devbetter.com/)
![UptimeRobot 30-day Uptime](https://img.shields.io/uptimerobot/ratio/m781614473-ee8694b6320f5a9ae94ffd29)

# DevBetterWeb

A web application for devBetter.com, a developer coaching program web site and application.

## What is devBetter?

Head over to [devBetter.com](https://devbetter.com) to see the live site. Scroll through the home page and read the testimonials. Essentially devBetter is a group dedicated to improving professional software developers of all stripes. We have a virtual community (currently using Discord) and we meet for live group Q&A sessions about once a week (currently using Zoom). We challenge and promote one another, answer tough code and software design questions, work through exercises, and more. This site is used as a playground by some members and its owner, ardalis, to provide a real, working example of some of the coding techniques and practices we discuss. This is in contrast to labs, katas, and exercises that, while also valuable, are not the same as solving real world problems with real software in a production environment.

## Features

- Register
- Login
- View Public Questions/Topics
- Validate Accounts via Email (SMTP2Go)

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

### Prerequisite

- [.NET 11 SDK](https://dotnet.microsoft.com/en-us/download)
- [The command-line interface (CLI) tools for Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### Building and Running the App Locally

The easiest way to run the app locally is with [.NET Aspire](#run-with-net-aspire), which starts SQL Server for you and applies migrations automatically. You can also run the app manually by running the SQL migrations, or by using `docker-compose` (see [this section](#run-with-docker)).

- Clone (or Fork and Clone) the repository locally
- Run migrations for both AppDbContext and IdentityDbContext

```powershell
# RUN THIS FROM THE WEB PROJECT FOLDER
dotnet ef database update -c appdbcontext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj

# RUN THIS FROM THE INFRASTRUCTURE PROJECT FOLDER
dotnet ef database update -c IdentityDbContext -s ..\devbetterweb.web\DevBetterWeb.Web.csproj
```

You should be able to run the application at this point. The default password for seeded accounts is [here](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Core/AuthConstants.cs#L13). The default users created are [here](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Infrastructure/Identity/Data/AppIdentityDbContextSeed.cs). Members are created [the first time they visit their edit profile page](https://github.com/DevBetterCom/DevBetterWeb/blob/master/src/DevBetterWeb.Web/Pages/User/MyProfile/Index.cshtml.cs#L64).

Some actions, such as registering a member, send email notifications. You should run a [local email emulator like SMTP4Dev or Papercut](https://ardalis.com/configuring-a-local-test-email-server/) to capture these, or configure your local environment to use a fake email sender class.

You should create an **appsettings.development.json** file to hold your other connection strings such as for Azure Storage. You can use [Azurite](https://github.com/Azure/Azurite) as a local emulator for this.

For the Discord web hook integration, you can use the `dev-test` channel in devBetter's Discord server. The web hook url is in the channel description on Discord. You can use that in you appsettings.development.json. Alternatively, you can set up your own Discord server - see [here](https://ardalis.com/add-discord-notifications-to-asp-net-core-apps/) for a walkthrough -  and add the url to  appsettings.development.json in the Discord Webhooks section that you can copy from appsettings.json. You could also create a mock server which will provide you with a url to use - an example is mocky.io

### EF Migrations Commands

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
dotnet ef database update -c AppDbContext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj
dotnet ef database update -c IdentityDbContext -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj
```

Generate Idempotent Update Script (for production)(from the DevBetter.Web folder):

```powershell
dotnet ef migrations script -c AppDbContext -i -o migrate.sql -p ../DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj -s DevBetterWeb.Web.csproj
```

### Run with .NET Aspire

The `DevBetterWeb.AppHost` project orchestrates the app for local development using [.NET Aspire](https://aspire.dev/). It:

- Starts a SQL Server container (`sql`) with a persistent data volume and creates the `DevBetterWeb` database (`devbetterdb`)
- Runs `DevBetterWeb.Web` in the `Development` environment, passing it `ConnectionStrings:DefaultConnection` and waiting for the database to be ready
- Applies the EF Core migrations for both `AppDbContext` and `IdentityDbContext` and seeds test data on startup
- Opens the Aspire dashboard with logs, traces, metrics, and health for every resource

Prerequisites: the .NET 11 SDK and a running container runtime ([Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/)). The [Aspire CLI](https://aspire.dev/get-started/install-cli/) is optional.

From the root of the repo, run either:

```bash
aspire run
```

or

```bash
dotnet run --project src/DevBetterWeb.AppHost
```

Then open the dashboard login URL printed in the console. The web app is available at `https://localhost:5011` (and `http://localhost:5010`). In Visual Studio or Rider, set `DevBetterWeb.AppHost` as the startup project.

Notes:

- The SQL Server container uses a persistent lifetime and data volume, so it keeps running (and keeps its data) after the AppHost stops, which makes subsequent starts fast. Stop or delete the `sql-*` container and its volume from Docker to reset the database.
- The generated SQL Server `sa` password is stored in the AppHost's user secrets (`Parameters:sql-password`). To connect with a SQL client, use the connection string shown for `devbetterdb` in the dashboard.
- Other settings (Stripe, Vimeo, Discord, email, etc.) still come from the web project's `appsettings.json`/user secrets, as described above.
- `DevBetterWeb.ServiceDefaults` adds OpenTelemetry, health checks (`/health` and `/alive`), HTTP resilience, and service discovery to the web project. OpenTelemetry is only enabled when an OTLP endpoint is configured (as it is under Aspire), so production telemetry (Serilog and Application Insights) is unchanged. HTTP resilience (timeouts, circuit breaker, and retries for safe methods like GET) applies to every `IHttpClientFactory` client in all environments, except the Vimeo clients, which opt out because uploads are long-running.

### Run with Docker

Alternatively you can use `docker-compose` to run the app locally. This is specially helpful when launching the app in operative systems that don't have support for SQL Express like MacOS.

To run with docker compose, simply run in the root of the repo:

```bash
docker compose up
```

The multi-container app runs two services:
- The web project: at `http://localhost/`
- The SQL Edge container at `localhost:1433`.

By default the application will run in the `Development` environment, and the SQL migrations will be applied programatically by the web project container during startup.

If you want to access the database outside of the app you will need the local password for the database, which you can find in this [.env file](https://github.com/DevBetterCom/DevBetterWeb/blob/main/.env). You can also find the full connection string as an environment variable for the web project with the name `ConnectionStrings:DefaultConnection` running in bash:

```bash
docker inspect \
	--format='{{range .Config.Env}}{{println .}}{{end}}' dev-better-web \
	| grep "ConnectionStrings:DefaultConnection=" \
	| cut -d '=' -f 2- \
	| sed 's/database/localhost/g'
```

Or in PowerShell:

```powershell
docker inspect `
	--format='{{range .Config.Env}}{{println .}}{{end}}' dev-better-web  `
	| Select-String "ConnectionStrings:DefaultConnection=" `
	| ForEach-Object { $_.Line -replace "database", "localhost" } `
	| ForEach-Object { ($_ -split "Connection=")[1] } 
```

To stop the services run:

```bash
docker compose down
```

## Video Upload Instructions (admin only)

Put the video files and their associated markdown files in a folder you wish to upload from. Specify the Vimeo token and devBetter API key.

For the API link, the production link should be the root web site, <https://devbetter.com/>

```powershell
.\DevBetterWeb.UploaderApp.exe -d [folder] -t [Vimeo token] -a [api link] -akey [api key]
```

## Stripe WebHook Development
Install ngrok for more information [here](https://ngrok.com) 
```powershell
	npm install ngrok -g 
```
If there is any security issue while you using it then run this command on pwoershell
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser
```
Active the forword by running this command
```powershell
ngrok http 5010
```
Save the link https://xxx-xxx-xxx-xxx.ngrok.io and create WebHook on your stripe account with this link. 
You need to check which version is supported by Stripe .NET and choose it while you creating the WebHook on your account.  
Copy the WebHook Secret from edit WebHook and insert it on `appsettings.json` -> `StripeOptions` -> `StripeCustomerSubscriptionDeletedWebHookSecretKey`,  `StripeCustomerSubscriptionUpdatedWebHookSecretKey`, `StripeInvoicePaidWebHookSecretKey`.  
Make DevBetter application using http without redirect by comment this line `app.UseHttpsRedirection();` from `startup.cs`.    
Open the application on `http://localhost:5010/`.   
You can check PaymentIntentWebHook endpoint for more information.    
![stripe-WebHook-1](https://user-images.githubusercontent.com/6225593/190837378-17d772ef-8669-41b8-bad0-ff24cffb6260.png)
![stripe-WebHook-2](https://user-images.githubusercontent.com/6225593/190837386-3b29fdfc-0617-4a4a-a01b-c2b724628bef.png)
![stripe-WebHook-3](https://user-images.githubusercontent.com/6225593/190837393-139eb304-5dd1-46bc-9f31-dd2e6d468118.png)
![stripe-WebHook-4](https://user-images.githubusercontent.com/6225593/190837398-f90d1b46-c1b2-445d-b303-095733441500.png)
![stripe-WebHook-5](https://user-images.githubusercontent.com/6225593/190940284-06cd9861-6745-4902-9bbe-4f0131d38ac2.png)

