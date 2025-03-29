#buid
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "/StreamingServerApi/StreamingServerApi.csproj" --disable-parallel
RUN dotnet publish "/StreamingServerApi/StreamingServerApi.csproj" -c release -o /app --no-restore

#serve
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS serve
WORKDIR /app
COPY --from=build /app .

EXPOSE 5001

ENTRYPOINT [ "dotnet", "StreamingServerApi.dll" ]
