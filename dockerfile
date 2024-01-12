FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Копирование файлов проекта
COPY . ./

# Восстановление зависимостей
RUN dotnet restore

# Сборка проекта
RUN dotnet build -c Release -o out

# Второй этап сборки
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TelegramBot_Si02.dll"]