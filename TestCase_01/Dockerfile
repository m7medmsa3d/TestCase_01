# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TestCase_01/TestCase_01.csproj", "TestCase_01/"]
COPY ["TestCase_01_DataAccess/TestCase_01_DataAccess.csproj", "TestCase_01_DataAccess/"]
COPY ["TestCase_01_DTO/TestCase_01_DTO.csproj", "TestCase_01_DTO/"]
RUN dotnet restore "./TestCase_01/TestCase_01.csproj"
COPY . .
WORKDIR "/src/TestCase_01"
RUN dotnet build "./TestCase_01.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TestCase_01.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestCase_01.dll"]