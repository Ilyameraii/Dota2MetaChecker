using System.Reflection;
using FormattingResources;

namespace Services.ImageProviders;

/// <summary>
///     Провайдер иконок рангов и ролей, встроенных в сборку FormattingResources как EmbeddedResource.
/// </summary>
public static class ImageResourceProvider
{
    // Явно указываем сборку FormattingResources через маркерный тип из того проекта.
    // typeof(ImageResourceProvider).Assembly здесь дало бы Services.dll — не то!
    private static readonly Assembly resourceAssembly = typeof(ResourceAssemblyMarker).Assembly;
 
    private static readonly Dictionary<string, byte[]> cache = new(StringComparer.OrdinalIgnoreCase);
 
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
    ///     Возвращает список всех встроенных имён ресурсов из FormattingResources.dll.
    ///     Используй при отладке чтобы убедиться что имена совпадают с запрашиваемыми.
    /// </summary>
    public static IEnumerable<string> ListAll()
        => resourceAssembly.GetManifestResourceNames();
 
    private static byte[]? GetResource(string resourceName)
    {
        if (cache.TryGetValue(resourceName, out var cached))
            return cached;
 
        using var stream = resourceAssembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return null;
 
        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);
 
        cache[resourceName] = bytes;
        return bytes;
    }
}
