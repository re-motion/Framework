// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.IO;
using System.Threading;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;
using SkiaSharp;

namespace Remotion.Web.Development.WebTesting.Resources;

public static class LiberationsSans
{
  private static readonly Lazy<SKTypeface> s_fontBoldTypeface = new(() => GetResourceBytes("LiberationSans-Bold.ttf"), LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Lazy<SKTypeface> s_fontBoldItalicTypeface = new(() => GetResourceBytes("LiberationSans-BoldItalic.ttf"), LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Lazy<SKTypeface> s_fontItalicTypeface = new(() => GetResourceBytes("LiberationSans-Italic.ttf"), LazyThreadSafetyMode.ExecutionAndPublication);
  private static readonly Lazy<SKTypeface> s_fontRegularTypeface = new(() => GetResourceBytes("LiberationSans-Regular.ttf"), LazyThreadSafetyMode.ExecutionAndPublication);

  public static Font Bold (float size = 12f) => new(new SKFont(s_fontBoldTypeface.Value, size));

  public static Font BoldItalic (float size = 12f) => new(new SKFont(s_fontBoldItalicTypeface.Value, size));

  public static Font Italic (float size = 12f) => new(new SKFont(s_fontItalicTypeface.Value, size));

  public static Font Regular (float size = 12f) => new(new SKFont(s_fontRegularTypeface.Value, size));

  private static SKTypeface GetResourceBytes (string name)
  {
    var fullResourceName = $"{typeof(LiberationsSans).Namespace}.{name}";
    using var stream = typeof(LiberationsSans).Assembly.GetManifestResourceStream(fullResourceName);
    if (stream == null)
      throw new InvalidOperationException($"Cannot find the embedded resource '{fullResourceName}'.");

    // .FromStream(...) will take ownership of the stream so we copy to a MemoryStream first
    var memoryStream = new MemoryStream();
    stream.CopyTo(memoryStream);
    memoryStream.Position = 0;

    var typeFace = SKTypeface.FromStream(memoryStream);
    if (typeFace == null)
      throw new InvalidOperationException($"Embedded resource '{fullResourceName}' is not a valid font file.");

    return typeFace;
  }
}
