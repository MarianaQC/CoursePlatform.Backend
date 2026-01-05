# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/CoursePlatform.Domain/CoursePlatform.Domain.csproj", "src/CoursePlatform.Domain/"]
COPY ["src/CoursePlatform.Application/CoursePlatform.Application.csproj", "src/CoursePlatform.Application/"]
COPY ["src/CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj", "src/CoursePlatform.Infrastructure/"]
COPY ["src/CoursePlatform.Api/CoursePlatform.Api.csproj", "src/CoursePlatform.Api/"]

RUN dotnet restore "src/CoursePlatform.Api/CoursePlatform.Api.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la aplicación
WORKDIR "/src/src/CoursePlatform.Api"
RUN dotnet build "CoursePlatform.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CoursePlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Exponer puerto
EXPOSE 8080

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando de inicio
ENTRYPOINT ["dotnet", "CoursePlatform.Api.dll"]