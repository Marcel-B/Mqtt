FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
EXPOSE 8883

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# Copy only API for restore packages
COPY ["Mqtt.Server.csproj", "Mqtt.Server/"]

# Restore packages
RUN dotnet restore "Mqtt.Server/Mqtt.Server.csproj"


# Copy rest of the Project
COPY . ./Mqtt.Server/

# Build API
WORKDIR "/src/Mqtt.Server"

RUN dotnet build "Mqtt.Server.csproj" -c Release -o /app/build

# Publish
WORKDIR "/src/Mqtt.Server"
FROM build AS publish
RUN dotnet publish "Mqtt.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Mqtt.Server.dll"]
