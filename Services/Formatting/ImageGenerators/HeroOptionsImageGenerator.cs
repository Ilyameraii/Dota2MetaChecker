using Dota2MetaChecker.Common.Enums;
using Dota2MetaChecker.Common.Models;
using Services.Contracts.Formatting;
using Services.ImageProviders;
using SkiaSharp;

namespace Services.Formatting.ImageGenerators;

/// <summary>
///     Генератор изображений для топа героев мета Dota 2
/// </summary>
public class HeroOptionsImageGenerator :IImageGenerator
{
    // === Размеры холста ===
    private const int Width = 900;
    private const int RowHeight = 88;
    private const int HeaderHeight = 90;    // две строки заголовка + отступ до линии
    private const int FooterHeight = 50;    // строка РАНГИ/РОЛИ снизу
    private const int Padding = 40;
    private const int AvatarSize = 72;
    private const int AvatarWidth = 128;

    // === Размеры иконок фильтров ===
    private const int FilterIconSize = 35;
    private const int FilterIconSpacing = 5;

    // === Цвета ===
    private static readonly SKColor BackgroundTop    = new(18, 12, 10);
    private static readonly SKColor BackgroundBottom = new(30, 14, 8);
    private static readonly SKColor AccentRed        = new(200, 40, 30);
    private static readonly SKColor WinRateColor     = new(106, 213, 106);
    private static readonly SKColor PickRateColor    = new(220, 220, 220);
    private static readonly SKColor HeroNameColor    = new(255, 255, 255);
    private static readonly SKColor RankColor        = new(180, 160, 140);
    private static readonly SKColor DividerColor     = new(60, 40, 30);
    private static readonly SKColor CardBackground   = new(28, 18, 14);
    private static readonly SKColor SubtitleColor    = new(180, 160, 140);

    /// <summary>
    ///     Генерирует PNG-изображение с топом героев.
    /// </summary>
    /// <param name="heroes">Список героев (уже отсортированный, топ N)</param>
    /// <param name="heroAvatars">Словарь аватаров: heroId -> PNG bytes</param>
    /// <param name="title">Заголовок (например "ТОП-5")</param>
    /// <param name="rankOffset">Смещение нумерации для пагинации</param>
    /// <param name="options">Параметры фильтрации и сортировки для отображения</param>
    /// <returns>PNG-байты изображения</returns>
    public byte[] Generate(
        IReadOnlyList<Hero> heroes,
        IReadOnlyDictionary<int, byte[]>? heroAvatars = null,
        string title = "ТОП-5",
        int rankOffset = 0,
        HeroProcessingOptions? options = null)
    {
        var height = HeaderHeight + heroes.Count * RowHeight + FooterHeight;

        var imageInfo = new SKImageInfo(Width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        DrawBackground(canvas, Width, height);
        DrawHeader(canvas, title, options);

        for (var i = 0; i < heroes.Count; i++)
            DrawHeroRow(canvas, heroes[i], i, heroAvatars, rankOffset);

        DrawFooter(canvas, height, options);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 95);
        return data.ToArray();
    }

    // ─── Фон ────────────────────────────────────────────────────────────────

