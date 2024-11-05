FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CityNexus.Api/CityNexus.Api.csproj", "src/CityNexus.Api/"]
RUN dotnet restore "src/CityNexus.Api/CityNexus.Api.csproj"
COPY . .
WORKDIR "/src/src/CityNexus.Api"
RUN dotnet build "CityNexus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CityNexus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
## Install the agent
#RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
#&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
#&& wget https://download.newrelic.com/548C16BF.gpg \
#&& apt-key add 548C16BF.gpg \
#&& apt-get update \
#&& apt-get install -y 'newrelic-dotnet-agent' \
#&& rm -rf /var/lib/apt/lists/*
#
## Enable the agent
#ENV CORECLR_ENABLE_PROFILING=1 \
#CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
#CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
#CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
#NEW_RELIC_LICENSE_KEY="" \
#NEW_RELIC_APP_NAME="CityNexus.People"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityNexus.Api.dll"]
