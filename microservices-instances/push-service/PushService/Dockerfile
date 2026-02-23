FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/PushService.WebApi/PushService.WebApi.csproj", "PushService.WebApi/"]
COPY ["/PushService.Infrastructure/PushService.Infrastructure.csproj", "PushService.Infrastructure/"]
COPY ["/PushService.Core/PushService.Core.csproj", "PushService.Core/"]
COPY ["/PushService.Application/PushService.Application.csproj", "PushService.Application/"]
RUN dotnet restore "PushService.WebApi/PushService.WebApi.csproj"
COPY . .
WORKDIR "/src/PushService.WebApi"
RUN dotnet build "./PushService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PushService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PushService.WebApi.dll"]
