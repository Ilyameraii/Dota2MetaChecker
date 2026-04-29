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
                await heroesDataService.UpdateNewStatsAsync();
                await heroesDataService.SaveNewStatsAsync();
                
                // Сначала читаем старое обновление, потом удаляем из бд как ненужное
                await heroesDataService.UpdateOldStatsAsync();
                await heroesDataService.RemoveNeedlessStatsAsync();
                
                Console.WriteLine("Данные обновлены: {0}", DateTime.Now);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка обновления данных: {0}", ex.Message);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}