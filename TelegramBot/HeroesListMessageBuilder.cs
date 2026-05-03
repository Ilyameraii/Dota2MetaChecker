using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Dota2MetaChecker.TelegramBot.Extensions;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using Telegram.Bot.Types.ReplyMarkups;
using SortType = Dota2MetaChecker.Common.Enums.SortType;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Строит сообщение со списком героев и клавиатуру для него.
/// </summary>
public class HeroesListMessageBuilder(
    IHeroInfoFormatter heroFormatter,
    IHeroStatsProcessor heroStatsProcessor,
    HeroesDataCache heroesCache) : IHeroesListMessageBuilder
{
    private const int HeroesPerPage = 5;

    /// <summary>
    ///     Возвращает true, если данные для построения сообщения готовы.
    /// </summary>
    public bool IsReady => heroesCache.IsLoaded;

    /// <summary>
    ///     Возвращает текст сообщения и inline-клавиатуру со списком героев для заданного пользователя.
    /// </summary>
    public (string message, InlineKeyboardMarkup keyboard) BuildMessageWithButtons(UserPreferences prefs)
    {
        var heroes = heroStatsProcessor.GetProcessedHeroStats(
            heroesCache.NewHeroesStats!,
            heroesCache.OldHeroesStats!,
            heroesCache.HeroesNames!,
            prefs.ProcessingOptions);

        var totalMatchCount = heroes.Sum(h => h.MatchCount);
        var totalPages = (heroes.Count - 1) / HeroesPerPage;
        var pageIndex = Math.Min(prefs.PageNumber, totalPages);

        var start = pageIndex * HeroesPerPage;
        var end = Math.Min(start + HeroesPerPage, heroes.Count);

        var lines = Enumerable.Range(start, end - start)
            .Select(i =>
                $"{i + 1}. {heroFormatter.FormatWithDelta(heroes[i], totalMatchCount)}");

        var message = string.Join("\n\n", lines);
        var keyboard = BuildKeyboard(pageIndex, totalPages, prefs);

        return (message, keyboard);
    }

    private InlineKeyboardMarkup BuildKeyboard(int pageIndex, int totalPages, UserPreferences prefs)
    {
        var options = prefs.ProcessingOptions;

        var navButtons = new[]
        {
            new InlineKeyboardButton("◀ Назад", CallbackPrefixes.Page + PageDirection.Previous),
            new InlineKeyboardButton($"{pageIndex + 1}/{totalPages + 1}", CallbackConstants.Noop),
            new InlineKeyboardButton("Вперёд ▶",
                CallbackPrefixes.Page + PageDirection.Next)
        };

        var sortButtons = Enum.GetValues<SortType>()
            .Select(sortType => new InlineKeyboardButton(
                sortType.ToDisplayName(options.SortBy, options.IsDescending),
                CallbackPrefixes.Sort + sortType))
            .WithStyle(KeyboardButtonStyle.Danger)
            .Chunk(2); // разбиваем по 2 кнопки на ряд

        var pairedRows = RankRolePairs.Default
            .Select(p => BuildRankRoleRow(p.RankFlag, p.RoleFlag, options))
            .ToList();

        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        if (pageIndex > 0 || pageIndex < totalPages)
        {
            var navRow = new List<InlineKeyboardButton>();
            if (pageIndex > 0) navRow.Add(navButtons[0]);
            navRow.Add(navButtons[1]);
            if (pageIndex < totalPages) navRow.Add(navButtons[2]);
            rows.Add(navRow);
        }

        rows.AddRange(pairedRows);
        
        // Вношу по 2 кнопки в ряд
        foreach (var row in sortButtons)
        {
            rows.Add(row);
        }
        
        // кнопка для сброса настроек
        rows.Add([
            new InlineKeyboardButton("Сбросить",  CallbackConstants.ClearOptions),
        ]);
        
        return new InlineKeyboardMarkup(rows);
    }

    private InlineKeyboardButton[] BuildRankRoleRow(
        RankFlags rankFlag,
        RoleFlags roleFlag,
        HeroProcessingOptions options)
    {
        var rankButton = new InlineKeyboardButton(rankFlag.ToDisplayName(options.Ranks), CallbackPrefixes.Rank + rankFlag);
        var roleButton = new InlineKeyboardButton(roleFlag.ToDisplayName(options.Roles), CallbackPrefixes.Role + roleFlag);
        rankButton.Style = KeyboardButtonStyle.Primary;
        roleButton.Style = KeyboardButtonStyle.Success;
        return [rankButton, roleButton];
    }
}