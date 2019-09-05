# ASP.NET Core 2.2.x Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

# ASP.NET Core 2.2.x SDK and Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
ARG TAG=master

ENV HACC_VERSION $TAG

WORKDIR /

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl git nano
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && apt-get install -yq nodejs npm build-essential
RUN npm install -g npm

RUN mkdir /src \
    && cd /src \
    && wget -qO src.tar.gz https://github.com/qJake/HADotNet.CommandCenter/archive/$TAG.tar.gz \
    && tar xzf src.tar.gz --strip=1 --exclude pax_global_header \
    && rm -rf src.tar.gz

WORKDIR /src/HADotNet.CommandCenter

RUN dotnet restore HADotNet.CommandCenter.csproj

RUN npm install

RUN dotnet build HADotNet.CommandCenter.csproj -c Release -o /app

# Publish Command
FROM build AS publish
RUN dotnet publish HADotNet.CommandCenter.csproj -c Release -o /app

# Pull publish into runtime
FROM base AS final
ARG TAG=master

WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS http://*:8095
ENV ASPNETCORE_ENVIRONMENT Production
ENV HACC_VERSION $TAG

EXPOSE 8095

ENTRYPOINT ["dotnet", "HADotNet.CommandCenter.dll"]
