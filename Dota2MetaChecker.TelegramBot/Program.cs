using System;
using System.IO;
using System.Linq;
using Context.Data;
using Entities.Classes;
using Entities.Enums;
using Services.Stratz;
using Microsoft.Extensions.Configuration;
using Repository;
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
    new StratzHeroParser(),
    new DatabaseStorage(new DatabaseContext())
);
var heroInfoFormatter = new HeroInfoFormatter();
if (heroStatisticsService.UpdateTime < DateTime.UtcNow - TimeSpan.FromHours(1))
{
    await heroStatisticsService.UpdateDataAsync();
    await heroStatisticsService.SaveDataAsync();
}
if (heroStatisticsService.HeroStats != null)
{
    var result = heroStatisticsService.HeroStats
        .Where(s=>s.Rank==HeroRank.DivineImmortal)
        .GroupBy(s => s.HeroId)
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