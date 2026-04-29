using Microsoft.Extensions.Hosting;
using Services.Contracts.Data_sync;

namespace Services.Data_sync;

/// <summary>
///     Фоновый сервис для периодического обновления данных о героях.
/// </summary>
public class HeroesDataSyncService(IHeroesDataService heroesDataService) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    /// <summary>
    ///     Запускает цикл периодического обновления данных.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await heroesDataService.UpdateDataAsync();
                await heroesDataService.SaveDataAsync();
                Console.WriteLine("Данные обновлены: {0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка обновления данных: {0}", ex.Message);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}