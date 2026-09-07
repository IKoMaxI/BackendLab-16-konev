# JwtAuth

ASP.NET Core приложение с JWT-аутентификацией и ролевой авторизацией.

## Запуск

```bash
dotnet restore
dotnet run
```

- UI: `https://localhost:7199`
- Swagger: `https://localhost:7199/swagger`

## Тестовые пользователи

| Логин | Пароль | Роль |
|---|---|---|
| `admin` | `admin123` | Admin |
| `user` | `user123` | User |
| `moderator` | `moderator123` | Moderator |

В production-среде секрет JWT следует задавать через переменную окружения или защищённое хранилище конфигурации.
