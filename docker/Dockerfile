# -----------------------------
# Etapa 1: Build (compilación)
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 

WORKDIR /src

# Copiar csproj y restaurar dependencias
# El contexto de build será la raíz del proyecto, así que copiamos src/
COPY ["src/SistemaGestionCitasHospital.csproj", "./"]
RUN dotnet restore "SistemaGestionCitasHospital.csproj"

# Copiar el resto del código
COPY src/ .
RUN dotnet build "SistemaGestionCitasHospital.csproj" -c Release -o /app/build

# -----------------------------
# Etapa 2: Publish (publicación)
# -----------------------------
FROM build AS publish
RUN dotnet publish "SistemaGestionCitasHospital.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------
# Etapa 3: Runtime (ejecución)
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SistemaGestionCitasHospital.dll"]
