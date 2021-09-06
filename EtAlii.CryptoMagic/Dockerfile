FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EtAlii.CryptoMagic/EtAlii.CryptoMagic.csproj", "EtAlii.CryptoMagic/"]
RUN dotnet restore "EtAlii.CryptoMagic/EtAlii.CryptoMagic.csproj"
COPY . .
WORKDIR "/src/EtAlii.CryptoMagic"
RUN dotnet build "EtAlii.CryptoMagic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EtAlii.CryptoMagic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtAlii.CryptoMagic.dll"]
