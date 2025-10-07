FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép các file .csproj và restore dependencies trước
COPY ["PGB.Chat.Api/PGB.Chat.Api.csproj", "PGB.Chat.Api/"]
COPY ["PGB.Chat.Application/PGB.Chat.Application.csproj", "PGB.Chat.Application/"]
COPY ["PGB.Chat.Domain/PGB.Chat.Domain.csproj", "PGB.Chat.Domain/"]
COPY ["PGB.Chat.Infrastructure/PGB.Chat.Infrastructure.csproj", "PGB.Chat.Infrastructure/"]
COPY ["PGB.BuildingBlocks.Application/PGB.BuildingBlocks.Application.csproj", "PGB.BuildingBlocks.Application/"]
COPY ["PGB.BuildingBlocks.Domain/PGB.BuildingBlocks.Domain.csproj", "PGB.BuildingBlocks.Domain/"]
COPY ["PGB.BuildingBlocks.WebApi.Common/PGB.BuildingBlocks.WebApi.Common.csproj", "PGB.BuildingBlocks.WebApi.Common/"]
RUN dotnet restore "PGB.Chat.Api/PGB.Chat.Api.csproj"

# Sao chép toàn bộ source code và build
COPY . .
WORKDIR "/src/PGB.Chat.Api"
RUN dotnet build "PGB.Chat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PGB.Chat.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PGB.Chat.Api.dll"]