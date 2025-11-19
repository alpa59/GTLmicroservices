# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SearchService/Contracts ./Contracts/
COPY WarehouseService/WarehouseService.csproj .
RUN dotnet restore "WarehouseService.csproj"
COPY WarehouseService/. .
RUN dotnet publish "WarehouseService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WarehouseService.dll"]
