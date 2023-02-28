FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY payment-gateway/src/ .

RUN dotnet restore Bank.Processor/Bank.Processor.csproj
WORKDIR /Bank.Processor
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build Bank.Processor/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Bank.Processor.dll"]