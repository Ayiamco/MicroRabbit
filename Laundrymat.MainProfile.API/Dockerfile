#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Laundrymat.MainProfile.API/Laundrymat.MainProfile.API.csproj", "Laundrymat.MainProfile.API/"]
RUN dotnet restore "Laundrymat.MainProfile.API/Laundrymat.MainProfile.API.csproj"
COPY . .
WORKDIR "/src/Laundrymat.MainProfile.API"
RUN dotnet build "Laundrymat.MainProfile.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Laundrymat.MainProfile.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Laundrymat.MainProfile.API.dll"]