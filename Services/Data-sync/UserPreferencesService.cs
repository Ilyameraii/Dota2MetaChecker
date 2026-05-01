using Dota2MetaChecker.Common.Models;
using Services.Contracts.Data_sync;

namespace Services.Data_sync;

public class UserPreferencesService(IEnumerable<ICallbackHandler> handlers): IUserPreferencesService
{
    private readonly Dictionary<long, UserPreferences> preferences = new();

    /// <summary>
    /// Возвращает настройки пользователя, создавая их при первом обращении.
    /// </summary>
    public UserPreferences GetOrCreate(long userId)
    {
        if (!preferences.ContainsKey(userId))
            preferences[userId] = new UserPreferences();

        return preferences[userId];
    }

    /// <summary>
    /// Применяет изменение на основе данных callback-а.
    /// </summary>
    public void Apply(long userId, string callbackData)
    {
        var prefs = GetOrCreate(userId);
        
        var handler = handlers.FirstOrDefault(h => h.CanHandle(callbackData));
        handler?.Handle(prefs, callbackData);
    }
}

