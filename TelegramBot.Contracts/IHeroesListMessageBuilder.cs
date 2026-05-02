using Dota2MetaChecker.Common.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot.Contracts;

/// <summary>
///     Строит сообщение со списком героев и клавиатуру для него.
/// </summary>
public interface IHeroesListMessageBuilder
{
    /// <summary>
    ///     Возвращает true, если данные для построения сообщения готовы.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    ///     Возвращает текст сообщения и inline-клавиатуру для заданного пользователя.
    /// </summary>
    (string message, InlineKeyboardMarkup keyboard) BuildMessageWithButtons(UserPreferences prefs);
}