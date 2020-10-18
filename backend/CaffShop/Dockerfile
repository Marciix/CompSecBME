# Build
FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app --no-restore

# Run
FROM mcr.microsoft.com/dotnet/core/aspnet:latest
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "CaffShop.dll"]
EXPOSE 80/tcp
