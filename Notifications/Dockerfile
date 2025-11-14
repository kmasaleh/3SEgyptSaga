# Use .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ./Notifications/Notifications.csproj  ./Notifications/
COPY ./Contracts/Contracts.csproj ./Contracts/

RUN dotnet restore ./Notifications/Notifications.csproj

# Copy everything else and build
COPY ./Notifications/. ./Notifications/
COPY ./Contracts/. ./Contracts/
WORKDIR /src/Notifications/
RUN dotnet publish -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Notifications.dll"]
