namespace Services.Contracts.Avatars;

public interface IHeroAvatarProvider
{
    Task<Dictionary<int, byte[]>> GetAvatarsAsync(IEnumerable<int> heroIds, CancellationToken ct);
}