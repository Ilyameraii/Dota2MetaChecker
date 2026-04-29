using Microsoft.Extensions.Hosting;
using Services.Contracts.Data_sync;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Data_sync;

/// <summary>
/// Фоновый сервис для автоматического обновления данных о героях Dota2 каждый час.
/// </summary>
public class HeroDataUpdateHostedService : IHostedService, IDisposable
{
    private readonly IHeroesDataService heroesDataService;
    private readonly PeriodicTimer timer;
    private readonly CancellationTokenSource cancellationTokenSource;
    private const int retryCount = 3;
    private static readonly TimeSpan retryDelay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan updateInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Инициализирует новый экземпляр сервиса обновления данных.
    /// </summary>
    /// <param name="heroesDataService">Сервис данных о героях.</param>
    public HeroDataUpdateHostedService(IHeroesDataService heroesDataService)
    {
        this.heroesDataService = heroesDataService;
        timer = new PeriodicTimer(updateInterval);
        cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Запускает выполнение фонового сервиса.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = RunUpdateLoopAsync(cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    private async Task RunUpdateLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await ExecuteUpdateWithRetryAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
    }

    internal async Task ExecuteUpdateWithRetryAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                await heroesDataService.UpdateDataAsync();
                await heroesDataService.SaveDataAsync();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed (attempt {attempt}/{retryCount}): {ex.Message}");
                if (attempt < retryCount)
                {
                    await Task.Delay(retryDelay, cancellationToken);
                }
            }
        }
        Console.WriteLine($"All {retryCount} update attempts failed. Waiting for next scheduled update.");
    }

    /// <summary>
    /// Останавливает выполнение фонового сервиса.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource.Cancel();
        timer.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Освобождает ресурсы, используемые сервисом.
    /// </summary>
    public void Dispose()
    {
        cancellationTokenSource?.Dispose();
        timer?.Dispose();
    }
}
