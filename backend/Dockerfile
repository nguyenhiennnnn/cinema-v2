# syntax=docker/dockerfile:1
# Build context: ./backend (chua Cinema.Api, Cinema.Application, Cinema.Domain,
# Cinema.Infrastructure, Cinema.Shared). Xem .dockerignore de biet nhung gi bi loai tru
# (tests/, bin/, obj/, ...)

ARG DOTNET_VERSION=10.0

# ───────────────────────── Stage: restore ─────────────────────────
# Chi copy .csproj truoc de Docker cache lop restore NuGet — cache nay chi bi invalidate
# khi co thay doi dependency (PackageReference/ProjectReference), khong bi invalidate
# khi sua code (.cs), giup build lai nhanh hon nhieu.
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-alpine AS restore
WORKDIR /src

COPY Cinema.Api/Cinema.Api.csproj Cinema.Api/
COPY Cinema.Application/Cinema.Application.csproj Cinema.Application/
COPY Cinema.Domain/Cinema.Domain.csproj Cinema.Domain/
COPY Cinema.Infrastructure/Cinema.Infrastructure.csproj Cinema.Infrastructure/
COPY Cinema.Shared/Cinema.Shared.csproj Cinema.Shared/

RUN dotnet restore Cinema.Api/Cinema.Api.csproj

# ───────────────────────── Stage: build ─────────────────────────
FROM restore AS build
WORKDIR /src
COPY . .
RUN dotnet publish Cinema.Api/Cinema.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ───────────────────────── Stage: dev ─────────────────────────
# Dung cho docker-compose.yml (development). Ke thua lop "restore" (da co goi NuGet
# trong ~/.nuget/packages) roi chay `dotnet watch`. Source code thuc te duoc bind-mount
# tu host (xem docker-compose.yml), nen COPY o day chi la fallback khi chua mount.
FROM restore AS dev
WORKDIR /src

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

COPY . .

EXPOSE 8080

ENTRYPOINT ["dotnet", "watch", "--project", "Cinema.Api", "run", "--no-launch-profile", "--non-interactive"]

# ───────────────────────── Stage: production ─────────────────────────
# Anh huong nho: chi chua runtime, khong chua SDK hay source code.
# Dat o cuoi cung de lenh docker build mac dinh tren Render se build stage nay.
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS production
WORKDIR /app

# Cai dat ca-certificates de ho tro HTTPS outbound (Cloudinary, Stripe, v.v.)
RUN apk add --no-cache ca-certificates

COPY --from=build /app/publish .

# Khong chay voi root user (security best practice)
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_CLI_TELEMETRY_OPTOUT=1

EXPOSE 8080
ENTRYPOINT ["dotnet", "Cinema.Api.dll"]

