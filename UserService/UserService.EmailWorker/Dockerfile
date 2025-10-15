# Etapa base (runtime) - Worker doesn't need ASP.NET, just .NET runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto (.csproj)
COPY ["UserService.EmailWorker/UserService.EmailWorker.csproj", "UserService.EmailWorker/"]
COPY ["UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]
COPY ["UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]

# Restaura dependências
RUN dotnet restore "UserService.EmailWorker/UserService.EmailWorker.csproj"

# Copia todo o código fonte
COPY . .

# Compila
WORKDIR "/src/UserService.EmailWorker"
RUN dotnet build "UserService.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UserService.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "UserService.EmailWorker.dll"]