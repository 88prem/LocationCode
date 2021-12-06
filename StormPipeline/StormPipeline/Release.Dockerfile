FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app

COPY ["bin/Release/net5.0/", "."]
ENTRYPOINT ["dotnet", "StormPipeline.dll"]