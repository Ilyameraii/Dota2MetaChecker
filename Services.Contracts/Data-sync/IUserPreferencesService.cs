using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Data_sync;

public interface IUserPreferencesService
{
    /// <summary>
    /// Возвращает настройки пользователя.
    /// </summary>
    UserPreferences GetOrCreate(long userId);

    /// <summary>
    /// Применяет изменение на основе данных callback-а.
    /// </summary>
    void Apply(long userId, string callbackData);
}