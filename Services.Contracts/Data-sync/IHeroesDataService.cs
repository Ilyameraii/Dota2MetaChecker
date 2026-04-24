namespace Services.Contracts.Data_sync;

public interface IHeroesDataService
{
    public Task UpdateDataAsync();

    public  Task SaveDataAsync();

    public  Task LoadLastDataAsync();
}