FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY EtAlii.CryptoMagic/bin/Release/net5.0/publish/ cryptomagic/
WORKDIR /CryptoMagic

EXPOSE 5001
VOLUME ["/cryptomagic/data"]
ENTRYPOINT ["dotnet", "EtAlii.CryptoMagic.dll"]