#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Microservices.UsersService/Microservices.UsersService.csproj", "Microservices.UsersService/"]
RUN dotnet restore "Microservices.UsersService/Microservices.UsersService.csproj"
COPY . .
WORKDIR "/src/Microservices.UsersService"
RUN dotnet build "Microservices.UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservices.UsersService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Microservices.UsersService.dll"]