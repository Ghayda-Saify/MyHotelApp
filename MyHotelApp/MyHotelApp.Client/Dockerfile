# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projects and restore dependencies
COPY ["MyHotelApp.Client/MyHotelApp.Client.csproj", "MyHotelApp.Client/"]
COPY ["MyHotelApp.Shared/MyHotelApp.Shared.csproj", "MyHotelApp.Shared/"]
RUN dotnet restore "MyHotelApp.Client/MyHotelApp.Client.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/MyHotelApp.Client"
RUN dotnet build "MyHotelApp.Client.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "MyHotelApp.Client.csproj" -c Release -o /app/publish

# Runtime Stage (Nginx)
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
# Custom nginx config to handle SPA routing if needed
# COPY MyHotelApp.Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
