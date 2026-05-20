using System.Reflection;
using FormattingResources;

namespace Services.Formatting.ImageGenerators.ImageProviders;

/// <summary>
///     Провайдер иконок рангов и ролей, встроенных в сборку FormattingResources как EmbeddedResource.
/// </summary>
public static class ImageResourceProvider
{
    // Явно указываем сборку FormattingResources через маркерный тип из того проекта.
    private static readonly Assembly ResourceAssembly = typeof(ResourceAssemblyMarker).Assembly;

    private static readonly Dictionary<string, byte[]> Cache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Возвращает байты PNG-иконки ранга или null, если ресурс не найден.
    ///     Имя файла передаётся без расширения, в нижнем регистре, например "herald".
    /// </summary>
    public static byte[]? GetRankIcon(string fileName)
        => GetResource($"FormattingResources.Ranks.{fileName}.png");

    /// <summary>
    ///     Возвращает байты PNG-иконки роли или null, если ресурс не найден.
    ///     Имя файла передаётся без расширения, в нижнем регистре, например "hard_support".
    /// </summary>
    public static byte[]? GetRoleIcon(string fileName)
        => GetResource($"FormattingResources.Roles.{fileName}.png");

    /// <summary>
    ///     Возвращает байты PNG-иконки героя или null, если ресурс не найден.
    ///     Имя файла — urlName героя, например "antimage", "shadow_fiend".
    /// </summary>
    public static byte[]? GetHeroIcon(string fileName)
        => GetResource($"FormattingResources.Heroes.{fileName}.png");
    
    /// <summary>
    ///     Возвращает список всех встроенных имён ресурсов из FormattingResources.dll.
    /// </summary>
    public static IEnumerable<string> ListAll()
        => ResourceAssembly.GetManifestResourceNames();

    private static byte[]? GetResource(string resourceName)
    {
        if (Cache.TryGetValue(resourceName, out var cached))
            return cached;

        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return null;

        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);

        Cache[resourceName] = bytes;
        return bytes;
    }
}