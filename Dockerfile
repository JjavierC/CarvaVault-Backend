# 1. Usamos el SDK de .NET 10 para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 2. Copiamos el .csproj y restauramos (capa de cache)
COPY *.csproj ./
RUN dotnet restore

# 3. Copiamos todo y publicamos en modo Release
COPY . ./
RUN dotnet publish -c Release -o out

# 4. Imagen ligera para correr la App (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# 5. Configuración de puertos para Render
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# 6. El ejecutable que encontramos en tu build
ENTRYPOINT ["dotnet", "CarvaVault-API.dll"]