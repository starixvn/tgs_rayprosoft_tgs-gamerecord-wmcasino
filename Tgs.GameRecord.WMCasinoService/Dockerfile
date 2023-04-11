#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.WMCasinoService/Tgs.GameRecord.WMCasinoService.csproj", "Tgs.GameRecord.WMCasinoService/"]
COPY ["Tgs.GameRecord.WMCasinoService.Core/Tgs.GameRecord.WMCasinoService.Core.csproj", "Tgs.GameRecord.WMCasinoService.Core/"]
RUN dotnet restore "Tgs.GameRecord.WMCasinoService/Tgs.GameRecord.WMCasinoService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WMCasinoService"
RUN dotnet build "Tgs.GameRecord.WMCasinoService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.WMCasinoService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WMCasinoService.dll"]