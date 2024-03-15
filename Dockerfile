# RUN JUST THIS CONTAINER FROM ROOT (folder with .sln file):
# docker build --pull -t web -f src/DevBetterWeb.Web/Dockerfile .
#
# RUN COMMAND
#  docker run --name devbetterweb --rm -it -p 5000:80 web
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Debug
#COPY ["src/DevBetterWeb.Web/DevBetterWeb.Web.csproj", "src/DevBetterWeb.Web/"]
#COPY ["src/DevBetterWeb.Core/DevBetterWeb.Core.csproj", "src/DevBetterWeb.Core/"]
#COPY ["src/DevBetterWeb.Infrastructure/DevBetterWeb.Infrastructure.csproj", "src/DevBetterWeb.Infrastructure/"]
COPY *.sln .
COPY . .
RUN dotnet restore  
#    "src/DevBetterWeb.Web/DevBetterWeb.Web.csproj"
#COPY . .
WORKDIR "/src/DevBetterWeb.Web"
RUN dotnet build "DevBetterWeb.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "DevBetterWeb.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Optional: Set this here if not setting it from docker-compose.yml
ENV ASPNETCORE_ENVIRONMENT=Local
ENTRYPOINT ["dotnet", "DevBetterWeb.Web.dll"]
