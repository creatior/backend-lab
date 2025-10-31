FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

COPY UniverseLab/UniverseLab.csproj UniverseLab/
COPY UniverseLab.Common/UniverseLab.Common.csproj UniverseLab.Common/
COPY Models/Models.csproj Models/
COPY Migrations/Migrations.csproj Migrations/
COPY UniverseLab.Messages/UniverseLab.Messages.csproj UniverseLab.Messages/
RUN dotnet restore "UniverseLab/UniverseLab.csproj"

COPY . .

RUN dotnet publish "UniverseLab/UniverseLab.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UniverseLab.dll"]