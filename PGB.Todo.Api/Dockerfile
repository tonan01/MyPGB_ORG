FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép các file .csproj và restore dependencies trước
COPY ["PGB.Todo.Api/PGB.Todo.Api.csproj", "PGB.Todo.Api/"]
COPY ["PGB.Todo.Application/PGB.Todo.Application.csproj", "PGB.Todo.Application/"]
COPY ["PGB.Todo.Domain/PGB.Todo.Domain.csproj", "PGB.Todo.Domain/"]
COPY ["PGB.Todo.Infrastructure/PGB.Todo.Infrastructure.csproj", "PGB.Todo.Infrastructure/"]
COPY ["PGB.BuildingBlocks.Application/PGB.BuildingBlocks.Application.csproj", "PGB.BuildingBlocks.Application/"]
COPY ["PGB.BuildingBlocks.Domain/PGB.BuildingBlocks.Domain.csproj", "PGB.BuildingBlocks.Domain/"]
COPY ["PGB.BuildingBlocks.WebApi.Common/PGB.BuildingBlocks.WebApi.Common.csproj", "PGB.BuildingBlocks.WebApi.Common/"]
RUN dotnet restore "PGB.Todo.Api/PGB.Todo.Api.csproj"

# Sao chép toàn bộ source code và build
COPY . .
WORKDIR "/src/PGB.Todo.Api"
RUN dotnet build "PGB.Todo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PGB.Todo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PGB.Todo.Api.dll"]