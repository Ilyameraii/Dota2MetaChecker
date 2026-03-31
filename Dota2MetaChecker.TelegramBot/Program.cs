using System;
using System.IO;
using System.Linq;
using Entities.Classes;
using Services.Stratz;
using Microsoft.Extensions.Configuration;
using Services;

string? token;
try
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    token = config["StratzApi:Token2"];
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка конфигурации: {ex.Message}");
    return;
}

var heroStatisticsService = new HeroStatisticsService(
    new StratzApiService(token),
    new StratzHeroParser()
);

if (heroStatisticsService.TimeOfLastUpdate < DateTime.UtcNow - TimeSpan.FromHours(1))
    await heroStatisticsService.UpdateDataAsync();

if (heroStatisticsService.HeroStats != null)
{
    var result = heroStatisticsService.HeroStats
        .GroupBy(h => h.HeroId)
        .Select(h => new HeroSummary
        {
            HeroId = h.Key,
            Name = heroStatisticsService.HeroesNames?[h.Key],
            WinCount = h.Sum(x => x.WinCount),
            MatchCount = h.Sum(x => x.MatchCount)
        })
        .OrderBy(h => h.Rating)
        .ToList();

    var totalMatches = result.Sum(h => h.MatchCount);
    foreach (var hero in result)
    {
        Console.WriteLine(
            $"{hero.Name} - {100.0 * hero.WinCount / hero.MatchCount:F2}% winrate, {100.0 * hero.MatchCount / totalMatches:F2}% pickrate");
    }
}