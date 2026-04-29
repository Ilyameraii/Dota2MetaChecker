using System.Text;
using Entities.Enums;
using Services.Contracts.Data_sync;
using Services.Contracts.Enums;
using Services.Contracts.Formatting;
using Services.Contracts.Models;
using Services.Contracts.Processing;
using Services.Data_sync;
using Services.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot;

/// <summary>
///     Telegram бот для отображения статистики персонажей
/// </summary>
public class Dota2MetaBot(
    ITelegramBotClient botClient,
    IHeroInfoFormatter heroFormatter,
    IHeroStatsProcessor heroStatsProcessor,
    HeroesDataCache heroesCache)
{
    private const int HeroesPerPage = 5;

    /// <summary>
    ///     Запускает приём обновлений от Telegram
    /// </summary>
    public async Task StartReceivingAsync(CancellationToken cancellationToken)
    {
        var me = await botClient.GetMe();
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

    private Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
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
        if (!heroesCache.IsLoaded)
        {
            await botClient.SendMessage(chatId, "Данные ещё загружаются, попробуйте через минуту.");
            return;
        }

        EnsureUserPreferences(userId);

        var messageText = BuildHeroesListMessage(userId, out var keyboard);
        await botClient.SendMessage(chatId, messageText, replyMarkup: keyboard);
    }

    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        var data = callbackQuery.Data;
        if (data == null) return;

        var userId = callbackQuery.From.Id;

        if (!EnsureUserPreferences(userId))
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "Начните с /start");
            return;
        }

        var prefs = heroesCache.UserPreferences[userId];

        if (data.StartsWith("page:"))
        {
            var direction = data.Substring(5);
            prefs.PageNumber = direction == "next" ? prefs.PageNumber + 1 : Math.Max(0, prefs.PageNumber - 1);
        }
        else if (data.StartsWith("rank:"))
        {
            var rankStr = data.Substring(5);
            var rank = Enum.Parse<Rank>(rankStr);
            var flag = GetRankFlag(rank);
            prefs.ProcessingOptions.Ranks ^= flag;
        }
        else if (data.StartsWith("role:"))
        {
            var roleStr = data.Substring(5);
            var role = Enum.Parse<Role>(roleStr);
            var flag = GetRoleFlag(role);
            prefs.ProcessingOptions.Roles ^= flag;
        }
        else if (data.StartsWith("sort:"))
        {
            var sortStr = data.Substring(5);
            var newSortType = Enum.Parse<SortType>(sortStr);

            if (prefs.ProcessingOptions.SortBy == newSortType)
            {
                prefs.ProcessingOptions.IsDescending = !prefs.ProcessingOptions.IsDescending;
            }
            else
            {
                prefs.ProcessingOptions.SortBy = newSortType;
                prefs.ProcessingOptions.IsDescending = true;
            }

            prefs.PageNumber = 0;
        }

        var messageText = BuildHeroesListMessage(userId, out var keyboard);
        try
        {
            await botClient.EditMessageText(
                callbackQuery.Message!.Chat.Id,
                callbackQuery.Message.MessageId,
                messageText,
                replyMarkup: keyboard);
        }
        catch
        {
            await botClient.SendMessage(
                callbackQuery.Message!.Chat.Id,
                messageText,
                replyMarkup: keyboard);
        }

        await botClient.AnswerCallbackQuery(callbackQuery.Id);
    }

    private static RankFlags GetRankFlag(Rank rank)
    {
        return rank switch
        {
            Rank.Uncalibrated => RankFlags.Uncalibrated,
            Rank.HeraldGuardian => RankFlags.HeraldGuardian,
            Rank.CrusaderArchon => RankFlags.CrusaderArchon,
            Rank.LegendAncient => RankFlags.LegendAncient,
            Rank.DivineImmortal => RankFlags.DivineImmortal,
            _ => RankFlags.None
        };
    }

    private static RoleFlags GetRoleFlag(Role role)
    {
        return role switch
        {
            Role.Safelane => RoleFlags.Safelane,
            Role.Midlane => RoleFlags.Midlane,
            Role.Offlane => RoleFlags.Offlane,
            Role.Support => RoleFlags.Support,
            Role.HardSupport => RoleFlags.HardSupport,
            _ => RoleFlags.None
        };
    }

    private bool EnsureUserPreferences(long userId)
    {
        if (!heroesCache.UserPreferences.ContainsKey(userId))
            heroesCache.UserPreferences[userId] = new UserPreferences();

        return true;
    }

    private string BuildHeroesListMessage(long userId, out InlineKeyboardMarkup keyboard)
    {
        var prefs = heroesCache.UserPreferences[userId];
        var options = new HeroProcessingOptions
        {
            Ranks = prefs.ProcessingOptions.Ranks,
            Roles = prefs.ProcessingOptions.Roles,
            SortBy = prefs.ProcessingOptions.SortBy,
            IsDescending = prefs.ProcessingOptions.IsDescending
        };

        var heroes = heroStatsProcessor.GetProcessedHeroStats(
            heroesCache.HeroesStats!,
            heroesCache.HeroesNames!,
            options);

        var totalMatchCount = heroes.Sum(h => h.MatchCount);
        var totalPages = (heroes.Count - 1) / HeroesPerPage;
        var pageIndex = Math.Min(prefs.PageNumber, totalPages);

        var sb = new StringBuilder();
        var start = pageIndex * HeroesPerPage;
        var end = Math.Min(start + HeroesPerPage, heroes.Count);

        for (var i = start; i < end; i++)
        {
            sb.AppendLine(i + 1 + ". " + heroFormatter.Format(heroes[i], totalMatchCount));
            if (i < end - 1)
                sb.AppendLine();
        }

        keyboard = BuildKeyboard(pageIndex, totalPages, prefs);
        return sb.ToString();
    }

    private InlineKeyboardMarkup BuildKeyboard(int pageIndex, int totalPages, UserPreferences prefs)
    {
        var options = prefs.ProcessingOptions;

        var navButtons = new[]
        {
            InlineKeyboardButton.WithCallbackData("◀ Назад", "page:prev"),
            InlineKeyboardButton.WithCallbackData(string.Format("{0}/{1}", pageIndex + 1, totalPages + 1), "noop"),
            InlineKeyboardButton.WithCallbackData("Вперёд ▶", "page:next")
        };

        var rankButtonsRow1 = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                GetRankButtonText(RankFlags.HeraldGuardian, options.Ranks),
                "rank:" + Rank.HeraldGuardian),
            InlineKeyboardButton.WithCallbackData(
                GetRankButtonText(RankFlags.CrusaderArchon, options.Ranks),
                "rank:" + Rank.CrusaderArchon)
        };

        var rankButtonsRow2 = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                GetRankButtonText(RankFlags.LegendAncient, options.Ranks),
                "rank:" + Rank.LegendAncient),
            InlineKeyboardButton.WithCallbackData(
                GetRankButtonText(RankFlags.DivineImmortal, options.Ranks),
                "rank:" + Rank.DivineImmortal)
        };

        var roleButtonsRow1 = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                GetRoleButtonText(RoleFlags.Safelane, options.Roles),
                "role:" + Role.Safelane),
            InlineKeyboardButton.WithCallbackData(
                GetRoleButtonText(RoleFlags.Midlane, options.Roles),
                "role:" + Role.Midlane)
        };

        var roleButtonsRow2 = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                GetRoleButtonText(RoleFlags.Offlane, options.Roles),
                "role:" + Role.Offlane),
            InlineKeyboardButton.WithCallbackData(
                GetRoleButtonText(RoleFlags.Support, options.Roles),
                "role:" + Role.Support),
            InlineKeyboardButton.WithCallbackData(
                GetRoleButtonText(RoleFlags.HardSupport, options.Roles),
                "role:" + Role.HardSupport)
        };

        var sortArrows = new[] { "↓", "↑" };
        var activeArrow = options.IsDescending ? sortArrows[0] : sortArrows[1];
        var check = "✅";

        var sortButtons = new[]
        {
            InlineKeyboardButton.WithCallbackData(
                GetSortButtonText(SortType.MatchCount, options.SortBy, options.IsDescending, activeArrow, check),
                "sort:" + SortType.MatchCount),
            InlineKeyboardButton.WithCallbackData(
                GetSortButtonText(SortType.WinRate, options.SortBy, options.IsDescending, activeArrow, check),
                "sort:" + SortType.WinRate),
            InlineKeyboardButton.WithCallbackData(
                GetSortButtonText(SortType.Rating, options.SortBy, options.IsDescending, activeArrow, check),
                "sort:" + SortType.Rating)
        };


        var rows = new List<IEnumerable<InlineKeyboardButton>>();

        if (pageIndex > 0 || pageIndex < totalPages)
        {
            var navRow = new List<InlineKeyboardButton>();
            if (pageIndex > 0) navRow.Add(navButtons[0]);

            navRow.Add(navButtons[1]);

            if (pageIndex < totalPages) navRow.Add(navButtons[2]);

            if (navRow.Count > 0) rows.Add(navRow);
        }

        rows.Add(rankButtonsRow1);
        rows.Add(rankButtonsRow2);
        rows.Add(roleButtonsRow1);
        rows.Add(roleButtonsRow2);
        rows.Add(sortButtons);

        return new InlineKeyboardMarkup(rows);
    }

    private static string GetSortButtonText(SortType sortType, SortType currentSort, bool isDescending,
        string activeArrow, string check)
    {
        var baseText = sortType switch
        {
            SortType.MatchCount => "Матчи",
            SortType.WinRate => "Винрейт",
            SortType.Rating => "Рейтинг",
            _ => "?"
        };

        if (currentSort == sortType) return baseText + (isDescending ? "↓" : "↑") + check;

        return baseText + " ↓";
    }

    private static string GetRankButtonText(RankFlags flag, RankFlags selectedFlags)
    {
        var baseText = flag switch
        {
            RankFlags.HeraldGuardian => "Рекрут + Страж",
            RankFlags.CrusaderArchon => "Рыцарь + Герой",
            RankFlags.LegendAncient => "Легенда + Властелин",
            RankFlags.DivineImmortal => "Божество + Титан",
            _ => "?"
        };
        return selectedFlags.HasFlag(flag) ? baseText + "✅" : baseText;
    }

    private static string GetRoleButtonText(RoleFlags flag, RoleFlags selectedFlags)
    {
        var baseText = flag switch
        {
            RoleFlags.Safelane => "Safelane",
            RoleFlags.Midlane => "Midlane",
            RoleFlags.Offlane => "Offlane",
            RoleFlags.Support => "Support",
            RoleFlags.HardSupport => "Hard Support",
            _ => "?"
        };
        return selectedFlags.HasFlag(flag) ? baseText + "✅" : baseText;
    }
}