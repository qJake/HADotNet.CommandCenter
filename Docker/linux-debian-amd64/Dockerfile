# ASP.NET Core 2.2.x Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 8095

# ASP.NET Core 2.2.x SDK and Build
FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build

ENV HACC_VERSION 0.4.1

WORKDIR /

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm

RUN mkdir /src \
    && cd /src \
    && wget -O src.tar.gz https://github.com/qJake/HADotNet.CommandCenter/archive/v$HACC_VERSION.tar.gz \
    && tar xzf src.tar.gz \
    && rm -rf src.tar.gz

WORKDIR /src/HADotNet.CommandCenter-$HACC_VERSION/HADotNet.CommandCenter

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

ENTRYPOINT ["dotnet", "HADotNet.CommandCenter.dll"]
