namespace Services.Contracts.Data_sync;

/// <summary>
///     Сервис для управления данными персонажей
/// </summary>
public interface IHeroesDataService
{
    /// <summary>
    ///     Обновляет данные о персонажах из STRATZ API
    /// </summary>
    public Task UpdateDataAsync();

    /// <summary>
    ///     Сохраняет текущие данные в базу данных
    /// </summary>
    public Task SaveDataAsync();

    /// <summary>
    ///     Загружает последние сохранённые данные из базы данных
    /// </summary>
    public Task LoadLastDataAsync();
}