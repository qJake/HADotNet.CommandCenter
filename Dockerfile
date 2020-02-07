FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 8095

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet build "HADotNet.CommandCenter.sln" -c Release -o /app
RUN dotnet publish "HADotNet.CommandCenter.sln" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS http://*:8095
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "HADotNet.CommandCenter.dll"]