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
RUN apk add --no-cache krb5-libs tzdata
ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    # Disable runtime diagnostics IPC (named pipes) that crash on Alpine/musl
    DOTNET_EnableDiagnostics=0
COPY . .
EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "--project", "Cinema.Api", "run", "--no-launch-profile", "--non-interactive", "--no-hot-reload"]

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS production
WORKDIR /app
RUN apk add --no-cache ca-certificates krb5-libs tzdata
COPY --from=build /app/publish .
RUN addgroup -S appgroup && adduser -S appuser -G appgroup && chown -R appuser:appgroup /app
USER appuser
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    # GC tuning cho môi trường free-tier RAM giới hạn (~512 MB)
    # ConserveMemory=9: GC aggressively thu hồi bộ nhớ sau mỗi collection
    DOTNET_GCConserveMemory=9 \
    # GCHeapHardLimit: giới hạn cứng heap = 384 MB (để OS & stack dùng 128 MB còn lại)
    DOTNET_GCHeapHardLimit=402653184 \
    # Server GC tắt: dùng Workstation GC tiết kiệm hơn cho single-service container
    DOTNET_GCDynamicAdaptationMode=0
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cinema.Api.dll"]
