# RUN JUST THIS CONTAINER FROM ROOT (folder with .sln file):
# docker build --pull -t web -f src/DevBetterWeb.Web/Dockerfile .
#
# RUN COMMAND
#  docker run --name devbetterweb --rm -it -p 5000:80 web
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.slnx .
COPY . .
WORKDIR /app/src/DevBetterWeb.Web
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/DevBetterWeb.Web/out ./

# Optional: Set this here if not setting it from docker-compose.yml
# ENV ASPNETCORE_ENVIRONMENT Development

ENTRYPOINT ["dotnet", "DevBetterWeb.Web.dll"]
