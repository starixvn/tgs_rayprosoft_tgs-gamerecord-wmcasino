#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.WMCasino/Tgs.GameRecord.WMCasino.csproj", "Tgs.GameRecord.WMCasino/"]
COPY ["Tgs.GameRecord.WMCasino.Core/Tgs.GameRecord.WMCasino.Core.csproj", "Tgs.GameRecord.WMCasino.Core/"]
COPY ["Tgs.GameRecord.WMCasino.Common/Tgs.GameRecord.WMCasino.Common.csproj", "Tgs.GameRecord.WMCasino.Common/"]
RUN dotnet restore "./Tgs.GameRecord.WMCasino/./Tgs.GameRecord.WMCasino.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WMCasino"
RUN dotnet build "./Tgs.GameRecord.WMCasino.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.WMCasino.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WMCasino.dll"]