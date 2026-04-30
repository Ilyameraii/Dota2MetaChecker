using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot;

public static class Helper
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