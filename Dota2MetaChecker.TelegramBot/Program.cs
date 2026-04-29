using Context;
using Dota2MetaChecker.TelegramBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using Repository.Contracts;
using Services.Contracts.Data_sync;
using Services.Contracts.Deserialization;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using Services.Deserialization;
using Services.Formatting;
using Services.Processing;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

// 1. Настройка базы данных PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация сервисов 
builder.Services.AddSingleton<HeroesDataCache>();

// Регистрация API клиента через HttpClient 
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
    new TelegramBotClient(builder.Configuration["Telegram:Token"] ?? throw new Exception("Token not found")));

// Регистрация бизнес-логики
builder.Services.AddSingleton<IStratzApiService>(sp =>
    new StratzApiService(builder.Configuration["StratzApi:Token"] ?? string.Empty));

builder.Services.AddSingleton<IStratzHeroParser, StratzHeroParser>();
builder.Services.AddSingleton<IDatabaseStorage, DatabaseStorage>();
builder.Services.AddSingleton<IHeroStatsFilterService, HeroStatsFilterService>();
builder.Services.AddSingleton<IHeroStatsAggregator, HeroStatsAggregator>();
builder.Services.AddSingleton<IHeroInfoFormatter, HeroInfoFormatter>();

// Регистрация основных процессоров
builder.Services.AddSingleton<IHeroStatsProcessor, HeroStatsProcessor>();
builder.Services.AddSingleton<IHeroesDataService, HeroesDataService>();
builder.Services.AddHostedService<HeroDataUpdateHostedService>();

// Регистрация самого бота
builder.Services.AddSingleton<Dota2MetaBot>();

using var host = builder.Build();

// 3. Запуск логики
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