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
var heroInfoFormatter = new HeroInfoFormatter();
if (heroStatisticsService.TimeOfLastUpdate < DateTime.UtcNow - TimeSpan.FromHours(1))
    await heroStatisticsService.UpdateDataAsync();

if (heroStatisticsService.HeroStats != null)
{
    var result = heroStatisticsService.HeroStats
        .GroupBy(h => h.HeroId)
        .Select(h => new Hero
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
        Console.WriteLine(heroInfoFormatter.Format(hero,totalMatches));
    }
}