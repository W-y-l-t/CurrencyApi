ARG DOTNET_SDK_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS prepare-restore
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-subset --version 0.3.2 --no-cache

WORKDIR /src
COPY . .
RUN dotnet subset restore InternalApi/InternalApi.csproj \
    --root-directory /src \
    --output restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
WORKDIR /src
COPY --from=prepare-restore /src/restore_subset .
RUN dotnet restore InternalApi/InternalApi.csproj

COPY . .
RUN dotnet publish InternalApi/InternalApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_SDK_VERSION} AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5888
EXPOSE 5889

RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

LABEL maintainer="Dmitry Semenov dmi7rysemenov@gmail.com"

ENTRYPOINT ["dotnet", "InternalApi.dll"]
