# syntax=docker/dockerfile:1
ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS restore
WORKDIR /src
COPY Cinema.Api/Cinema.Api.csproj Cinema.Api/
COPY Cinema.Application/Cinema.Application.csproj Cinema.Application/
COPY Cinema.Domain/Cinema.Domain.csproj Cinema.Domain/
COPY Cinema.Infrastructure/Cinema.Infrastructure.csproj Cinema.Infrastructure/
COPY Cinema.Shared/Cinema.Shared.csproj Cinema.Shared/
RUN dotnet restore Cinema.Api/Cinema.Api.csproj

FROM restore AS build
COPY . .
RUN dotnet publish Cinema.Api/Cinema.Api.csproj -c Release -o /app/publish --no-restore

FROM restore AS dev
ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY . .
EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "--project", "Cinema.Api", "run", "--no-launch-profile", "--non-interactive"]

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS production
WORKDIR /app
RUN apk add --no-cache ca-certificates krb5-libs tzdata
COPY --from=build /app/publish .
RUN addgroup -S appgroup && adduser -S appuser -G appgroup && chown -R appuser:appgroup /app
USER appuser
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cinema.Api.dll"]
