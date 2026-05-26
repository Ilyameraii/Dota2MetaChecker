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
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(TimeSpan.FromMinutes(5));

                await heroesDataService.UpdateNewStatsAsync(cts.Token);
                await heroesDataService.SaveNewStatsAsync();
                await heroesDataService.UpdateOldStatsAsync();
                await heroesDataService.RemoveNeedlessStatsAsync();

                Console.WriteLine("Данные обновлены: {0}", DateTime.Now);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка обновления данных, использую предыдущие: {0}", ex.Message);
            
                // При таймауте/ошибке — дублируем последние данные из БД
                await heroesDataService.DuplicateLastStatsAsync();
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}