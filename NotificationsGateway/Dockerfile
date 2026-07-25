FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/NotificationsGateway.WebApi/NotificationsGateway.WebApi.csproj", "NotificationsGateway.WebApi/"]
COPY ["/NotificationsGateway.Application/NotificationsGateway.Application.csproj", "NotificationsGateway.Application/"]
COPY ["/NotificationsGateway.Core/NotificationsGateway.Core.csproj", "NotificationsGateway.Core/"]
COPY ["/NotificationsGateway.Infrastructure/NotificationsGateway.Infrastructure.csproj", "NotificationsGateway.Infrastructure/"]
RUN dotnet restore "NotificationsGateway.WebApi/NotificationsGateway.WebApi.csproj"
COPY . .
WORKDIR "/src/NotificationsGateway.WebApi"
RUN dotnet build "./NotificationsGateway.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NotificationsGateway.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationsGateway.WebApi.dll"]
