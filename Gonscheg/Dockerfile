FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gonscheg/Gonscheg.csproj", "Gonscheg/"]
COPY ["Gonscheg.Infrastructure/Gonscheg.Infrastructure.csproj", "Gonscheg.Infrastructure/"]
COPY ["Gonscheg.Domain/Gonscheg.Domain.csproj", "Gonscheg.Domain/"]
COPY ["Gonscheg.Application/Gonscheg.Application.csproj", "Gonscheg.Application/"]
COPY ["Gonscheg.Shared/Gonscheg.Shared.csproj", "Gonscheg.Shared/"]
RUN dotnet restore "Gonscheg/Gonscheg.csproj"
COPY . .
WORKDIR "/src/Gonscheg"
RUN dotnet build "./Gonscheg.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Gonscheg.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gonscheg.dll"]