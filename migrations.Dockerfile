ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_RUNTIME} AS base
ENV ASPNETCORE_URLS=http://+:7105
WORKDIR /home/app
EXPOSE 7105

FROM ${DOTNET_SDK} AS buildbase
WORKDIR /source

COPY EpamKse.GameStore.Api/*.csproj ./EpamKse.GameStore.Api/

RUN dotnet restore ./EpamKse.GameStore.Api/EpamKse.GameStore.Api.csproj

COPY EpamKse.GameStore.Api/. ./EpamKse.GameStore.Api/
COPY EpamKse.GameStore.Domain/. ./EpamKse.GameStore.Domain/
COPY EpamKse.GameStore.Services/. ./EpamKse.GameStore.Services/
COPY EpamKse.GameStore.DataAccess/. ./EpamKse.GameStore.DataAccess/

FROM buildbase AS migrations
RUN dotnet tool install --version 8.0.17 --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet-ef database update --project EpamKse.GameStore.DataAccess/EpamKse.GameStore.DataAccess.csproj --startup-project EpamKse.GameStore.Api/EpamKse.GameStore.Api.csproj