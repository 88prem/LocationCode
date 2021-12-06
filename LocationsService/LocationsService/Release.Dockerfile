FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app
EXPOSE 34021

COPY ["bin/Release/net5.0/", "."]
ENTRYPOINT ["dotnet", "LocationsService.dll"]
