FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LanguageGuide.API/LanguageGuide.API.csproj", "LanguageGuide.API/"]
COPY ["LanguageGuide.Services/LanguageGuide.Services.csproj", "LanguageGuide.Services/"]
COPY ["LanguageGuide.ContentBuilder/LanguageGuide.ContentBuilder.csproj", "LanguageGuide.ContentBuilder/"]
RUN dotnet restore "LanguageGuide.API/LanguageGuide.API.csproj"
COPY . .
WORKDIR "/src/LanguageGuide.API"
RUN dotnet build "LanguageGuide.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LanguageGuide.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LanguageGuide.API.dll"]
