using Context;
using Dota2MetaChecker.TelegramBot;
using Dota2MetaChecker.TelegramBot.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Contracts.Data_sync;
using Services.Contracts.Deserialization;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using Services.Data_sync.CallbackHandlers;
using Services.Deserialization;
using Services.Formatting;
using Services.Processing;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

//  Убираю логирование добавлений данных в БД
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

// 1. Настройка базы данных PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация сервисов 
builder.Services.AddSingleton<HeroesDataCache>();

// Регистрация API клиента через HttpClient 
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["Telegram:Token"] ?? throw new Exception("Token not found")));

// Регистрация бизнес-логики
builder.Services.AddSingleton<IStratzApiService>(_ =>
    new StratzApiService(builder.Configuration["StratzApi:Token"] ?? string.Empty));

builder.Services.AddSingleton<IStratzHeroParser, StratzHeroParser>();
builder.Services.AddSingleton<IHeroStatsFilterService, HeroStatsFilterService>();
builder.Services.AddSingleton<IHeroStatsAggregator, HeroStatsAggregator>();
builder.Services.AddSingleton<IHeroInfoFormatter, HeroInfoFormatter>();
builder.Services.AddSingleton<IHeroStatsProcessor, HeroStatsProcessor>();
builder.Services.AddSingleton<IHeroesDataService, HeroesDataService>();
builder.Services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
builder.Services.AddSingleton<IHeroesListMessageBuilder, HeroesListMessageBuilder>();
builder.Services.AddSingleton<IHeroStatDeltaCalculator, HeroStatDeltaCalculator>();

// Callback-обработчики для UserPreferencesService
builder.Services.AddSingleton<ICallbackHandler, PageCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, RankCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, RoleCallbackHandler>();
builder.Services.AddSingleton<ICallbackHandler, SortCallbackHandler>();

// Фоновый сервис для обновления и сохранения в БД статистики по героям
builder.Services.AddHostedService<HeroesDataSyncService>();

// Регистрация самого бота
builder.Services.AddSingleton<Dota2MetaBot>();

using var host = builder.Build();

// 3. Запуск хоста (стартует все IHostedService, включая HeroDataUpdateHostedService)
await host.StartAsync();

// 4. Запуск бота
var bot = host.Services.GetRequiredService<Dota2MetaBot>();
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