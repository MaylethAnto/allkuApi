# Etapa 1: Construcción del proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura dependencias
COPY ["AllkuApi.csproj", "./"]
RUN dotnet restore "./AllkuApi.csproj"

# Copia el resto del código y compila
COPY . .
RUN dotnet build "AllkuApi.csproj" -c Release -o /app/build

# Etapa 2: Publicación del proyecto
FROM build AS publish
RUN dotnet publish "AllkuApi.csproj" -c Release -o /app/publish

# Etapa 3: Imagen final para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AllkuApi.dll"]