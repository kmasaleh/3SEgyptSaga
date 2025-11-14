# Use .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ./EMailService/*.csproj  ./EMailService/
COPY ./Contracts/*.csproj ./Contracts/
RUN dotnet restore ./EMailService/EMailService.csproj

# Copy everything else and build
COPY ./EMailService/. ./EMailService/
COPY ./Contracts/. ./Contracts/
WORKDIR /src/EMailService/
RUN dotnet publish -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "EMailService.dll"]
