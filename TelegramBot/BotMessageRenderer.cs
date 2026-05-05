using Dota2MetaChecker.Common.Models;
using Dota2MetaChecker.TelegramBot.Contracts;
using Services.Data_sync;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static Dota2MetaChecker.TelegramBot.Constants.PaginationConstants;
    
namespace Dota2MetaChecker.TelegramBot;

public class BotMessageRenderer(
    ITelegramBotClient botClient,
    IHeroesKeyboardBuilder keyboardBuilder,
    IHeroesListMessageBuilder messageBuilder,
    IHeroesImageBuilder imageBuilder,
    HeroesDataCache heroesCache
) : IBotMessageRenderer
{
    public async Task RenderAsync(long chatId, int? oldMessageId, UserPreferences prefs)
    {
        var totalPages = (heroesCache.HeroCount - 1) / HeroesPerPage;
        var pageIndex = Math.Min(prefs.PageNumber, totalPages);
        var keyboard = keyboardBuilder.Build(pageIndex, totalPages, prefs);

        if (prefs.IsImageFormat)
            await RenderImageAsync(chatId, oldMessageId, prefs, keyboard);
        else
            await RenderTextAsync(chatId, oldMessageId, prefs, keyboard);
    }

    private async Task RenderImageAsync(long chatId, int? oldMessageId,
        UserPreferences prefs, InlineKeyboardMarkup keyboard)
    {
        var image = await imageBuilder.BuildAsync(prefs);

        // Пробуем edit — если не вышло (был текст), удаляем и шлём новое
        if (oldMessageId.HasValue)
        {
            try
            {
                using var stream = new MemoryStream(image);
                await botClient.EditMessageMedia(chatId, oldMessageId.Value,
                    new InputMediaPhoto(InputFile.FromStream(stream, "meta.png")),
                    replyMarkup: keyboard);
                return;
            }
            catch
            {
                await TryDeleteAsync(chatId, oldMessageId.Value);
            }
        }

        using var newStream = new MemoryStream(image);
        await botClient.SendPhoto(chatId,
            InputFile.FromStream(newStream, "meta.png"),
            replyMarkup: keyboard);
    }

    private async Task RenderTextAsync(long chatId, int? oldMessageId,
        UserPreferences prefs, InlineKeyboardMarkup keyboard)
    {
        var text = messageBuilder.BuildMessage(prefs);

        if (oldMessageId.HasValue)
        {
            try
            {
                await botClient.EditMessageText(chatId, oldMessageId.Value,
                    text, ParseMode.Html, replyMarkup: keyboard);
                return;
            }
            catch
            {
                await TryDeleteAsync(chatId, oldMessageId.Value);
            }
        }

        await botClient.SendMessage(chatId, text, ParseMode.Html, replyMarkup: keyboard);
    }

    private async Task TryDeleteAsync(long chatId, int messageId)
    {
        try { await botClient.DeleteMessage(chatId, messageId); }
        catch { /* сообщение уже удалено или нет прав */ }
    }
}