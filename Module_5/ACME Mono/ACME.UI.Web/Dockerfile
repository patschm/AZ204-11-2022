FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src


COPY . .
WORKDIR "/src/."

RUN dotnet restore "ACME.UI.Web/ACME.UI.Web.csproj"
RUN dotnet build "ACME.UI.Web/ACME.UI.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACME.UI.Web/ACME.UI.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACME.UI.Web.dll"]
