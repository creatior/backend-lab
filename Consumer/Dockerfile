FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Consumer/Consumer.csproj Consumer/
COPY UniverseLab.Common/UniverseLab.Common.csproj UniverseLab.Common/
COPY Models/Models.csproj Models/
COPY UniverseLab.Messages/UniverseLab.Messages.csproj UniverseLab.Messages/
RUN dotnet restore Consumer/Consumer.csproj

COPY . .

RUN dotnet publish Consumer/Consumer.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Consumer.dll"]
