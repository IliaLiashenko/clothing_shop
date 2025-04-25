
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["clothing_shop/clothing_shop.csproj", "clothing_shop/"]
COPY ["Shop_DataAccess/Shop_DataAccess.csproj", "Shop_DataAccess/"]
COPY ["Shop_Models/Shop_Models.csproj", "Shop_Models/"]
COPY ["Shop_Utility/Shop_Utility.csproj", "Shop_Utility/"]
RUN dotnet restore "./clothing_shop/clothing_shop.csproj"
COPY . .
WORKDIR "/src/clothing_shop"
RUN dotnet build "./clothing_shop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./clothing_shop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "clothing_shop.dll"]