using Services.Contracts.Avatars;
using Services.Data_sync;

namespace Services.Formatting.ImageGenerators.ImageProviders;

public class FallbackHeroAvatarProvider(
    IHeroAvatarProvider inner,
    HeroesDataCache cache
) : IHeroAvatarProvider
{

    public async Task<Dictionary<int, byte[]>> GetAvatarsAsync(
        IEnumerable<int> heroIds,
        CancellationToken ct)
    {
        var result = await inner.GetAvatarsAsync(heroIds, ct);

        foreach (var key in result.Keys.ToList())
        {
            if (result[key].Length != 0)
                continue;

            // Строим urlName так же, как в HeroAvatarProvider
            var name = cache.HeroesNames?.GetValueOrDefault(key) ?? string.Empty;
            var urlName = name.ToLowerInvariant().Replace(' ', '_').Replace("-", "");

            result[key] = ImageResourceProvider.GetHeroIcon(urlName) ?? [];
        }

        return result;
    }
}