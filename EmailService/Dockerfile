FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/EmailService.WebApi/EmailService.WebApi.csproj", "EmailService.WebApi/"]
COPY ["/EmailService.Infrastructure/EmailService.Infrastructure.csproj", "EmailService.Infrastructure/"]
COPY ["/EmailService.Core/EmailService.Core.csproj", "EmailService.Core/"]
COPY ["/EmailService.Application/EmailService.Application.csproj", "EmailService.Application/"]
RUN dotnet restore "EmailService.WebApi/EmailService.WebApi.csproj"
COPY . .
WORKDIR "/src/EmailService.WebApi"
RUN dotnet build "./EmailService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EmailService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmailService.WebApi.dll"]
