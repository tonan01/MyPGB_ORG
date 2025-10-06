FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép các file .csproj và restore dependencies trước
COPY ["PGB.Auth.Api/PGB.Auth.Api.csproj", "PGB.Auth.Api/"]
COPY ["PGB.Auth.Application/PGB.Auth.Application.csproj", "PGB.Auth.Application/"]
COPY ["PGB.Auth.Domain/PGB.Auth.Domain.csproj", "PGB.Auth.Domain/"]
COPY ["PGB.Auth.Infrastructure/PGB.Auth.Infrastructure.csproj", "PGB.Auth.Infrastructure/"]
COPY ["PGB.BuildingBlocks.Application/PGB.BuildingBlocks.Application.csproj", "PGB.BuildingBlocks.Application/"]
COPY ["PGB.BuildingBlocks.Domain/PGB.BuildingBlocks.Domain.csproj", "PGB.BuildingBlocks.Domain/"]
COPY ["PGB.BuildingBlocks.Infrastructure/PGB.BuildingBlocks.Infrastructure.csproj", "PGB.BuildingBlocks.Infrastructure/"]
COPY ["PGB.BuildingBlocks.WebApi.Common/PGB.BuildingBlocks.WebApi.Common.csproj", "PGB.BuildingBlocks.WebApi.Common/"]
RUN dotnet restore "PGB.Auth.Api/PGB.Auth.Api.csproj"

# Sao chép toàn bộ source code và build
COPY . .
WORKDIR "/src/PGB.Auth.Api"
RUN dotnet build "PGB.Auth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PGB.Auth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PGB.Auth.Api.dll"]