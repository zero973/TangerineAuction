FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 10000
EXPOSE 10001


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS with-node
RUN apt-get update
RUN apt-get install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs


FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TangerineAuction.Server/TangerineAuction.Server.csproj", "TangerineAuction.Server/"]
COPY ["tangerineauction.client/tangerineauction.client.esproj", "tangerineauction.client/"]
RUN dotnet restore "./TangerineAuction.Server/TangerineAuction.Server.csproj"
COPY . .
WORKDIR "/src/TangerineAuction.Server"
RUN dotnet build "./TangerineAuction.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TangerineAuction.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TangerineAuction.Server.dll"]
