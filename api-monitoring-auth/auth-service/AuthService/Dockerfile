FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/AuthService.WebApi/AuthService.WebApi.csproj", "AuthService.WebApi/"]
COPY ["/AuthService.Application/AuthService.Application.csproj", "AuthService.Application/"]
COPY ["/AuthService.Core/AuthService.Core.csproj", "AuthService.Core/"]
COPY ["/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "AuthService.Infrastructure/"]
RUN dotnet restore "AuthService.WebApi/AuthService.WebApi.csproj"
COPY . .
WORKDIR "/src/AuthService.WebApi"
RUN dotnet build "./AuthService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.WebApi.dll"]
