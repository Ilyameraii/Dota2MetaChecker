using Dota2MetaChecker.Common.Models;

namespace Services.Contracts.Formatting;

public interface IImageGenerator
{
    public byte[] Generate(
        IReadOnlyList<Hero> heroes,
        IReadOnlyDictionary<int, byte[]>? heroAvatars = null,
        string title = "ТОП-5",
        int rankOffset = 0,
        HeroProcessingOptions? options = null);
}