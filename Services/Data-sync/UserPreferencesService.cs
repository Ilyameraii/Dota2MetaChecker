using System.Collections.Concurrent;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync;

public class UserPreferencesService(IEnumerable<ICallbackHandler> handlers) : IUserPreferencesService
{
    private readonly ConcurrentDictionary<long, UserPreferences> preferences = new();

    /// <summary>
    ///     Возвращает настройки пользователя, создавая их при первом обращении.
    /// </summary>
    public UserPreferences GetOrCreate(long userId)
    {
        return preferences.GetOrAdd(userId, _ => new  UserPreferences());
    }

    /// <summary>
    ///     Применяет изменение на основе данных callback-а.
    /// </summary>
    public void Apply(long userId, string callbackData)
    {
        var handler = handlers.FirstOrDefault(h => h.CanHandle(callbackData));
        handler?.Handle(GetOrCreate(userId), callbackData);
    }
}