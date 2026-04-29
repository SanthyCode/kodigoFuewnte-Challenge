# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos absolutamente todo el contenido de la carpeta backend al contenedor
COPY . .

# Nos movemos a la carpeta principal (API)
WORKDIR /src/Kodigo.API

# Restauramos dependencias y compilamos
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Etapa final (solo el runtime para que la imagen sea ligera)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Kodigo.API.dll"]