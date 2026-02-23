FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/SmsService.WebApi/SmsService.WebApi.csproj", "SmsService.WebApi/"]
COPY ["/SmsService.Infrastructure/SmsService.Infrastructure.csproj", "SmsService.Infrastructure/"]
COPY ["/SmsService.Core/SmsService.Core.csproj", "SmsService.Core/"]
COPY ["/SmsService.Application/SmsService.Application.csproj", "SmsService.Application/"]
RUN dotnet restore "SmsService.WebApi/SmsService.WebApi.csproj"
COPY . .
WORKDIR "/src/SmsService.WebApi"
RUN dotnet build "./SmsService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmsService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmsService.WebApi.dll"]
