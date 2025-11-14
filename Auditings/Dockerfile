# Use .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ./Auditings/Auditings.csproj  ./Auditings/
COPY ./Contracts/Contracts.csproj ./Contracts/
RUN dotnet restore ./Auditings/Auditings.csproj



COPY ./Auditings/.  ./Auditings/
COPY ./Contracts/. ./Contracts/

WORKDIR /src/Auditings/
RUN dotnet publish -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Auditings.dll"]
