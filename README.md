# Установка зависимостей

В проекте есть две части — frontend (Nuxt) и backend (ASP.NET Core). Чтобы всё заработало, нужно установить зависимости для обеих:

``` bash
cd frontend
npm install
```

``` bash
cd ../backend
dotnet restore
```

# Запуск проекта

## Фронтенд

Фронтенд можно запускать в двух режимах:

- Dev‑режим — работает с эмуляцией бэкенда через MSW.

- Prod‑режим — обращается к настоящему ASP.NET Core серверу (http://localhost:5000/api).

Команды для запуска:

- Dev-режим:
``` bash
npm run build:dev && npm run preview:dev
```

- Prod-режим:
``` bash
npm run build:prod && npm run preview:prod
```

В Dev‑режиме можно разрабатывать интерфейс без запуска бэкенда. В Prod‑режиме фронт общается с реальным сервером.

## Бэкенд

Бэкенд запускается так:

``` bash
cd backend
dotnet run
```

В VS Code можно запустить его под отладчиком, выбрав конфигурацию Backend (.NET Core Launch).

По умолчанию сервер стартует на http://localhost:5000.

# Настройка .env файлов

На фронтенде используются три файла окружения:

``` bash
.env
.env.development
.env.production
```

При запуске команд npm run build:dev или npm run build:prod содержимое соответствующего файла копируется в .env.

Файл .env.development:

``` bash
API_URL=http://localhost:3000
USE_MSW=true
```

Файл .env.production:

``` bash
API_URL=http://localhost:5000
USE_MSW=false
```

# База данных

В проекте используется PostgreSQL. Все данные о пользователях и задачах сохраняются именно в базе данных на PostgreSQL.

Чтобы бэкенд работал, нужно установить и запустить PostgreSQL локально или использовать доступный сервер.
Подключение к базе настраивается в файлах конфигурации проекта.

Пример строки подключения в appsettings.json:

``` bash
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=tasksdb;Username=postgres;Password=password"
}
```

В режиме разработки можно указать тестовую базу данных в файле appsettings.Development.json. Это удобно, чтобы не портить основную базу во время экспериментов.

# Список API эндпоинтов

## Авторизация и пользователи

- POST /api/auth/login — авторизация пользователя, возвращает токен.

- GET /api/users/by-email — получить пользователя по email.

- GET /api/auth/me — получить информацию о текущем авторизованном пользователе на основе токена в заголовке Authorization.

## Задачи

- GET /api/tasks — получить список всех задач.

- POST /api/tasks — создать новую задачу.

- PUT /api/tasks/:id — обновить задачу.

- DELETE /api/tasks/:id — удалить задачу.
