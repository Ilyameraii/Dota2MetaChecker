using Dota2MetaChecker.Common.Models;
using SkiaSharp;

namespace Services.Formatting;

/// <summary>
///     Генератор изображений для топа героев мета Dota 2
/// </summary>
public class HeroImageGenerator
{
    // === Размеры холста ===
    private const int Width = 900;
    private const int RowHeight = 88;       // было 110 — уменьшено для нормальных пропорций
    private const int HeaderHeight = 80;
    private const int Padding = 40;
    private const int AvatarSize = 72;   // это ВЫСОТА аватара
    private const int AvatarWidth = 128; // это ШИРИНА аватара — добавить

    // === Цвета ===
    private static readonly SKColor BackgroundTop = new(18, 12, 10);
    private static readonly SKColor WinRateColor = new(106, 213, 106);
    private static readonly SKColor BackgroundBottom = new(30, 14, 8);
    private static readonly SKColor AccentRed = new(200, 40, 30);
    private static readonly SKColor PickRateColor = new(220, 220, 220);
    private static readonly SKColor HeroNameColor = new(255, 255, 255);
    private static readonly SKColor RankColor = new(180, 160, 140);
    private static readonly SKColor DividerColor = new(60, 40, 30);
    private static readonly SKColor CardBackground = new(28, 18, 14);

    /// <summary>
    ///     Генерирует PNG-изображение с топом героев.
    ///     Аватары берутся по словарю heroAvatars: heroId -> PNG bytes (или null если нет).
    /// </summary>
    /// <param name="heroes">Список героев (уже отсортированный, топ N)</param>
    /// <param name="heroAvatars">Словарь аватаров: heroId -> PNG bytes</param>
    /// <param name="title">Заголовок (например "ТОП-5")</param>
    /// <param name="rankOffset">
    ///     Смещение нумерации. Для страницы 1 (ТОП-5) = 0,
    ///     для страницы 2 (ТОП-10) = 5, для страницы 3 (ТОП-15) = 10 и т.д.
    /// </param>
    /// <returns>PNG-байты изображения</returns>
    public byte[] Generate(
        IReadOnlyList<Hero> heroes,
        IReadOnlyDictionary<int, byte[]>? heroAvatars = null,
        string title = "ТОП-5",
        int rankOffset = 0)
    {
        var height = HeaderHeight + heroes.Count * RowHeight + Padding;

        var imageInfo = new SKImageInfo(Width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(imageInfo);
        var canvas = surface.Canvas;

        DrawBackground(canvas, width: Width, height: height);
        DrawHeader(canvas, title);

        for (var i = 0; i < heroes.Count; i++)
        {
            DrawHeroRow(canvas, heroes[i], i, heroAvatars, rankOffset);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 95);
        return data.ToArray();
    }

    // ─── Фон ────────────────────────────────────────────────────────────────

    private static void DrawBackground(SKCanvas canvas, int width, int height)
    {
        // Градиентный фон сверху вниз
        using var bgPaint = new SKPaint();
        bgPaint.Shader = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, height),
            [BackgroundTop, BackgroundBottom],
            SKShaderTileMode.Clamp);
        canvas.DrawRect(0, 0, width, height, bgPaint);

