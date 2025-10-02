# Etapa de build com SDK 6.0
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Instalar dependências para SSL (necessário para Supabase/Postgres)
RUN apt-get update && apt-get install -y libssl-dev ca-certificates && update-ca-certificates

# Copiar csproj(s) e restaurar (melhora cache)
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código e publicar
COPY . ./
RUN dotnet publish -c Release -o /app

# Imagem final menor com runtime 6.0
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Instalar suporte a SSL no runtime também
RUN apt-get update && apt-get install -y libssl-dev ca-certificates && update-ca-certificates

COPY --from=build /app .

# Variável padrão para a porta
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "UserService.dll"]
