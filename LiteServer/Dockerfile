#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LiteServer/LiteServer.csproj", "LiteServer/"]
COPY ["LiteNetLib/LiteNetLib.csproj", "LiteNetLib/"]
RUN dotnet restore "LiteServer/LiteServer.csproj"
COPY . .
WORKDIR "/src/LiteServer"
RUN dotnet build "LiteServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiteServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiteServer.dll"]