using Context;
using Entities.Classes;
using Entities.Enums;
using Services.Stratz;
using Microsoft.Extensions.Configuration;
using Repository;
using Services;
using Services.Extensions;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var stratzToken = config["StratzApi:Token"];

var heroesDataHolder = new HeroesDataHolder(
    new StratzApiService(stratzToken),
    new StratzHeroParser(),
    new DatabaseStorage(new DatabaseContext())
);

var heroInfoFormatter = new HeroInfoFormatter();

var heroStasProcessor = new HeroStatsProcessor(
    new HeroStatsFilterService(),
    new HeroStatsAggregator()
);

await heroesDataHolder.UpdateDataAsync();
await heroesDataHolder.SaveDataAsync();

var selectedRanks = RankFlags.DivineImmortal | RankFlags.LegendAncient;
var selectedRoles = RoleFlags.Safelane | RoleFlags.Midlane;

var heroes = heroStasProcessor.GetProcessedHeroStats(
    heroesDataHolder.HeroesStats,
    heroesDataHolder.HeroesNames,
    selectedRanks,
    selectedRoles,
    h => h.OrderByRating()
);

var totalMatches = heroes.Sum(h => h.MatchCount);
foreach (var hero in heroes)
{
    Console.WriteLine(heroInfoFormatter.Format(hero, totalMatches));
}