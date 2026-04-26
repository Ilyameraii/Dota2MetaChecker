using Context;
using Dota2MetaChecker.TelegramBot;
using Entities.Enums;
using Microsoft.Extensions.Configuration;
using Repository;
using Services.Contracts.Models;
using Services.Data_sync;
using Services.Deserialization;
using Services.Formatting;
using Services.Processing;
using Telegram.Bot;

var config = await LoadConfigAsync();

var cache = new HeroesDataCache();

var heroesDataService = new HeroesDataService(
    new StratzApiService(config.StratzToken),
    new StratzHeroParser(),
    new DatabaseStorage(new DatabaseContext()),
    cache);

var heroStatsProcessor = new HeroStatsProcessor(
    new HeroStatsFilterService(),
    new HeroStatsAggregator());

var heroFormatter = new HeroInfoFormatter();

try
{
    await heroesDataService.UpdateDataAsync();
    await heroesDataService.SaveDataAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
}

if (cache.IsLoaded)
{
    var heroes = heroStatsProcessor.GetProcessedHeroStats(
        cache.HeroesStats!,
        cache.HeroesNames!,
        new HeroProcessingOptions
        {
            Ranks = RankFlags.DivineImmortal,
            Roles = RoleFlags.Safelane,
            SortBy = SortType.Rating,
            IsDescending = true
        });

    var totalMatchCount = heroes.Sum(h => h.MatchCount);
    foreach (var hero in heroes)
    {
        Console.WriteLine(heroFormatter.Format(hero, totalMatchCount));
    }
}

var bot = new Dota2MetaBot(
    new TelegramBotClient(config.TelegramToken),
    heroesDataService,
    heroFormatter,
    heroStatsProcessor,
    cache);

Console.WriteLine("Telegram бот запущен. Нажните Ctrl+C для выхода.");

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await bot.StartReceivingAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Бот остановлен.");
}

async Task<AppConfig> LoadConfigAsync()
{
    var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    if (!File.Exists(configPath))
    {
        Console.WriteLine("Свой STRATZ токен можно найти на сайте: https://stratz.com/api");
        Console.Write("Введите токен STRATZ API: ");
        var stratzToken = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.WriteLine("Свой Telegram токен можно найти у @BotFather");
        Console.Write("Введите токен Telegram бота: ");
        var telegramToken = Console.ReadLine()?.Trim() ?? string.Empty;

        var json = $$"""
            {
              "StratzApi": {
                "Token": "{{stratzToken}}"
              },
              "Telegram": {
                "Token": "{{telegramToken}}"
              }
            }
            """;

        await File.WriteAllTextAsync(configPath, json);
        Console.WriteLine("Конфигурация сохранена в appsettings.json");
    }

    var configBuilder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    return new AppConfig(
        configBuilder["StratzApi:Token"] ?? string.Empty,
        configBuilder["Telegram:Token"] ?? string.Empty);
}

record AppConfig(string StratzToken, string TelegramToken);