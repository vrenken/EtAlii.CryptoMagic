﻿FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY EtAlii.CryptoMagic/bin/Release/net5.0/publish/ cryptomagic/
WORKDIR /cryptomagic

EXPOSE 80
VOLUME ["/cryptomagic/data"]
ENV SERILOG_SERVER_URL=http://seq.avalon:5341

ENTRYPOINT ["dotnet", "EtAlii.CryptoMagic.dll"]