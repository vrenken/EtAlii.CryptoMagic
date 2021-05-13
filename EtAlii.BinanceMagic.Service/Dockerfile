FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EtAlii.BinanceMagic.Service/EtAlii.BinanceMagic.Service.csproj", "EtAlii.BinanceMagic.Service/"]
RUN dotnet restore "EtAlii.BinanceMagic.Service/EtAlii.BinanceMagic.Service.csproj"
COPY . .
WORKDIR "/src/EtAlii.BinanceMagic.Service"
RUN dotnet build "EtAlii.BinanceMagic.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EtAlii.BinanceMagic.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtAlii.BinanceMagic.Service.dll"]
