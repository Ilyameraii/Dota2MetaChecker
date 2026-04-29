using Microsoft.Extensions.Hosting;
using Services.Contracts.Data_sync;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Data_sync;

public class HeroDataUpdateHostedService : IHostedService, IDisposable
{
    private readonly IHeroesDataService _heroesDataService;
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private const int RetryCount = 3;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromHours(1);

    public HeroDataUpdateHostedService(IHeroesDataService heroesDataService)
    {
        _heroesDataService = heroesDataService;
        _timer = new PeriodicTimer(UpdateInterval);
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = RunUpdateLoopAsync(_cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    private async Task RunUpdateLoopAsync(CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken))
        {
            await ExecuteUpdateWithRetryAsync(cancellationToken);
        }
    }

    internal async Task ExecuteUpdateWithRetryAsync(CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= RetryCount; attempt++)
        {
            try
            {
                await _heroesDataService.UpdateDataAsync();
                await _heroesDataService.SaveDataAsync();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed (attempt {attempt}/{RetryCount}): {ex.Message}");
                if (attempt < RetryCount)
                {
                    await Task.Delay(RetryDelay, cancellationToken);
                }
            }
        }
        Console.WriteLine($"All {RetryCount} update attempts failed. Waiting for next scheduled update.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        _timer.Dispose();
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        _timer?.Dispose();
    }
}