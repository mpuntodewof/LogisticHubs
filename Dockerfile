FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY API/API.csproj API/
RUN dotnet restore API/API.csproj

# Copy everything and build
COPY . .
RUN dotnet publish API/API.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "API.dll"]
