using Context;
using Entities.Classes;
using Entities.Enums;
using Services.Stratz;
using Microsoft.Extensions.Configuration;
using Repository;
using Services;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var stratzToken = config["StratzApi:Token"];

var heroStatisticsService = new HeroStatisticsService(
    new StratzApiService(stratzToken),
    new StratzHeroParser(),
    new DatabaseStorage(new DatabaseContext())
);

var heroInfoFormatter = new HeroInfoFormatter();

await heroStatisticsService.UpdateDataAsync();
await heroStatisticsService.SaveDataAsync();

if (heroStatisticsService.HeroesStats != null)
{
    var result = heroStatisticsService.HeroesStats
        .Where(s => s is { Rank: Rank.DivineImmortal, Role: Role.Safelane })
        .GroupBy(s => s.HeroId)
        .Select(h => new Hero
        {
            Id = h.Key,
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