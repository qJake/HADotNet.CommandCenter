FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 8095

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["HADotNet.CommandCenter/HADotNet.CommandCenter.csproj", "HADotNet.CommandCenter/"]
RUN dotnet restore "HADotNet.CommandCenter/HADotNet.CommandCenter.csproj"
COPY . .
WORKDIR "/src/HADotNet.CommandCenter"
RUN dotnet build "HADotNet.CommandCenter.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HADotNet.CommandCenter.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS http://*:8095
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "HADotNet.CommandCenter.dll"]