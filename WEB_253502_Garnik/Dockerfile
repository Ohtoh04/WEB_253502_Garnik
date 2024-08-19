#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WEB_253502_Garnik/WEB_253502_Garnik.csproj", "WEB_253502_Garnik/"]
RUN dotnet restore "./WEB_253502_Garnik/WEB_253502_Garnik.csproj"
COPY . .
WORKDIR "/src/WEB_253502_Garnik"
RUN dotnet build "./WEB_253502_Garnik.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WEB_253502_Garnik.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WEB_253502_Garnik.dll"]