using Dota2MetaChecker.Common.Models;

namespace Dota2MetaChecker.TelegramBot.Contracts;

public interface IBotMessageRenderer
{
    Task RenderAsync(long chatId, int? oldMessageId, UserPreferences prefs);
}