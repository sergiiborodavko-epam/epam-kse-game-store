FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EpamKse.GameStore.PaymentService/*.csproj ./EpamKse.GameStore.PaymentService/
COPY EpamKse.GameStore.Domain/*.csproj ./EpamKse.GameStore.Domain/

RUN dotnet restore ./EpamKse.GameStore.PaymentService/EpamKse.GameStore.PaymentService.csproj

COPY EpamKse.GameStore.PaymentService/ ./EpamKse.GameStore.PaymentService/
COPY EpamKse.GameStore.Domain/ ./EpamKse.GameStore.Domain/

WORKDIR /src/EpamKse.GameStore.PaymentService

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5172
EXPOSE 5172

ENTRYPOINT ["dotnet", "EpamKse.GameStore.PaymentService.dll"]