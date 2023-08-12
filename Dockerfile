FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY ./src/ ./

WORKDIR /app/Tlis.WebApiGateway

RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "Tlis.WebApiGateway.dll" ]