#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Asp .NET MVC Core Mentoring Module1/Asp .NET MVC Core Mentoring.csproj", "Asp .NET MVC Core Mentoring Module1/"]
COPY ["Asp .NET Core Mentoring Module1.Logging/Asp .NET Core Mentoring.Logging.csproj", "Asp .NET Core Mentoring Module1.Logging/"]
COPY ["Asp .NET Core Mentoring Module1.Data/Asp .NET Core Mentoring.Data.csproj", "Asp .NET Core Mentoring Module1.Data/"]
COPY ["Asp .NET Core Mentoring Module1.Common/Asp .NET Core Mentoring.Common.csproj", "Asp .NET Core Mentoring Module1.Common/"]
COPY ["Asp. NET Core Mentoring Module1.Helpers/Asp. NET Core Mentoring.Helpers.csproj", "Asp. NET Core Mentoring Module1.Helpers/"]
RUN dotnet restore "Asp .NET MVC Core Mentoring Module1/Asp .NET MVC Core Mentoring.csproj"
COPY . .
WORKDIR "/src/Asp .NET MVC Core Mentoring Module1"
RUN dotnet build "Asp .NET MVC Core Mentoring.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Asp .NET MVC Core Mentoring.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asp .NET MVC Core Mentoring.dll"]
