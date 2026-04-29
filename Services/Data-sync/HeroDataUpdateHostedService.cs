using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Contracts.Data_sync;

namespace Services.Data_sync;

/// <summary>
/// Фоновый сервис для автоматического обновления данных о героях Dota2 каждый час.
/// </summary>
public class HeroDataUpdateHostedService : IHostedService, IDisposable
{
    private readonly IHeroesDataService heroesDataService;
    private readonly PeriodicTimer timer;
    private readonly ILogger<HeroDataUpdateHostedService> logger;
    private readonly CancellationTokenSource shutdownTokenSource;
    private Task? backgroundTask;
    private const int retryCount = 3;
    private static readonly TimeSpan retryDelay = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan updateInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Инициализирует новый экземпляр сервиса обновления данных.
    /// </summary>
    /// <param name="heroesDataService">Сервис данных о героях.</param>
    /// <param name="logger">Логгер.</param>
    public HeroDataUpdateHostedService(IHeroesDataService heroesDataService, ILogger<HeroDataUpdateHostedService> logger)
    {
        this.heroesDataService = heroesDataService;
        timer = new PeriodicTimer(updateInterval);
        shutdownTokenSource = new CancellationTokenSource();
        this.logger = logger;
    }

    /// <summary>
    /// Запускает выполнение фонового сервиса.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Связываем токен отмены от хоста с нашим токеном завершения
        cancellationToken.Register(() => shutdownTokenSource.Cancel());
        backgroundTask = RunUpdateLoopAsync(shutdownTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Выполняет цикл обновления данных.
    /// </summary>
    private async Task RunUpdateLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Сразу обновляем данные при запуске
            await ExecuteUpdateWithRetryAsync(cancellationToken);

            // Затем запускаем цикл ожидания таймера для последующих обновлений
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await ExecuteUpdateWithRetryAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неожиданная ошибка в цикле обновления");
        }
    }

    /// <summary>
    /// Выполняет обновление данных с повторными попытками.
    /// </summary>
    private async Task ExecuteUpdateWithRetryAsync(CancellationToken cancellationToken)
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
                logger.LogError(ex, "Ошибка обновления (попытка {Attempt}/{RetryCount})", attempt, retryCount);
                if (attempt < retryCount)
                {
                    await Task.Delay(retryDelay, cancellationToken);
                }
            }
        }
        logger.LogWarning("Все {RetryCount} попыток обновления не удались. Ожидание следующего обновления.", retryCount);
    }

    /// <summary>
    /// Останавливает выполнение фонового сервиса.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        shutdownTokenSource.Cancel();
        if (backgroundTask != null)
        {
            await backgroundTask;
        }
        timer.Dispose();
    }

    /// <summary>
    /// Освобождает ресурсы, используемые сервисом.
    /// </summary>
    public void Dispose()
    {
        shutdownTokenSource.Dispose();
        timer?.Dispose();
    }
}

    /// <summary>
    /// Запускает выполнение фонового сервиса.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        backgroundTask = InitializeAndRunLoopAsync(cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Инициализирует данные при запуске и запускает цикл обновления.
    /// </summary>
    private async Task InitializeAndRunLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Сразу обновляем данные при запуске
            await ExecuteUpdateWithRetryAsync(cancellationToken);

            // Затем запускаем цикл ожидания таймера для последующих обновлений
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await ExecuteUpdateWithRetryAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неожиданная ошибка в цикле обновления");
        }
    }

    private async Task ExecuteUpdateWithRetryAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= RetryCount; attempt++)
        {
            try
            {
                await heroesDataService.UpdateDataAsync();
                await heroesDataService.SaveDataAsync();
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка обновления (попытка {Attempt}/{RetryCount})", attempt, RetryCount);
                if (attempt < RetryCount)
                {
                    await Task.Delay(RetryDelay, cancellationToken);
                }
            }
        }
        logger.LogWarning("Все {RetryCount} попыток обновления не удались. Ожидание следующего обновления.", RetryCount);
    }

    /// <summary>
    /// Останавливает выполнение фонового сервиса.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await cancellationTokenSource.CancelAsync();
        if (backgroundTask != null)
        {
            await backgroundTask;
        }
        timer.Dispose();
    }

    /// <summary>
    /// Освобождает ресурсы, используемые сервисом.
    /// </summary>
    public void Dispose()
    {
        cancellationTokenSource.Dispose();
        timer.Dispose();
    }
}
