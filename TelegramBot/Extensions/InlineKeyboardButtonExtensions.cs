using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot.Extensions;

public static class InlineKeyboardButtonExtensions
{
    public static IEnumerable<InlineKeyboardButton> WithStyle(
        this IEnumerable<InlineKeyboardButton> buttons,
        KeyboardButtonStyle style)
    {
        foreach (var button in buttons)
        {
            button.Style = style;
            yield return button;
        }
    }
}