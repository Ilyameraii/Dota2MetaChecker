using Services.Contracts.Avatars;
using Services.Data_sync;

namespace Services.ImageProviders;

public class HeroAvatarProvider(HttpClient http, HeroesDataCache cache) : IHeroAvatarProvider
{
    private readonly Dictionary<int, byte[]> avatarCache = [];

    public async Task<Dictionary<int, byte[]>> GetAvatarsAsync(
        IEnumerable<int> heroIds,
        CancellationToken ct)
    {
        var result = new Dictionary<int, byte[]>();
        var toLoad = heroIds.Where(id => !avatarCache.ContainsKey(id)).ToList();

        await Parallel.ForEachAsync(toLoad, ct, async (id, token) =>
        {
            var name = cache.HeroesNames?.GetValueOrDefault(id) ?? string.Empty;
            var urlName = name.ToLowerInvariant().Replace(' ', '_').Replace("-", "");
            var url = $"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/{urlName}.png";

            try
            {
                var bytes = await http.GetByteArrayAsync(url, token);
                lock (avatarCache) avatarCache[id] = bytes;
            }
            catch
            {
                lock (avatarCache) avatarCache[id] = [];
            }
        });

        foreach (var id in heroIds)
            if (avatarCache.TryGetValue(id, out var b))
                result[id] = b;

        return result;
    }
}