        // Тонкая виньетка по краям (добавляет "кинематографичности")
        using var vignettePaint = new SKPaint();
        vignettePaint.Shader = SKShader.CreateRadialGradient(
            new SKPoint(width / 2f, height / 2f),
            MathF.Max(width, height) * 0.75f,
            [new SKColor(0, 0, 0, 0), new SKColor(0, 0, 0, 120)],
            SKShaderTileMode.Clamp);
        canvas.DrawRect(0, 0, width, height, vignettePaint);
    }

    // ─── Заголовок ───────────────────────────────────────────────────────────

    private static void DrawHeader(SKCanvas canvas, string title)
    {
        const float y = 52f;

        // "DOTA 2 META TRACKER"
        using var labelPaint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 28,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            FakeBoldText = true
        };
        canvas.DrawText("DOTA 2 META TRACKER", Padding, y, labelPaint);

        // Разделитель "|"
        using var sepPaint = new SKPaint
        {
            Color = new SKColor(100, 100, 100),
            TextSize = 28,
            IsAntialias = true,
            Typeface = labelPaint.Typeface
        };
        var labelWidth = labelPaint.MeasureText("DOTA 2 META TRACKER");
        canvas.DrawText("  |  ", Padding + labelWidth, y, sepPaint);

        // Красный заголовок (ТОП-5)
        using var titlePaint = new SKPaint
        {
            Color = AccentRed,
            TextSize = 28,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            FakeBoldText = true
        };
        var sepWidth = sepPaint.MeasureText("  |  ");
        canvas.DrawText(title.ToUpper(), Padding + labelWidth + sepWidth, y, titlePaint);

        // Нижняя линия заголовка
        using var linePaint = new SKPaint
        {
            Color = AccentRed,
            StrokeWidth = 2,
            IsAntialias = true
        };
        canvas.DrawLine(Padding, HeaderHeight - 10, Width - Padding, HeaderHeight - 10, linePaint);
    }

    // ─── Строка героя ────────────────────────────────────────────────────────

    private static void DrawHeroRow(
        SKCanvas canvas,
        Hero hero,
        int index,
        IReadOnlyDictionary<int, byte[]>? heroAvatars,
        int rankOffset = 0)
    {
        var rowTop = HeaderHeight + index * RowHeight;
        var rowBottom = rowTop + RowHeight;
        var centerY = rowTop + RowHeight / 2f;

        // Полупрозрачная карточка фона (чётные/нечётные чуть разные)
        using var cardPaint = new SKPaint { IsAntialias = true };
        var cardAlpha = (byte)(index % 2 == 0 ? 40 : 20);
        cardPaint.Color = CardBackground.WithAlpha(cardAlpha);
        var cardRect = new SKRect(Padding / 2f, rowTop + 4, Width - Padding / 2f, rowBottom - 4);
        canvas.DrawRoundRect(cardRect, 8, 8, cardPaint);

        // Разделитель
        if (index > 0)
        {
            using var divPaint = new SKPaint { Color = DividerColor, StrokeWidth = 1 };
            canvas.DrawLine(Padding, rowTop, Width - Padding, rowTop, divPaint);
        }

        float x = Padding;

        // ── Номер ──
        using var rankPaint = new SKPaint
        {
            Color = RankColor,
            TextSize = 30,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        var rankText = $"{index + 1 + rankOffset}";   // <-- учитываем смещение страницы
        canvas.DrawText(rankText, x, centerY + 11, rankPaint);
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
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        var displayName = (hero.Name ?? "Unknown").ToUpper();
        canvas.DrawText(displayName, x, centerY + 9, namePaint);

        // ── Win Rate ──
        const float statX = 580f;
        using var wrPaint = new SKPaint
        {
            Color = WinRateColor,
            TextSize = 24,
            IsAntialias = true,
            FakeBoldText = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        using var wrLabelPaint = new SKPaint
        {
            Color = new SKColor(160, 200, 160),
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };

        var wrText = $"{hero.WinRate * 100:F2}%";
        canvas.DrawText(wrText, statX, centerY + 4, wrPaint);
        var wrTextWidth = wrPaint.MeasureText(wrText);
        canvas.DrawText(" WR", statX + wrTextWidth, centerY + 4, wrLabelPaint);

        // Дельта WR
        DrawDelta(canvas, hero.WinRateDelta, statX, centerY + 22, 13);

        // ── Pick Rate ──
        const float prX = 740f;
        using var prPaint = new SKPaint
        {
            Color = PickRateColor,
            TextSize = 24,
            IsAntialias = true,
            FakeBoldText = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };
        using var prLabelPaint = new SKPaint
        {
            Color = new SKColor(180, 180, 180),
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright)
        };

        var prText = $"{hero.PickRate * 100:F2}%";
        canvas.DrawText(prText, prX, centerY + 4, prPaint);
        var prTextWidth = prPaint.MeasureText(prText);
        canvas.DrawText(" PR", prX + prTextWidth, centerY + 4, prLabelPaint);

        // Дельта PR
        DrawDelta(canvas, hero.PickRateDelta, prX, centerY + 22, 13);
    }

    // ─── Аватар героя ────────────────────────────────────────────────────────

    private static void DrawAvatar(
        SKCanvas canvas,
        Hero hero,
        IReadOnlyDictionary<int, byte[]>? heroAvatars,
        SKRect rect)
    {
        // Рамка аватара
        using var borderPaint = new SKPaint
        {
            Color = new SKColor(80, 60, 40),
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawRoundRect(rect, 6, 6, borderPaint);

        if (heroAvatars != null && heroAvatars.TryGetValue(hero.Id, out var avatarBytes) && avatarBytes.Length > 0)
        {
            using var avatarBitmap = SKBitmap.Decode(avatarBytes);
            if (avatarBitmap != null)
            {
                // Клиппинг по скруглённому прямоугольнику
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
        using var bgPaint = new SKPaint
        {
            Color = new SKColor(50, 35, 25),
            IsAntialias = true
        };
        canvas.DrawRoundRect(rect, 6, 6, bgPaint);

        var initial = (hero.Name ?? "?").Length > 0
            ? hero.Name![0].ToString().ToUpper()
            : "?";

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
        var isPositive = deltaPercent > 0;
        var arrow = isPositive ? "▲" : "▼";
        var color = isPositive ? new SKColor(80, 200, 80) : new SKColor(220, 70, 60);

        using var deltaPaint = new SKPaint
        {
            Color = color,
            TextSize = textSize,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };
        canvas.DrawText($"{arrow} {Math.Abs(deltaPercent):F2}%", x, y, deltaPaint);
    }
}