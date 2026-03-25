using System;
using System.IO;
using System.Linq;
using Entities.Classes;
using Services.Stratz;
using Microsoft.Extensions.Configuration;
using Services;

string stratzApiToken;
try
{
    stratzApiToken = LoadToken();
    // дальнейшая логика
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
    return;
}

var heroStatisticsService = new HeroStatisticsService(
    new StratzHeroDataOrchestrator(
        new StratzApiService(stratzApiToken),
        new StratzHeroParser()
    )
);
if(heroStatisticsService.TimeOfLastUpdate<DateTime.UtcNow-TimeSpan.FromHours(1))
await heroStatisticsService.UpdateDataAsync();

if (heroStatisticsService.Heroes != null)
{
    var result = heroStatisticsService.Heroes
        .GroupBy(h => h.HeroId)
        .Select(h => new HeroSummary
        {
            HeroId = h.Key,
            Name = h.First().Name,
            WinCount = h.Sum(x => x.WinCount),
            MatchCount = h.Sum(x => x.MatchCount)
        })
        .OrderByDescending(h => h.MatchCount)
        .ToList();
    foreach (var hero in result)
    {
        Console.WriteLine(
            $"{hero.Name} - {100.0 * hero.WinCount / hero.MatchCount:F2} % winrate, ");
    }
}

static string LoadToken()
{
    var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    if (!File.Exists(configPath))
        throw new FileNotFoundException("Файл appsettings.json не найден. Добавьте файл в директорию запуска программы",
            configPath);

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    var token = config["StratzApi:Token"];

    if (string.IsNullOrWhiteSpace(token))
        throw new InvalidOperationException("Токен StratzApi не задан в appsettings.json.");

    return token;
}