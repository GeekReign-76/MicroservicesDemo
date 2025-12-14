# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files
COPY . ./

# Publish app
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/out ./

# Expose port
EXPOSE 4000

ENTRYPOINT ["dotnet", "BffApi.dll"]
