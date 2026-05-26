FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CatalogService.API/CatalogService.API.csproj", "CatalogService.API/"]
COPY ["CatalogService.Application/CatalogService.Application.csproj", "CatalogService.Application/"]
COPY ["CatalogService.Domain/CatalogService.Domain.csproj", "CatalogService.Domain/"]
COPY ["CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "CatalogService.Infrastructure/"]
RUN dotnet restore "CatalogService.API/CatalogService.API.csproj"
COPY . .
WORKDIR "/src/CatalogService.API"
RUN dotnet publish "CatalogService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5290
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.API.dll"]
