using Context;
using Dota2MetaChecker.TelegramBot;
using Dota2MetaChecker.TelegramBot.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Contracts.Avatars;
using Services.Contracts.Data_sync;
using Services.Contracts.Deserialization;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using Services.Data_sync.CallbackHandlers;
using Services.Deserialization;
using Services.Formatting;
using Services.Formatting.ImageGenerators;
using Services.Formatting.ImageGenerators.ImageProviders;
using Services.Processing;
using Services.Processing.StrategiesOfSorting;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

// Скрываем логи Entity Framework (уровень Warning и выше)
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

// === КОНФИГУРАЦИЯ БАЗЫ ДАННЫХ ===
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === КЭШИРОВАНИЕ ===
builder.Services.AddSingleton<HeroesDataCache>();

// === ВНЕШНИЕ API КЛИЕНТЫ ===
// Telegram Bot API
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["Telegram:Token"] ?? throw new Exception("Token not found")));

// Stratz API (источник данных о героях)
builder.Services.AddSingleton<IStratzApiService>(_ =>
    new StratzApiService(builder.Configuration["StratzApi:Token"] ?? string.Empty));

// === ПАРСИНГ И ОБРАБОТКА ДАННЫХ ===
builder.Services.AddSingleton<IStratzHeroParser, StratzHeroParser>();
builder.Services.AddSingleton<IHeroStatsFilterService, HeroStatsFilterService>();
builder.Services.AddSingleton<IHeroStatsAggregator, HeroStatsAggregator>();
builder.Services.AddSingleton<IHeroCalculator, HeroCalculator>();

// === ФОРМАТИРОВАНИЕ И ОТОБРАЖЕНИЕ ===
builder.Services.AddSingleton<IHeroInfoFormatter, HeroInfoFormatter>();
builder.Services.AddSingleton<IHeroStatsProcessor, HeroStatsProcessor>();
builder.Services.AddSingleton<IHeroesDataService, HeroesDataService>();

// === ПОЛЬЗОВАТЕЛЬСКИЙ ИНТЕРФЕЙС ===
// Клавиатуры и сообщения
builder.Services.AddSingleton<IHeroesKeyboardBuilder, HeroesKeyboardBuilder>();
builder.Services.AddSingleton<IHeroesListMessageBuilder, HeroesListMessageBuilder>();
builder.Services.AddSingleton<IBotMessageRenderer, BotMessageRenderer>();

// Генерация изображений
builder.Services.AddSingleton<IHeroesImageBuilder, HeroesImageBuilder>();
builder.Services.AddSingleton<IImageGenerator, HeroOptionsImageGenerator>();

// Аватарки героев (с HTTP-клиентом)
builder.Services.AddSingleton<IHeroAvatarProvider, HeroAvatarProvider>();
builder.Services.AddHttpClient<HeroAvatarProvider>();
builder.Services.AddSingleton<IHeroAvatarProvider>(sp =>
    new FallbackHeroAvatarProvider(
        sp.GetRequiredService<HeroAvatarProvider>(),
        sp.GetRequiredService<HeroesDataCache>() 
    ));

// === ОБРАБОТЧИКИ CALLBACK-ЗАПРОСОВ ===
builder.Services.AddSingleton<ICallbackHandler, ToPageCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, RankCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, RoleCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, SortCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, ClearOptionsCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, SwitchFormatCallbackHandler>();

// === СТРАТЕГИИ СОРТИРОВКИ ===
builder.Services.AddSingleton<IHeroSortStategy, MatchCountSortStrategy>();
builder.Services.AddSingleton<IHeroSortStategy, WinrateSortStrategy>();
builder.Services.AddSingleton<IHeroSortStategy, RatingSortStrategy>();
builder.Services.AddSingleton<IHeroSortStategy, WinrateDeltaSortStrategy>();
builder.Services.AddSingleton<IHeroSortStategy, PickrateDeltaSortStrategy>();
builder.Services.AddSingleton<IHeroSortStategy, RatingDeltaSortStrategy>();

// === СЕРВИСЫ ПОЛЬЗОВАТЕЛЬСКИХ НАСТРОЕК ===
builder.Services.AddSingleton<IUserPreferencesService, UserPreferencesService>();

// === ФОНОВЫЕ СЕРВИСЫ ===
// Синхронизация статистики героев с БД
builder.Services.AddHostedService<HeroesDataSyncService>();

// === САМ БОТ ===
builder.Services.AddSingleton<Bot>();



var host = builder.Build();

// Применяем миграции при старте (опционально)
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await db.Database.MigrateAsync();
}

// 3. Запуск хоста (стартует все IHostedService, включая HeroDataUpdateHostedService)
await host.StartAsync();

// 4. Запуск бота
var bot = host.Services.GetRequiredService<Bot>();
var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.WriteLine("Telegram бот запущен. Нажмите Ctrl+C для выхода.");

try
{
    await bot.StartReceivingAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Бот остановлен.");
}
finally
{
    await host.StopAsync();
}