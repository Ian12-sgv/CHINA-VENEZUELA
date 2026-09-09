# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and projects
COPY . .

# Restore and publish the API project
RUN dotnet restore src/ChinaVenezuela.Api/ChinaVenezuela.Api.csproj
RUN dotnet publish src/ChinaVenezuela.Api/ChinaVenezuela.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ChinaVenezuela.Api.dll"]
