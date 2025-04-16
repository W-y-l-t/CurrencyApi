FROM mcr.microsoft.com/dotnet/sdk:9.0 AS prepare-restore
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /src
COPY . .
RUN dotnet subset restore PublicApi/PublicApi.csproj --root-directory /src --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY --from=prepare-restore /src/restore_subset .
RUN dotnet restore PublicApi/PublicApi.csproj

COPY . .
RUN dotnet publish PublicApi/PublicApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 5887

VOLUME ["/app/logs"]
LABEL maintainer="Dmitry Semenov dmi7rysemenov@gmail.com"

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:5887/health || exit 1

ENTRYPOINT ["dotnet", "PublicApi.dll"]

