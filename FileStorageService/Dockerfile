FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FileStorageService/FileStorageService.csproj", "FileStorageService/"]
RUN dotnet restore "FileStorageService/FileStorageService.csproj"
COPY . .
WORKDIR "/src/FileStorageService"
RUN dotnet build "FileStorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileStorageService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "FileStorageService.dll"] 