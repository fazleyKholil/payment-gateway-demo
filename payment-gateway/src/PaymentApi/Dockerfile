FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY payment-gateway/src/ .

RUN dotnet restore PaymentApi/PaymentApi.csproj
WORKDIR /PaymentApi
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build PaymentApi/out .

EXPOSE 8081
ENTRYPOINT ["dotnet", "PaymentApi.dll"]