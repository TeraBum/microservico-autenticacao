# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código e publica
COPY . ./
RUN dotnet publish -c Release -o /app

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copia os arquivos publicados da etapa de build
COPY --from=build /app .

# Expõe a porta que o container vai usar
EXPOSE 80

# Ajuste aqui para o DLL correto gerado no build
# Troque 'UserService.dll' pelo nome exato do seu projeto/dll
ENTRYPOINT ["dotnet", "UserService.dll"]
