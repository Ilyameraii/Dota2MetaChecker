using Dota2MetaChecker.Common.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Dota2MetaChecker.TelegramBot.Contracts;

public interface IHeroesImageBuilder
{
    bool IsReady { get; }
    
    Task<byte[]> BuildAsync(
        UserPreferences prefs,
        CancellationToken ct = default);
}