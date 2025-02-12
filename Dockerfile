# Etapa 1: Construcción del proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia el archivo de solución
COPY ["AllkuApi.sln", "."]

# Copia el archivo del proyecto
COPY ["AllkuApi/AllkuApi.csproj", "AllkuApi/"]

# Restaurar las dependencias
RUN dotnet restore "AllkuApi.sln"

# Copia el resto del código fuente
COPY . .

# Compilar el proyecto
RUN dotnet publish -c Release -o out AllkuApi/AllkuApi.csproj

# Imagen final para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AllkuApi.dll"]