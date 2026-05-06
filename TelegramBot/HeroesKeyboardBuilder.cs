using Dota2MetaChecker.Common.Constants;
using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Dota2MetaChecker.TelegramBot.Extensions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Строит inline-клавиатуру для списка героев.
/// </summary>
public class HeroesKeyboardBuilder : IHeroesKeyboardBuilder
{
    public InlineKeyboardMarkup Build(int pageIndex, int totalPages, UserPreferences prefs)
    {
        var options = prefs.ProcessingOptions;

        var navButtons = new[]
        {
            new InlineKeyboardButton("◀ Назад", CallbackPrefixes.Page + PageDirection.Previous),
            new InlineKeyboardButton($"{pageIndex + 1}/{totalPages + 1}", CallbackConstants.Noop),
            new InlineKeyboardButton("Вперёд ▶", CallbackPrefixes.Page + PageDirection.Next)
        };

        var sortButtons = Enum.GetValues<SortType>()
            .Select(sortType => new InlineKeyboardButton(
                sortType.ToDisplayName(options.SortBy, options.IsDescending),
                CallbackPrefixes.Sort + sortType))
            .WithStyle(KeyboardButtonStyle.Primary)
            .Chunk(2);

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

        foreach (var row in sortButtons)
            rows.Add(row);

        var clearButton = new InlineKeyboardButton("Сбросить", CallbackConstants.ClearOptions);
        clearButton.Style = KeyboardButtonStyle.Danger;
        rows.Add([clearButton]);

        var switchFormatButtonText = $"{(prefs.IsImageFormat ? "✅" : "")} Режим изображения (медленный)";

        rows.Add([new InlineKeyboardButton(switchFormatButtonText, CallbackConstants.SwitchFormat)]);

        return new InlineKeyboardMarkup(rows);
    }

    private static InlineKeyboardButton[] BuildRankRoleRow(
        RankFlags rankFlag,
        RoleFlags roleFlag,
        HeroProcessingOptions options)
    {
        var rankButton =
            new InlineKeyboardButton(rankFlag.ToDisplayName(options.Ranks), CallbackPrefixes.Rank + rankFlag);
        var roleButton =
            new InlineKeyboardButton(roleFlag.ToDisplayName(options.Roles), CallbackPrefixes.Role + roleFlag);
        rankButton.Style = KeyboardButtonStyle.Danger;
        roleButton.Style = KeyboardButtonStyle.Success;
        return [rankButton, roleButton];
    }
}