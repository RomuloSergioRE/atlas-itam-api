FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Atlas.Itam.Api/Atlas.Itam.Api.csproj", "src/Atlas.Itam.Api/"]
COPY ["src/Atlas.Itam.Application/Atlas.Itam.Application.csproj", "src/Atlas.Itam.Application/"]
COPY ["src/Atlas.Itam.Domain/Atlas.Itam.Domain.csproj", "src/Atlas.Itam.Domain/"]
COPY ["src/Atlas.Itam.Infrastructure/Atlas.Itam.Infrastructure.csproj", "src/Atlas.Itam.Infrastructure/"]
RUN dotnet restore "src/Atlas.Itam.Api/Atlas.Itam.Api.csproj"
COPY . .
WORKDIR "/src/src/Atlas.Itam.Api"
RUN dotnet build "Atlas.Itam.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Atlas.Itam.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Atlas.Itam.Api.dll"]
