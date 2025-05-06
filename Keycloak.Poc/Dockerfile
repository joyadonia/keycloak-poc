FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Install Zscaler cert
RUN echo "Copying Zscaler.crt to /usr/local/share/ca-certificates"
COPY Zscaler.crt /usr/local/share/ca-certificates/Zscaler.crt
RUN chmod 644  /usr/local/share/ca-certificates/Zscaler.crt && update-ca-certificates

# Install ping and curl
RUN apt-get update && apt-get install -y iputils-ping curl
RUN chmod 777 /usr/bin/ping 

USER $APP_UID
WORKDIR /app
EXPOSE 5256


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Zscaler cert for build-time SSL trust (NuGet, etc.)
COPY Zscaler.crt /usr/local/share/ca-certificates/Zscaler.crt
RUN chmod 644 /usr/local/share/ca-certificates/Zscaler.crt && \
    update-ca-certificates

COPY ["nuget.config", "."]
COPY _containerNugets /src/_containerNugets

COPY Keycloak.Poc.csproj .
RUN dotnet restore "Keycloak.Poc.csproj" 

# Copy the rest of the code
COPY . .
WORKDIR "/src"
RUN echo "Listing files in /src/Keycloak.Poc directory before build:" && ls -la /src

RUN dotnet build "Keycloak.Poc.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Keycloak.Poc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keycloak.Poc.dll"]