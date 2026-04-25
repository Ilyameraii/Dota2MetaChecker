namespace Services.Contracts.Deserialization;

/// <summary>
/// Сервис для взаимодействия с STRATZ GraphQL API
/// </summary>
public interface IStratzApiService
{
    /// <summary>
    /// Получает статистику персонажей из STRATZ API
    /// </summary>
    public Task<string> GetHeroesStats();
    
    /// <summary>
    /// Получает список имён персонажей из STRATZ API
    /// </summary>
    public Task<string> GetHeroesNames();
}

