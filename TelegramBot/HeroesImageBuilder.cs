using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Services.Contracts.Avatars;
using Services.Contracts.Processing;
using Services.Data_sync;
using Services.Formatting;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Строит PNG-изображение с топом героев и клавиатуру для него.
/// </summary>
public class HeroesImageBuilder(
    IHeroStatsProcessor heroStatsProcessor,
    HeroesDataCache heroesCache,
    HeroImageGenerator imageGenerator,
    IHeroAvatarProvider avatarProvider) : IHeroesImageBuilder
{
    private const int HeroesPerPage = 5;
 
    public bool IsReady => heroesCache.IsLoaded;
 
    public async Task<byte[]> BuildAsync(
        UserPreferences prefs,
        CancellationToken ct = default)
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
        var pageHeroes = heroes.GetRange(start, end - start);
 
        var avatars = await avatarProvider.GetAvatarsAsync(pageHeroes.Select(h => h.Id), ct);
        var pngBytes = imageGenerator.Generate(pageHeroes, avatars, $"ТОП-{end}", start);
        
        return pngBytes;
    }
}