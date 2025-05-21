
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

USER root
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
    ffmpeg \
    libreoffice \
    imagemagick \
    pandoc \
 && rm -rf /var/lib/apt/lists/* \
 && ln -s /usr/bin/soffice /usr/local/bin/libreoffice \
 && ln -s /usr/bin/convert /usr/local/bin/magick


USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SimpleFileConverter.API/SimpleFileConverter.API.csproj", "src/SimpleFileConverter.API/"]
RUN dotnet restore "./src/SimpleFileConverter.API/SimpleFileConverter.API.csproj"
COPY . .
WORKDIR "/src/src/SimpleFileConverter.API"
RUN dotnet build "SimpleFileConverter.API.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SimpleFileConverter.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleFileConverter.API.dll"]
