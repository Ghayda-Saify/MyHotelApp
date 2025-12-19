# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projects and restore dependencies
COPY ["MyHotelApp.Api/MyHotelApp.Api.csproj", "MyHotelApp.Api/"]
COPY ["MyHotelApp.Core/MyHotelApp.Core.csproj", "MyHotelApp.Core/"]
COPY ["MyHotelApp.Infrastructure/MyHotelApp.Infrastructure.csproj", "MyHotelApp.Infrastructure/"]
COPY ["MyHotelApp.Shared/MyHotelApp.Shared.csproj", "MyHotelApp.Shared/"]
RUN dotnet restore "MyHotelApp.Api/MyHotelApp.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/MyHotelApp.Api"
RUN dotnet build "MyHotelApp.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "MyHotelApp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyHotelApp.Api.dll"]
