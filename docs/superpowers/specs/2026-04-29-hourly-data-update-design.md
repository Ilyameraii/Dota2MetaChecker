# Hourly Data Update — Design

## Overview

Add automatic hourly update of Dota2 hero stats from STRATZ API with persistence to PostgreSQL. The update runs as a background hosted service, independent of user interaction.

## Architecture

**HeroDataUpdateHostedService** (new class in `Services/Data-sync/`)

- Implements `IHostedService` and `IDisposable`
- Depends on `IHeroesDataService` (injected via constructor)
- Uses `PeriodicTimer` for hourly ticks — no external libraries
- Registered in `Program.cs` via `AddHostedService<HeroDataUpdateHostedService>()`

**HeroesDataService** — unchanged, remains a pure data service exposing `UpdateDataAsync()`, `SaveDataAsync()`, `LoadLastDataAsync()`.

## Data Flow

1. `PeriodicTimer` fires every hour
2. `ExecuteUpdateAsync` calls `heroesDataService.UpdateDataAsync()` (fetches from API into `HeroesDataCache`)
3. Then calls `heroesDataService.SaveDataAsync()` (persists cached data to DB via `DatabaseStorage`)
4. On success, waits for next timer tick

## Error Handling & Retry

- On failure (API error or DB error): retry up to 3 times
- 5-minute delay between retries
- After 3 failed attempts: log the error, wait for next hourly tick
- `CancellationToken` from `StopAsync` cancels all in-progress operations

## Program.cs Changes

- Keep existing `builder.Services.AddSingleton<IHeroesDataService, HeroesDataService>()`
- Add `builder.Services.AddHostedService<HeroDataUpdateHostedService>()`
- No manual update commands — schedule only

## Files to Create

- `Services/Data-sync/HeroDataUpdateHostedService.cs`

## Files to Modify

- `Dota2MetaChecker.TelegramBot/Program.cs` — register hosted service
