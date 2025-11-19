# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SearchService/Contracts ./Contracts/
COPY BookAddService/BookAddService.csproj .
RUN dotnet restore "BookAddService.csproj"
COPY BookAddService/. .
RUN dotnet publish "BookAddService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookAddService.dll"]
