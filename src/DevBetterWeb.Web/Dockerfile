# RUN JUST THIS CONTAINER FROM ROOT (folder with .sln file):
# docker build --pull -t web -f src/DevBetterWeb.Web/Dockerfile .
#
# RUN COMMAND
#  docker run --name devbetterweb --rm -it -p 5000:80 web
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

COPY *.sln .
COPY . .
WORKDIR /app/src/DevBetterWeb.Web
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/src/DevBetterWeb.Web/out ./

# Optional: Set this here if not setting it from docker-compose.yml
# ENV ASPNETCORE_ENVIRONMENT Development

ENTRYPOINT ["dotnet", "DevBetterWeb.Web.dll"]