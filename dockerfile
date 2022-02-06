FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./src/Scrummy.UI/Scrummy.UI.csproj"
RUN dotnet build "./src/Scrummy.UI/Scrummy.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./src/Scrummy.UI/Scrummy.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Scrummy.UI.dll"]