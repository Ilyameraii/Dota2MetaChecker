using Dota2MetaChecker.Common.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot.Contracts;

/// <summary>
///     Строит inline-клавиатуру для списка героев.
/// </summary>
public interface IHeroesKeyboardBuilder
{
    InlineKeyboardMarkup Build(int pageIndex, int totalPages, UserPreferences prefs);
}
