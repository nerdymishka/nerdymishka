#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Nex/Nex.csproj", "src/Nex/"]
RUN dotnet restore "src/Nex/Nex.csproj"
COPY . .
WORKDIR "/src/src/Nex"
RUN dotnet build "Nex.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nex.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nex.dll"]