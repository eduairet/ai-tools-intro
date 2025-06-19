# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

# Use the official .NET runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
COPY wwwroot ./wwwroot

# Create Data directory for SQLite database
RUN mkdir -p /app/Data

# Expose port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Set environment variable for JWT key
ENV DOTNET_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "EventManagementApi.dll"] 