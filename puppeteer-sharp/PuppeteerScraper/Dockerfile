ARG NET_IMAGE=7.0-bullseye-slim

FROM mcr.microsoft.com/dotnet/aspnet:${NET_IMAGE} AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:${NET_IMAGE} AS build
WORKDIR /src
COPY ["./", "/PuppeteerScraper"]
RUN dotnet restore "/PuppeteerScraper/PuppeteerScraper.csproj"
COPY . .
WORKDIR "/PuppeteerScraper"
RUN dotnet build "PuppeteerScraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PuppeteerScraper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PuppeteerScraper.dll"]
