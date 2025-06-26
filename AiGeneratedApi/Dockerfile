# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all source code
COPY . .

# Install SQLite and EF tools
RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev
RUN dotnet tool install --global dotnet-ef --version 9.0.6
ENV PATH="$PATH:/root/.dotnet/tools"

# Create folder for SQLite database
RUN mkdir -p /src/Data/Databases

# Create the SQLite DB file manually
RUN sqlite3 /src/Data/Databases/event_management.db ""

# Apply EF Core migrations using the new .db
RUN dotnet ef database update

# Publish the app to /app
RUN dotnet publish -c Release -o /app

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app ./

# Copy generated database file
COPY --from=build /src/Data/Databases /app/Data/Databases

# Configure ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "EventManagementApi.dll"]