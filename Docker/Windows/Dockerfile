# ASP.NET Core 2.2.x Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 8095

# ASP.NET Core 2.2.x SDK and Build
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

WORKDIR /

# TODO: Pull from GitHub and install dependencies like npm...?

WORKDIR /src/HADotNet.CommandCenter-master/HADotNet.CommandCenter

RUN dotnet restore HADotNet.CommandCenter.csproj

RUN npm install

RUN dotnet build HADotNet.CommandCenter.csproj -c Release -o /app

# Publish Command
FROM build AS publish
RUN dotnet publish HADotNet.CommandCenter.csproj -c Release -o /app

# Pull publish into runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS http://*:8095
ENV ASPNETCORE_ENVIRONMENT Production

EXPOSE 8095

ENTRYPOINT ["dotnet", "HADotNet.CommandCenter.dll"]