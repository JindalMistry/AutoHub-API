FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore AutoHub.API/AutoHub.API.csproj

RUN dotnet publish AutoHub.API/AutoHub.API.csproj \
    -c Release \
    -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AutoHub.API.dll"]