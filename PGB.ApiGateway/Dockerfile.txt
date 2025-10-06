FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PGB.ApiGateway/PGB.ApiGateway.csproj", "PGB.ApiGateway/"]
RUN dotnet restore "PGB.ApiGateway/PGB.ApiGateway.csproj"
COPY . .
WORKDIR "/src/PGB.ApiGateway"
RUN dotnet build "PGB.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PGB.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PGB.ApiGateway.dll"]