using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Dota2MetaChecker.TelegramBot.Extensions;
using Services.Contracts.Formatting;
using Services.Contracts.Processing;
using Services.Data_sync;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
/// Строит сообщение со списком героев и клавиатуру для него.
/// </summary>
public class HeroesListMessageBuilder(
    IHeroInfoFormatter heroFormatter,
    IHeroStatsProcessor heroStatsProcessor,
    HeroesDataCache heroesCache) : IHeroesListMessageBuilder
{
    private const int HeroesPerPage = 5;

    /// <summary>
    /// Возвращает true, если данные для построения сообщения готовы.
    /// </summary>
    public bool IsReady => heroesCache.IsLoaded;

    /// <summary>
    /// Возвращает текст сообщения и inline-клавиатуру со списком героев для заданного пользователя.
    /// </summary>
    public (string message, InlineKeyboardMarkup keyboard) BuildMessageWithButtons(UserPreferences prefs)
    {
        var newStatsHeroes = heroStatsProcessor.GetProcessedHeroStats(
            heroesCache.NewHeroesStats!,
            heroesCache.HeroesNames!,
            prefs.ProcessingOptions);

        var oldStatsHeroes = heroStatsProcessor.GetProcessedHeroStats(
            heroesCache.OldHeroesStats!,
            heroesCache.HeroesNames!,
            prefs.ProcessingOptions);

        var newTotalMatchCount = newStatsHeroes.Sum(h => h.MatchCount);
        var oldTotalMatchCount = oldStatsHeroes.Sum(h => h.MatchCount);

        var totalPages = (newStatsHeroes.Count - 1) / HeroesPerPage;
        var pageIndex = Math.Min(prefs.PageNumber, totalPages);

        var start = pageIndex * HeroesPerPage;
        var end = Math.Min(start + HeroesPerPage, newStatsHeroes.Count);

        var lines = Enumerable.Range(start, end - start)
            .Select(i =>
                $"{i + 1}. {heroFormatter.Format(newStatsHeroes[i], newTotalMatchCount, oldStatsHeroes[i], oldTotalMatchCount)}");

        var message = string.Join("\n\n", lines);
        var keyboard = BuildKeyboard(pageIndex, totalPages, prefs);

        return (message, keyboard);
    }

    private InlineKeyboardMarkup BuildKeyboard(int pageIndex, int totalPages, UserPreferences prefs)
    {
        var options = prefs.ProcessingOptions;

        var navButtons = new[]
        {
            new InlineKeyboardButton("◀ Назад", "page:prev"),
            new InlineKeyboardButton($"{pageIndex + 1}/{totalPages + 1}", "noop"),
            new InlineKeyboardButton("Вперёд ▶", "page:next")
        };

        var sortButtons = new[]
        {
            new InlineKeyboardButton(
                SortType.MatchCount.ToDisplayName(options.SortBy, options.IsDescending),
                CallbackPrefixes.Sort + SortType.MatchCount),
            new InlineKeyboardButton(
                SortType.WinRate.ToDisplayName(options.SortBy, options.IsDescending),
                CallbackPrefixes.Sort + SortType.WinRate),
            new InlineKeyboardButton(
                SortType.Rating.ToDisplayName(options.SortBy, options.IsDescending),
                CallbackPrefixes.Sort + SortType.Rating)
        }.WithStyle(KeyboardButtonStyle.Danger);

        var pairedRows = new List<IEnumerable<InlineKeyboardButton>>
        {
            BuildRankRoleRow(RankFlags.HeraldGuardian, Rank.HeraldGuardian, RoleFlags.Safelane, Role.Safelane, options),
            BuildRankRoleRow(RankFlags.CrusaderArchon, Rank.CrusaderArchon, RoleFlags.Midlane, Role.Midlane, options),
            BuildRankRoleRow(RankFlags.LegendAncient, Rank.LegendAncient, RoleFlags.Offlane, Role.Offlane, options),
            BuildRankRoleRow(RankFlags.DivineImmortal, Rank.DivineImmortal, RoleFlags.Support, Role.Support, options),
            BuildRankRoleRow(RankFlags.Uncalibrated, Rank.Uncalibrated, RoleFlags.HardSupport, Role.HardSupport, options)
        };

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
        rows.Add(sortButtons);

        return new InlineKeyboardMarkup(rows);
    }

    private InlineKeyboardButton[] BuildRankRoleRow(
        RankFlags rankFlag, Rank rank,
        RoleFlags roleFlag, Role role,
        HeroProcessingOptions options)
    {
        var rankButton = new InlineKeyboardButton(rankFlag.ToDisplayName(options.Ranks), CallbackPrefixes.Rank + rank);
        var roleButton = new InlineKeyboardButton(roleFlag.ToDisplayName(options.Roles), CallbackPrefixes.Role + role);
        rankButton.Style = KeyboardButtonStyle.Primary;
        roleButton.Style = KeyboardButtonStyle.Success;
        return [rankButton, roleButton];
    }

    

    
}