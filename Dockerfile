# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copiar archivos y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Publicar la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Configuración de red para Render
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# El nombre de tu DLL confirmado en tu build
ENTRYPOINT ["dotnet", "CarvaVault-API.dll"]