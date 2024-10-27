FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /source
COPY . .
RUN dotnet restore "./UserManagement.csproj"
RUN dotnet publish "./UserManagement.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app ./

EXPOSE 5036

ENTRYPOINT ["dotnet", "UserManagement.dll"]

