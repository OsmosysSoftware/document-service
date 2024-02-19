# Use the official .NET SDK image as the build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Expose the desired port
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 5000

# Copy data
COPY ["DocumentService.API/DocumentService.API.csproj", "DocumentService.API/"]
COPY ["DocumentService/DocumentService.csproj", "DocumentService/"]

# Restore the project dependencies
RUN dotnet restore "./DocumentService.API/./DocumentService.API.csproj"
RUN dotnet restore "./DocumentService/./DocumentService.csproj"

# Copy the rest of the data
COPY . .
WORKDIR "/app/DocumentService.API"

# Build the project and store artifacts in /out folder
RUN dotnet publish "./DocumentService.API.csproj" -c Debug -o /app/out

# Use the official ASP.NET runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

# Copy the published artifacts from the build stage
COPY --from=build /app/out .

# Install only necessary dependencies for wkhtmltopdf, Node.js and npm
RUN apt-get update \
    && apt-get install -y --fix-missing wkhtmltopdf \
    && apt-get install -y --fix-missing nodejs \
    && apt-get install -y --no-install-recommends npm \
    && rm -rf /var/lib/apt/lists/*

# Install wkhtmltopdf and allow execute access
RUN chmod 755 /usr/bin/wkhtmltopdf

# Install ejs globally without unnecessary dependencies
RUN npm install -g --only=prod ejs

# Set the environment variable for the application URL
ENV DOTNET_URLS=http://localhost:5000

# Set the entry point for the container
ENTRYPOINT ["dotnet", "DocumentService.API.dll"]
