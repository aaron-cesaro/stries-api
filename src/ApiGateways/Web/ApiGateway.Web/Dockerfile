#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ApiGateways/Web/ApiGateway.Web/ApiGateway.Web.csproj", "ApiGateways/Web/ApiGateway.Web/"]
RUN dotnet restore "ApiGateways/Web/ApiGateway.Web/ApiGateway.Web.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Web/ApiGateway.Web"
RUN dotnet build "ApiGateway.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.Web.dll"]