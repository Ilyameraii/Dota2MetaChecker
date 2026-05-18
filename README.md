# 🎮 Dota2MetaChecker

Telegram-бот на C# (.NET 10), который позволяет отслеживать актуальную мету героев в Dota 2 — прямо из мессенджера. Данные о статистике героев берутся из **Stratz API** в реальном времени.

---

## 📋 Возможности

- Получение списка героев с актуальной статой по текущему патчу
- Фильтрация героев **по роли**
- Фильтрация по **рангу**
- Постраничная навигация по результатам
- Управление полностью через **inline-кнопки** — никакого ручного ввода

---

## 🕹️ Как пользоваться

Бот управляется одной командой:

**`/start`** — запускает бота и отображает сообщение с набором кнопок.

Через кнопки можно настроить:

| Параметр | Варианты |
|---|---|
| **Роль** | Safelane, Midlane, Offlane, Support, Hard Support |
| **Ранг** | Uncalibrated, Herald-Guardian, Crusader-Archon, Legend-Ancient, Divine-Immortal |
| **Сортировка** | По винрейту, по количеству матчей, по рейтингу (`X * winrate + pickrate`), по росту винрейта за неделю, по росту матчей за неделю, по росту рейтинга за неделю |
| **Страница** | Навигация по списку героев |

После настройки параметров бот формирует и отображает актуальный список героев по выбранным фильтрам.

---

## 🛠️ Технологии

- [.NET 10](https://dotnet.microsoft.com/)
- [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)
- [Stratz API](https://stratz.com/api)

---

## 🚀 Запуск

### Предварительные требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Telegram Bot Token — получить у [@BotFather](https://t.me/BotFather)
- Stratz API Token — получить на [stratz.com/api](https://stratz.com/api)

### 1. Клонировать репозиторий

```bash
git clone https://github.com/Ilyameraii/Dota2MetaChecker.git
cd Dota2MetaChecker
```

### 2. Настроить конфигурацию

Откройте файл `appsettings.json` и укажите свои токены:

```json
{
  "TelegramBot": {
    "Token": "YOUR_TELEGRAM_BOT_TOKEN"
  },
  "Stratz": {
    "Token": "YOUR_STRATZ_API_TOKEN"
  }
}
```

> ⚠️ Не коммитьте `appsettings.json` с реальными токенами в публичный репозиторий. Добавьте его в `.gitignore`.

### 3. Запустить проект

```bash
dotnet run --project Dota2MetaChecker
```

---

## 📁 Структура проекта

```
Dota2MetaChecker/
├── Commands/          # Обработчики команд и callback-кнопок
├── Services/          # Логика работы со Stratz API
├── Models/            # Модели данных (герои, фильтры, параметры пользователя)
├── appsettings.json   # Конфигурация (токены)
└── Program.cs         # Точка входа
```

---

## 📖 Пример использования

1. Запустите бота и откройте чат с ним в Telegram.
2. Отправьте `/start` — появится сообщение с кнопками управления.
3. Выберите роль, например **Support**, и ранг **Divine/Immortal**.
4. Бот отобразит список актуальных героев с учётом выбранных фильтров.
5. Используйте кнопки навигации для перелистывания страниц.

---

## 🗄️ База данных

Проект использует **PostgreSQL** + **Entity Framework Core**.

### Настройка connection string

В `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=dota2_stats_heroes;Username=your_user;Password=your_password"
  }
}
```

На сервере рекомендуется передавать connection string через переменную окружения, не хранить его в файле:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=dota2_stats_heroes;Username=your_user;Password=your_password"
```

Или прописать в systemd-сервисе (см. секцию [Деплой](#-деплой-ubuntu--debian-systemd)):

```ini
Environment="ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=dota2_stats_heroes;Username=your_user;Password=your_password"
```

> .NET автоматически подхватывает переменные окружения с двойным подчёркиванием `__` как вложенные ключи конфигурации — они имеют приоритет над `appsettings.json`.

### Применение миграций

```bash
dotnet ef database update
```

Если `dotnet-ef` не установлен:

```bash
dotnet tool install --global dotnet-ef
```

---

## 🖥️ Деплой (Ubuntu / Debian, systemd)

### 1. Установите .NET 10 Runtime на сервере

```bash
sudo apt-get update && sudo apt-get install -y dotnet-runtime-10.0
```

### 2. Опубликуйте проект и скопируйте на сервер

```bash
dotnet publish -c Release -o ./publish
scp -r ./publish user@your-server:/opt/dota2metachecker
```

### 3. Создайте systemd-сервис

Создайте файл `/etc/systemd/system/dota2metachecker.service`:

```ini
[Unit]
Description=Dota2MetaChecker Telegram Bot
After=network.target

[Service]
WorkingDirectory=/opt/dota2metachecker
ExecStart=/usr/bin/dotnet /opt/dota2metachecker/Dota2MetaChecker.dll
Restart=always
Environment="TelegramBot__Token=YOUR_TELEGRAM_BOT_TOKEN"
Environment="Stratz__Token=YOUR_STRATZ_API_TOKEN"
Environment="ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=dota2meta;Username=your_user;Password=your_password"

[Install]
WantedBy=multi-user.target
```

> Токены передаются через переменные окружения — так безопаснее, чем хранить их в `appsettings.json` на сервере.

### 4. Запустите и добавьте в автозапуск

```bash
sudo systemctl daemon-reload
sudo systemctl enable dota2metachecker
sudo systemctl start dota2metachecker
```

Проверить статус:

```bash
sudo systemctl status dota2metachecker
```

Copyright (c) 2026 Ilyameraii

All rights reserved.
