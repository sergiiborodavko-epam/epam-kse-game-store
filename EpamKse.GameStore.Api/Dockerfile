FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EpamKse.GameStore.Api.Prod.sln .
COPY EpamKse.GameStore.Api/*.csproj ./EpamKse.GameStore.Api/
COPY EpamKse.GameStore.Domain/*.csproj ./EpamKse.GameStore.Domain/
COPY EpamKse.GameStore.Services/*.csproj ./EpamKse.GameStore.Services/
COPY EpamKse.GameStore.DataAccess/*.csproj ./EpamKse.GameStore.DataAccess/

RUN dotnet restore

COPY EpamKse.GameStore.Api/. ./EpamKse.GameStore.Api/
COPY EpamKse.GameStore.Domain/. ./EpamKse.GameStore.Domain/
COPY EpamKse.GameStore.Services/. ./EpamKse.GameStore.Services/
COPY EpamKse.GameStore.DataAccess/. ./EpamKse.GameStore.DataAccess/


WORKDIR /src/EpamKse.GameStore.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5186
EXPOSE 5186

ENTRYPOINT ["dotnet", "EpamKse.GameStore.Api.dll"]