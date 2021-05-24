#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Post.Api/Post/Post.Api.csproj", "Services/Post/Post.Api/"]
RUN dotnet restore "Services/Post/Post.Api/Post.Api.csproj"
COPY . .
WORKDIR "/src/Services/Post/Post.Api"
RUN dotnet build "Post.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Api.dll"]