# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY FormEngineAPI.csproj ./
RUN dotnet restore FormEngineAPI.csproj

# Copy everything else and build
COPY . .
RUN dotnet build FormEngineAPI.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish FormEngineAPI.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a script to wait for MySQL and run migrations
RUN apt-get update && apt-get install -y netcat-traditional

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "FormEngineAPI.dll"]
