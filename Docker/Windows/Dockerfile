# ASP.NET Core 2.2.x Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 8095

# ASP.NET Core 2.2.x SDK and Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

ENV NODE_FULL_NAME node-v10.16.3-win-x64
ENV NODE_VERSION 10.16.3

ENV HACC_VERSION 0.5.0

WORKDIR /

# Installs NPM... by hand. :/
RUN %SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -Command New-Item -ItemType directory -Path /build; \
    Invoke-WebRequest https://nodejs.org/dist/v${env:NODE_VERSION}/${env:NODE_FULL_NAME}.zip -OutFile /build/${env:NODE_FULL_NAME}.zip; \
    Expand-Archive -LiteralPath /build/${env:NODE_FULL_NAME}.zip /build; \
    $newPath = (Get-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH).path; \
    $nodeFullName = ${env:NODE_FULL_NAME}; \
    $newPath = $newPath + ';/build/' + $nodeFullName; \
    Set-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH -Value $newPath;

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