using Context;
using Entities.Enums;
using Microsoft.Extensions.Configuration;
using Repository;
using Services.Contracts.Models;
using Services.Data_sync;
using Services.Deserialization;
using Services.Extensions;
using Services.Formatting;
using Services.Processing;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var token = config["StratzApi:Token"];

var cache = new HeroesDataCache();

var service = new HeroesDataService(
    new StratzApiService(token),
    new StratzHeroParser(),
    new DatabaseStorage(new DatabaseContext()),
    cache
);

var processor = new HeroStatsProcessor(
    new HeroStatsFilterService(),
    new HeroStatsAggregator());

var formatter = new HeroInfoFormatter();

try
{
    await service.UpdateDataAsync();
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}

if (cache.IsLoaded)
{
    var heroes = processor.GetProcessedHeroStats(
        cache.HeroesStats!,
        cache.HeroesNames!,
        new HeroProcessingOptions
        {
            Ranks = RankFlags.DivineImmortal | RankFlags.LegendAncient,
            Roles = RoleFlags.Safelane,
            SortBy = h=>h.OrderByWinRate(true),
        });

    var totalMatchCount = heroes.Sum(h => h.MatchCount);
    foreach (var hero in heroes)
    {
        Console.WriteLine(formatter.Format(hero,totalMatchCount));
    }
}