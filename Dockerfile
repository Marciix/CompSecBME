# Build parser
FROM gcc:latest AS parser

WORKDIR /parser
COPY ./parser .
RUN apt-get update && apt-get install -y make 
RUN make build-linux

# Build
FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /build
COPY ./backend/CaffShop .
RUN dotnet restore
RUN dotnet publish -o /app --no-restore

# Run
FROM mcr.microsoft.com/dotnet/core/aspnet:latest
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
RUN mkdir /app/Resources
COPY --from=build /app ./
COPY --from=parser /parser/Release/CaffParser.so /app/Resources/CaffParser.so
COPY backend/CaffShop/Resources/TestFiles /app/Resources/TestFiles
ENTRYPOINT ["dotnet", "CaffShop.dll"]
EXPOSE 80/tcp
