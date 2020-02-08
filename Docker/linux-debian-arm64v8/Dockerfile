# ASP.NET Core 3.x Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim-arm64v8 AS base
WORKDIR /app

# ASP.NET Core 3.x SDK and Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster-arm64v8 AS build
ARG TAG

ENV HACC_VERSION $TAG

WORKDIR /

RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && apt-get install -yq nodejs npm build-essential

# Needed for ARM architecture
RUN npm config set unsafe-perm true 
RUN npm install -g npm

RUN mkdir /src \
    && cd /src \
    && wget -qO src.tar.gz https://github.com/qJake/HADotNet.CommandCenter/archive/v$TAG.tar.gz \
    && tar xzf src.tar.gz --strip=1 --exclude pax_global_header \
    && rm -rf src.tar.gz \
	&& ln -s /usr/share/dotnet/sdk/3.0.100-preview9-014004/ /usr/share/dotnet/sdk/3.0.0 \
	&& ln -s /usr/share/dotnet/host/fxr/3.0.0-preview9-19423-09/ /usr/share/dotnet/host/fxr/3.0.0 \
	&& dotnet --info
	
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

# Labels
LABEL \
    io.hass.name="HACC" \
    io.hass.description="Wall-mountable tablet interface for Home Assistant." \
    io.hass.arch="aarch64" \
    io.hass.type="addon" \
    io.hass.version=${HACC_VERSION} \
    maintainer="qJake <https://github.com/qJake/>" \
    org.label-schema.description="Wall-mountable tablet interface for Home Assistant." \
    org.label-schema.name="HACC" \
    org.label-schema.schema-version="1.0" \    
    org.label-schema.url="https://github.com/qJake/HADotNet.CommandCenter/" \
    org.label-schema.usage="https://github.com/qJake/HADotNet.CommandCenter/README.md" \
    org.label-schema.vcs-url="https://github.com/qJake/HADotNet.CommandCenter/" \
    org.label-schema.vendor="qJake"
