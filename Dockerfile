### Build Project ###
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# --- Install Node.js and npm ---
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

# Copy everything
COPY . .

WORKDIR ./src/DScratch.Host
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release --no-restore --output ./out

### Build RUN Image ###
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/DScratch.Host/out ./

EXPOSE 8080
# Add helth check maybe?

ENTRYPOINT ["dotnet", "DScratch.Host.dll"]