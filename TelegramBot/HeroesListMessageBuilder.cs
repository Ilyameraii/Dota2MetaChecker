using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using static Dota2MetaChecker.TelegramBot.Constants.PaginationConstants;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Строит сообщение со списком героев и клавиатуру для него.
/// </summary>
public class HeroesListMessageBuilder(
    IHeroInfoFormatter heroFormatter,
    IHeroStatsProcessor heroStatsProcessor,
    HeroesDataCache heroesCache) : IHeroesListMessageBuilder
{
 
    public bool IsReady => heroesCache.IsLoaded;
 
    public string BuildMessage(UserPreferences prefs)
    {
        var heroes = heroStatsProcessor.GetProcessedHeroStats(
            heroesCache.NewHeroesStats!,
            heroesCache.OldHeroesStats!,
            heroesCache.HeroesNames!,
            prefs.ProcessingOptions);
 
        var totalPages = (heroes.Count - 1) / HeroesPerPage;
        var pageIndex = Math.Min(prefs.PageNumber, totalPages);
 
        var start = pageIndex * HeroesPerPage;
        var end = Math.Min(start + HeroesPerPage, heroes.Count);
 
        var lines = Enumerable.Range(start, end - start)
            .Select(i => $"{i + 1}. {heroFormatter.FormatWithDelta(heroes[i])}");
 
        var message = string.Join("\n\n", lines);
        
        return message;
    }
}