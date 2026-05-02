using Dota2MetaChecker.TelegramBot.Contracts;
using Services.Contracts.Data_sync;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Telegram бот для отображения статистики персонажей
/// </summary>
public class Dota2MetaBot(
    ITelegramBotClient botClient,
    IUserPreferencesService preferencesService,
    IHeroesListMessageBuilder messageBuilder)
{
    /// <summary>
    ///     Запускает приём обновлений от Telegram
    /// </summary>
    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        var me = await botClient.GetMe(cancellationToken);
        Console.WriteLine("Бот @{0} запущен.", me.Username);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        try
        {
            await botClient.ReceiveAsync(
                HandleUpdate,
                HandleError,
                receiverOptions,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Бот остановлен.");
        }
    }

    private static Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("Ошибка: {0}", exception.Message);
        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is { } message)
                await HandleMessageAsync(message);
            else if (update.CallbackQuery is { } callbackQuery) await HandleCallbackQueryAsync(callbackQuery);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка обработки: {0}", ex.Message);
        }
    }

    private async Task HandleMessageAsync(Message message)
    {
        if (message.Text == "/start") await HandleStartCommandAsync(message.Chat.Id, message.From!.Id);
    }

    private async Task HandleStartCommandAsync(long chatId, long userId)
    {
        if (!messageBuilder.IsReady)
        {
            await botClient.SendMessage(chatId, "Данные ещё загружаются, попробуйте через минуту.", ParseMode.Html);
            return;
        }

        var (messageText, keyboard) = messageBuilder.BuildMessageWithButtons(preferencesService.GetOrCreate(userId));

        await botClient.SendMessage(chatId, messageText, ParseMode.Html, replyMarkup: keyboard);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        var data = callbackQuery.Data;
        if (data == null) return;

        var userId = callbackQuery.From.Id;

        preferencesService.Apply(userId, data);

        var (messageText, keyboard) = messageBuilder.BuildMessageWithButtons(preferencesService.GetOrCreate(userId));

        try
        {
            await botClient.EditMessageText(
                callbackQuery.Message!.Chat.Id,
                callbackQuery.Message.MessageId,
                messageText,
                ParseMode.Html,
                keyboard);
        }
        catch
        {
            await botClient.SendMessage(
                callbackQuery.Message!.Chat.Id,
                messageText,
                ParseMode.Html,
                replyMarkup: keyboard);
        }

        await botClient.AnswerCallbackQuery(callbackQuery.Id);
    }
}