    private static void DrawBackground(SKCanvas canvas, int width, int height)
    {
        using var bgPaint = new SKPaint();
        bgPaint.Shader = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, height),
            [BackgroundTop, BackgroundBottom],
            SKShaderTileMode.Clamp);
        canvas.DrawRect(0, 0, width, height, bgPaint);

        using var vignettePaint = new SKPaint();
        vignettePaint.Shader = SKShader.CreateRadialGradient(
            new SKPoint(width / 2f, height / 2f),
            MathF.Max(width, height) * 0.75f,
            [new SKColor(0, 0, 0, 0), new SKColor(0, 0, 0, 120)],
            SKShaderTileMode.Clamp);
        canvas.DrawRect(0, 0, width, height, vignettePaint);
    }

    // ─── Заголовок ───────────────────────────────────────────────────────────

    private static void DrawHeader(SKCanvas canvas, string title, HeroProcessingOptions? options)
    {
        var boldTypeface = SKTypeface.FromFamilyName(
            "Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

        // ── Строка 1: "DOTA 2 META TRACKER  |  ТОП-5 ЛУЧШИХ ГЕРОЕВ" ──
        const float line1Y = 36f;

        using var trackerPaint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 28,
            IsAntialias = true,
            Typeface = boldTypeface,
            FakeBoldText = true
        };
        canvas.DrawText("DOTA 2 META TRACKER", Padding, line1Y, trackerPaint);

        using var sepPaint = new SKPaint
        {
            Color = new SKColor(100, 100, 100),
            TextSize = 28,
            IsAntialias = true,
            Typeface = boldTypeface
        };
        var trackerWidth = trackerPaint.MeasureText("DOTA 2 META TRACKER");
        const string sepText = "  |  ";
        canvas.DrawText(sepText, Padding + trackerWidth, line1Y, sepPaint);

        var qualityLabel = options == null || options.IsDescending ? "ЛУЧШИХ ГЕРОЕВ" : "ХУДШИХ ГЕРОЕВ";
        var topLabel = $"{title.ToUpper()} {qualityLabel}";

        using var topPaint = new SKPaint
        {
            Color = AccentRed,
            TextSize = 28,
            IsAntialias = true,
            Typeface = boldTypeface,
            FakeBoldText = true
        };
        var sepWidth = sepPaint.MeasureText(sepText);
        var redStartX = Padding + trackerWidth + sepWidth;
        canvas.DrawText(topLabel, redStartX, line1Y, topPaint);

        // ── Строка 2: тип сортировки красным, под красной частью строки 1 ──
        // 18px — самая длинная метка "ПО РОСТУ РЕЙТИНГА ЗА НЕДЕЛЮ" влезает в ширину
        if (options != null)
        {
            const float line2Y = 68f;
            var sortLabel = GetSortLabel(options.SortBy).ToUpper();

            using var sortPaint = new SKPaint
            {
                Color = AccentRed,
                TextSize = 18,
                IsAntialias = true,
                Typeface = boldTypeface,
                FakeBoldText = true
            };
            canvas.DrawText(sortLabel, redStartX, line2Y, sortPaint);
        }

        // ── Нижняя линия ──
        using var linePaint = new SKPaint { Color = AccentRed, StrokeWidth = 2, IsAntialias = true };
        canvas.DrawLine(Padding, HeaderHeight - 6, Width - Padding, HeaderHeight - 6, linePaint);
    }

    // ─── Футер: РАНГИ и РОЛИ под последним героем ────────────────────────────

    private static void DrawFooter(SKCanvas canvas, int totalHeight, HeroProcessingOptions? options)
{
    var footerTop = totalHeight - FooterHeight;
    var footerIconY = footerTop + (FooterHeight - FilterIconSize) / 2f;
    var textY = footerTop + FooterHeight / 2f + 6f;

    // Разделительная линия над футером
    using var linePaint = new SKPaint { Color = DividerColor, StrokeWidth = 1, IsAntialias = true };
    canvas.DrawLine(Padding, footerTop + 4, Width - Padding, footerTop + 4, linePaint);

    var boldTypeface = SKTypeface.FromFamilyName(
        "Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

    using var labelPaint = new SKPaint
    {
        Color = SKColors.White,
        TextSize = 16,
        IsAntialias = true,
        Typeface = boldTypeface,
        FakeBoldText = true
    };
    using var valuePaint = new SKPaint
    {
        Color = SubtitleColor,
        TextSize = 16,
        IsAntialias = true,
        Typeface = boldTypeface
    };

    // ── РАНГИ (слева) ──
    float rankX = Padding-20f;
    canvas.DrawText("РАНГИ:", rankX, textY, labelPaint);
    rankX += labelPaint.MeasureText("РАНГИ:") + 10;

    var rankIcons = options != null ? GetRankIconNames(options.Ranks) : [];
    if (rankIcons.Count == 0)
    {
        canvas.DrawText("ВСЕ", rankX, textY, valuePaint);
    }
    else
    {
        foreach (var name in rankIcons)
        {
            var bytes = ImageResourceProvider.GetRankIcon(name);
            if (bytes == null) continue;
            DrawFilterIcon(canvas, bytes, rankX, footerIconY);
            rankX += FilterIconSize + FilterIconSpacing;
        }
    }

    // ── РОЛИ (правее горизонтального центра) ──
    float roleX = Width / 2f + 20f;
    canvas.DrawText("РОЛИ:", roleX, textY, labelPaint);
    roleX += labelPaint.MeasureText("РОЛИ:") + 10;

    var roleIcons = options != null ? GetRoleIconNames(options.Roles) : [];
    if (roleIcons.Count == 0)
    {
        canvas.DrawText("ВСЕ", roleX, textY, valuePaint);
    }
    else
    {
        foreach (var name in roleIcons)
        {
            var bytes = ImageResourceProvider.GetRoleIcon(name);
            if (bytes == null) continue;
            DrawFilterIcon(canvas, bytes, roleX, footerIconY);
            roleX += FilterIconSize + FilterIconSpacing;
        }
    }
}

    // ─── Иконка фильтра ──────────────────────────────────────────────────────

    private static void DrawFilterIcon(SKCanvas canvas, byte[] iconBytes, float x, float y)
    {
        using var bitmap = SKBitmap.Decode(iconBytes);
        if (bitmap == null) return;

        var destRect = new SKRect(x, y, x + FilterIconSize, y + FilterIconSize);

        canvas.Save();
        using var clipPath = new SKPath();
        clipPath.AddRoundRect(destRect, 3, 3);
        canvas.ClipPath(clipPath, antialias: true);

        using var imgPaint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High };
        canvas.DrawBitmap(bitmap, destRect, imgPaint);
        canvas.Restore();
    }

    // ─── Строка героя ────────────────────────────────────────────────────────

    private static void DrawHeroRow(
        SKCanvas canvas,
        Hero hero,
        int index,
        IReadOnlyDictionary<int, byte[]>? heroAvatars,
        int rankOffset = 0)
    {
        var rowTop    = HeaderHeight + index * RowHeight;
        var rowBottom = rowTop + RowHeight;
        var centerY   = rowTop + RowHeight / 2f;

        // Полупрозрачная карточка фона
        using var cardPaint = new SKPaint { IsAntialias = true };
        var cardAlpha = (byte)(index % 2 == 0 ? 40 : 20);
        cardPaint.Color = CardBackground.WithAlpha(cardAlpha);
        canvas.DrawRoundRect(
            new SKRect(Padding / 2f, rowTop + 4, Width - Padding / 2f, rowBottom - 4),
            8, 8, cardPaint);

        // Разделитель
        if (index > 0)
        {
            using var divPaint = new SKPaint { Color = DividerColor, StrokeWidth = 1 };
            canvas.DrawLine(Padding, rowTop, Width - Padding, rowTop, divPaint);
        }

        float x = Padding;

        // ── Номер ──
        using var numPaint = new SKPaint
        {
            Color = RankColor,
            TextSize = 30,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        canvas.DrawText($"{index + 1 + rankOffset}", x, centerY + 11, numPaint);
        x += 52;

        // ── Аватар ──
        var avatarRect = new SKRect(x, centerY - AvatarSize / 2f, x + AvatarWidth, centerY + AvatarSize / 2f);
        DrawAvatar(canvas, hero, heroAvatars, avatarRect);
        x += AvatarWidth + 20;

        // ── Имя героя ──
        using var namePaint = new SKPaint
        {
            Color = HeroNameColor,
            TextSize = 26,
            IsAntialias = true,
            FakeBoldText = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };

        const float maxNameWidth = 560f;
        
        var displayName = (hero.Name ?? "Unknown").ToUpper();
        var fontSize = 26f;
        namePaint.TextSize = fontSize;

        while (fontSize > 16f && x + namePaint.MeasureText(displayName) > maxNameWidth)
        {
            fontSize -= 1f;
            namePaint.TextSize = fontSize;
        }

        canvas.DrawText(displayName, x, centerY + 9, namePaint);

        // ── Win Rate ──
        const float statX = 580f;
        using var wrPaint = new SKPaint
        {
            Color = WinRateColor,
            TextSize = 24,
            IsAntialias = true,
            FakeBoldText = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        using var wrLabelPaint = new SKPaint
        {
            Color = new SKColor(160, 200, 160),
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        var wrText = $"{hero.WinRate * 100:F2}%";
        canvas.DrawText(wrText, statX, centerY + 4, wrPaint);
        canvas.DrawText(" WR", statX + wrPaint.MeasureText(wrText), centerY + 4, wrLabelPaint);
        DrawDelta(canvas, hero.WinRateDelta, statX, centerY + 22, 13);

        // ── Pick Rate ──
        const float prX = 740f;
        using var prPaint = new SKPaint
        {
            Color = PickRateColor,
            TextSize = 24,
            IsAntialias = true,
            FakeBoldText = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        using var prLabelPaint = new SKPaint
        {
            Color = new SKColor(180, 180, 180),
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName(
                "Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        var prText = $"{hero.PickRate * 100:F2}%";
        canvas.DrawText(prText, prX, centerY + 4, prPaint);
        canvas.DrawText(" PR", prX + prPaint.MeasureText(prText), centerY + 4, prLabelPaint);
        DrawDelta(canvas, hero.PickRateDelta, prX, centerY + 22, 13);
    }

    // ─── Аватар героя ────────────────────────────────────────────────────────

    private static void DrawAvatar(
        SKCanvas canvas,
        Hero hero,
        IReadOnlyDictionary<int, byte[]>? heroAvatars,
        SKRect rect)
    {
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(80, 60, 40),
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRoundRect(rect, 6, 6, borderPaint);

        if (heroAvatars != null
            && heroAvatars.TryGetValue(hero.Id, out var avatarBytes)
            && avatarBytes.Length > 0)
        {
            using var avatarBitmap = SKBitmap.Decode(avatarBytes);
            if (avatarBitmap != null)
            {
                canvas.Save();
                using var clipPath = new SKPath();
                clipPath.AddRoundRect(rect, 6, 6);
                canvas.ClipPath(clipPath, antialias: true);

                using var imgPaint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High };
                canvas.DrawBitmap(avatarBitmap, rect, imgPaint);
                canvas.Restore();
                return;
            }
        }

        // Fallback: placeholder с инициалом
        using var bgPaint = new SKPaint { Color = new SKColor(50, 35, 25), IsAntialias = true };
        canvas.DrawRoundRect(rect, 6, 6, bgPaint);

        var initial = (hero.Name ?? "?").Length > 0 ? hero.Name![0].ToString().ToUpper() : "?";
        using var initPaint = new SKPaint
        {
            Color = new SKColor(180, 140, 100),
            TextSize = 28,
            IsAntialias = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Center
        };
        canvas.DrawText(initial, rect.MidX, rect.MidY + 10, initPaint);
    }

    // ─── Дельта (изменение) ──────────────────────────────────────────────────

    private static void DrawDelta(SKCanvas canvas, double delta, float x, float y, float textSize)
    {
        if (Math.Abs(delta) < 0.0001) return;

        var deltaPercent = delta * 100;
        var isPositive   = deltaPercent > 0;
        var color        = isPositive ? new SKColor(80, 200, 80) : new SKColor(220, 70, 60);
        var arrow        = isPositive ? "▲" : "▼";

        using var deltaPaint = new SKPaint
        {
            Color = color,
            TextSize = textSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };
        canvas.DrawText($"{arrow} {Math.Abs(deltaPercent):F2}%", x, y, deltaPaint);
    }

    // ─── Вспомогательные методы ──────────────────────────────────────────────

    private static string GetSortLabel(SortType sortBy) => sortBy switch
    {
        SortType.WinRate          => "По винрейту",
        SortType.MatchCount       => "По количеству матчей",
        SortType.Rating           => "По рейтингу",
        SortType.WinrateDelta    => "По росту винрейта за неделю",
        SortType.PickrateDelta => "По росту матчей за неделю",
        SortType.RatingDelta     => "По росту рейтинга за неделю",
        _                         => string.Empty
    };

    /// <summary>
    ///     Возвращает список имён иконок рангов по флагам.
    ///     Имена соответствуют именам файлов в FormattingResources/Ranks/.
    ///     Пары рангов разворачиваются в обе иконки.
    /// </summary>
    private static List<string> GetRankIconNames(RankFlags ranks)
    {
        var result = new List<string>();

        if (ranks == RankFlags.None) return result;

        // Файлы: archon.png, crusader.png, guardian.png, herald.png, immortal.png, legend.png
        // Файлы divine.png, ancient.png, uncalibrated.png отсутствуют — иконки будут пропущены
        if (ranks.HasFlag(RankFlags.Uncalibrated))   result.Add("uncalibrated");
        if (ranks.HasFlag(RankFlags.HeraldGuardian)) { result.Add("herald");   result.Add("guardian"); }
        if (ranks.HasFlag(RankFlags.CrusaderArchon)) { result.Add("crusader"); result.Add("archon");   }
        if (ranks.HasFlag(RankFlags.LegendAncient))  { result.Add("legend");   result.Add("ancient");  }
        if (ranks.HasFlag(RankFlags.DivineImmortal)) { result.Add("divine");   result.Add("immortal"); }

        return result;
    }

    /// <summary>
    ///     Возвращает список имён иконок ролей по флагам.
    ///     Имена соответствуют именам файлов в FormattingResources/Roles/.
    /// </summary>
    private static List<string> GetRoleIconNames(RoleFlags roles)
    {
        var result = new List<string>();

        if (roles == RoleFlags.None) return result;

        // Файлы: safelane.png, midlane.png, offlane.png, support.png, hard_support.png
        if (roles.HasFlag(RoleFlags.Safelane))    result.Add("safelane");
        if (roles.HasFlag(RoleFlags.Midlane))     result.Add("midlane");
        if (roles.HasFlag(RoleFlags.Offlane))     result.Add("offlane");
        if (roles.HasFlag(RoleFlags.Support))     result.Add("support");
        if (roles.HasFlag(RoleFlags.HardSupport)) result.Add("hard_support");

        return result;
    }
}