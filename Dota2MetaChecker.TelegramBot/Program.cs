using Context;
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

    token = config["StratzApi:Token"];
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
await heroStatisticsService.UpdateDataAsync();
await heroStatisticsService.SaveDataAsync();
if (heroStatisticsService.HeroesStats != null)
{
    var result = heroStatisticsService.HeroesStats
        .Where(s => s is { Rank: HeroRank.DivineImmortal, Role: HeroRole.Safelane })
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
        Console.WriteLine(heroInfoFormatter.Format(hero, totalMatches));
    }
}