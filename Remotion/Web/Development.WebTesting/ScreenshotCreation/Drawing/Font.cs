// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

/// <summary>
/// This is a wrapper-class for <see cref="SKFont"/>. For further information see <see cref="SKFont"/>
/// </summary>
public class Font : IDisposable
{
  public static Font DefaultFont => new Font();

  private SKFont _font;

  public SKFont UnderlyingSkFont
  {
    get => _font;
    set
    {
      ArgumentNullException.ThrowIfNull(value);
      _font = value;
    }
  }

  public Font ()
  {
    _font = new SKFont();
  }

  public Font (string fontFamilyName, float size = 12f, float scaleX = 1f, float skewX = 0.0f)
    : this(SKTypeface.FromFamilyName(fontFamilyName), size, scaleX, skewX)
  {
  }

  public Font (SKTypeface typeface, float size = 12f, float scaleX = 1f, float skewX = 0.0f)
  {
    _font = new SKFont(typeface, size, scaleX, skewX);
  }

  public bool ForceAutoHinting
  {
    get => _font.ForceAutoHinting;
    set => _font.ForceAutoHinting = value;
  }

  public bool EmbeddedBitmaps
  {
    get => _font.EmbeddedBitmaps;
    set => _font.EmbeddedBitmaps = value;
  }

  public bool Subpixel
  {
    get => _font.Subpixel;
    set => _font.Subpixel = value;
  }

  public bool LinearMetrics
  {
    get => _font.LinearMetrics;
    set => _font.LinearMetrics = value;
  }

  public bool Embolden
  {
    get => _font.Embolden;
    set => _font.Embolden = value;
  }

  public bool BaselineSnap
  {
    get => _font.BaselineSnap;
    set => _font.BaselineSnap = value;
  }

  public SKFontEdging Edging
  {
    get => _font.Edging;
    set => _font.Edging = value;
  }

  public SKFontHinting Hinting
  {
    get => _font.Hinting;
    set => _font.Hinting = value;
  }

  public SKTypeface Typeface
  {
    get => _font.Typeface;
    set => _font.Typeface = value;
  }

  public float Size
  {
    get => _font.Size;
    set => _font.Size = value;
  }

  public float ScaleX
  {
    get => _font.ScaleX;
    set => _font.ScaleX = value;
  }

  public float SkewX
  {
    get => _font.SkewX;
    set => _font.SkewX = value;
  }

  public float Spacing => _font.Spacing;

  public SKFontMetrics Metrics => _font.Metrics;

  public float GetFontMetrics (out SKFontMetrics metrics)
  {
    return _font.GetFontMetrics(out metrics);
  }

  public ushort GetGlyph (int codepoint)
  {
    return _font.GetGlyph(codepoint);
  }

  public void GetGlyphs (ReadOnlySpan<int> codepoints, Span<ushort> glyphs)
  {
    _font.GetGlyphs(codepoints, glyphs);
  }

  public void GetGlyphs (string text, Span<ushort> glyphs)
  {
    _font.GetGlyphs(text, glyphs);
  }

  public void GetGlyphs (ReadOnlySpan<char> text, Span<ushort> glyphs)
  {
    _font.GetGlyphs(text, glyphs);
  }

  public void GetGlyphs (ReadOnlySpan<byte> text, SKTextEncoding encoding, Span<ushort> glyphs)
  {
    _font.GetGlyphs(text, encoding, glyphs);
  }

  public void GetGlyphs (IntPtr text, int length, SKTextEncoding encoding, Span<ushort> glyphs)
  {
    _font.GetGlyphs(text, length, encoding, glyphs);
  }

  public bool ContainsGlyph (int codepoint)
  {
    return _font.ContainsGlyph(codepoint);
  }

  public bool ContainsGlyphs (ReadOnlySpan<int> codepoints)
  {
    return _font.ContainsGlyphs(codepoints);
  }

  public bool ContainsGlyphs (string text)
  {
    return _font.ContainsGlyphs(text);
  }

  public bool ContainsGlyphs (ReadOnlySpan<char> text)
  {
    return _font.ContainsGlyphs(text);
  }

  public bool ContainsGlyphs (ReadOnlySpan<byte> text, SKTextEncoding encoding)
  {
    return _font.ContainsGlyphs(text, encoding);
  }

  public bool ContainsGlyphs (IntPtr text, int length, SKTextEncoding encoding)
  {
    return _font.ContainsGlyphs(text, length, encoding);
  }

  public void GetGlyphPositions (ReadOnlySpan<ushort> glyphs, Span<SKPoint> positions, SKPoint origin = default)
  {
    _font.GetGlyphPositions(glyphs, positions, origin);
  }

  public void GetGlyphOffsets (ReadOnlySpan<ushort> glyphs, Span<float> offsets, float origin = 0.0f)
  {
    _font.GetGlyphOffsets(glyphs, offsets, origin);
  }

  public void GetGlyphWidths (ReadOnlySpan<ushort> glyphs, Span<float> widths, Span<SKRect> bounds, SKPaint paint = null!)
  {
    _font.GetGlyphWidths(glyphs, widths, bounds, paint);
  }

  public SKPath GetGlyphPath (ushort glyph)
  {
    return _font.GetGlyphPath(glyph);
  }

  public void GetGlyphPaths (ReadOnlySpan<ushort> glyphs, SKGlyphPathDelegate glyphPathDelegate)
  {
    _font.GetGlyphPaths(glyphs, glyphPathDelegate);
  }

  public void Dispose ()
  {
    _font?.Dispose();
  }
}
