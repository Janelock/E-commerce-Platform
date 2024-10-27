FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /source
COPY . .
RUN dotnet restore "./ProductCatalog.csproj"
RUN dotnet publish "./ProductCatalog.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app ./

EXPOSE 5149

ENTRYPOINT ["dotnet", "ProductCatalog.dll"]

