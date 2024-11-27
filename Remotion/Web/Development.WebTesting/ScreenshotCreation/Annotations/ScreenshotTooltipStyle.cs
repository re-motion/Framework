// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using JetBrains.Annotations;
using Microsoft.Maui.Graphics;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.SystemDrawingImitators;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations
{
  /// <summary>
  /// Describes how a tooltip looks when it is drawn.
  /// </summary>
  public class ScreenshotTooltipStyle
  {
    public static readonly ScreenshotTooltipStyle Chrome = new ScreenshotTooltipStyle(
        new Font("Arial", 9, FontStyle.Regular),
        new SolidBrush(Color.FromArgb(0x57, 0x57, 0x57)),
        Brushes.White,
        new Pen(Color.FromArgb(0x76, 0x76, 0x76), 1),
        TooltipPositioning.BottomRight,
        new WebPadding(2, 0, 2, 0),
        true,
        new Size(12, 18),
        new Size(970, 110));

    public static readonly ScreenshotTooltipStyle Edge = new ScreenshotTooltipStyle(
        new Font("Arial", 9, FontStyle.Regular),
        new SolidBrush(Color.FromArgb(0x57, 0x57, 0x57)),
        Brushes.White,
        new Pen(Color.FromArgb(0x76, 0x76, 0x76), 1),
        TooltipPositioning.BottomRight,
        new WebPadding(2, 0, 2, 0),
        true,
        new Size(12, 18),
        new Size(970, 110));

    public static readonly ScreenshotTooltipStyle Firefox = new ScreenshotTooltipStyle(
        new Font("Sans-Serif", 9, FontStyle.Regular),
        new SolidBrush(Color.FromArgb(0x0, 0x0, 0x0)),
        Brushes.White,
        new Pen(Color.FromArgb(0x76, 0x76, 0x76), 1),
        TooltipPositioning.BottomRight,
        new WebPadding(2, 2, 2, 2),
        true,
        new Size(12, 18),
        new Size(970, 110));

    private readonly Font _font;
    private readonly Brush _foregroundBrush;
    private readonly Brush? _backgroundBrush;
    private readonly Pen _border;
    private readonly TooltipPositioning _positioning;
    private readonly WebPadding _contentPadding;
    private readonly bool _wrapLines;
    private readonly Size _minimumSize;
    private readonly Size _maximumSize;

    public ScreenshotTooltipStyle (
        [NotNull] Font font,
        [NotNull] Brush foregroundBrush,
        [CanBeNull] Brush? backgroundBrush,
        [NotNull] Pen border,
        TooltipPositioning positioning,
        WebPadding contentPadding,
        bool wrapLines,
        Size minimumSize,
        Size maximumSize)
    {
      ArgumentUtility.CheckNotNull("font", font);
      ArgumentUtility.CheckNotNull("foregroundBrush", foregroundBrush);
      ArgumentUtility.CheckNotNull("border", border);

      _font = font;
      _foregroundBrush = foregroundBrush;
      _backgroundBrush = backgroundBrush;
      _border = border;
      _positioning = positioning;
      _contentPadding = contentPadding;
      _wrapLines = wrapLines;
      _minimumSize = minimumSize;
      _maximumSize = maximumSize;
    }

    /// <summary>
    /// The <see cref="System.Drawing.Font"/> that will be used to draw the tooltips content.
    /// </summary>
    public Font Font
    {
      get { return _font; }
    }

    /// <summary>
    /// The <see cref="Brush"/> that will be used to draw the content.
    /// </summary>
    public Brush ForegroundBrush
    {
      get { return _foregroundBrush; }
    }

    /// <summary>
    /// The <see cref="Brush"/> that will be used to fill the background of the tooltip, or <see langword="null" /> if the background should not be filled.
    /// </summary>
    [CanBeNull]
    public Brush? BackgroundBrush
    {
      get { return _backgroundBrush; }
    }

    /// <summary>
    /// The <see cref="Pen"/> that will be used to draw the border of the tooltip or <see langword="null" /> if no border should be drawn.
    /// </summary>
    public Pen Border
    {
      get { return _border; }
    }

    /// <summary>
    /// The <see cref="ContentAlignment"/> which determines where the tooltip will be located (top, center, bottom)
    /// and in which direction it will expand (left, center, right).
    /// </summary>
    public TooltipPositioning Positioning
    {
      get { return _positioning; }
    }

    /// <summary>
    /// The <see cref="WebPadding"/> used to pad the content.
    /// </summary>
    public WebPadding ContentPadding
    {
      get { return _contentPadding; }
    }

    /// <summary>
    /// If <see langword="true" /> lines will be wrapped when they get too long, otherwise <see langword="false" />.
    /// </summary>
    public bool WrapLines
    {
      get { return _wrapLines; }
    }

    /// <summary>
    /// The minimum size of the tooltip can have.
    /// </summary>
    public Size MinimumSize
    {
      get { return _minimumSize; }
    }

    /// <summary>
    /// The maximum size the tooltip can have before wrapping/clipping.
    /// </summary>
    public Size MaximumSize
    {
      get { return _maximumSize; }
    }

    /// <summary>
    /// Clones the <see cref="ScreenshotTooltipStyle"/> overriding the specified members.
    /// </summary>
    public ScreenshotTooltipStyle Clone (
        Font? font = null,
        Brush? foregroundBrush = null,
        OptionalParameter<Brush> backgroundBrush = default(OptionalParameter<Brush>),
        Pen? border = null,
        TooltipPositioning? positioning = null,
        WebPadding? contentPadding = null,
        bool? wrapLines = null,
        Size? minimumSize = null,
        Size? maximumSize = null)
    {
      return new ScreenshotTooltipStyle(
          font ?? _font,
          foregroundBrush ?? _foregroundBrush,
          backgroundBrush.GetValueOrDefault(_backgroundBrush),
          border ?? _border,
          positioning ?? _positioning,
          contentPadding ?? _contentPadding,
          wrapLines ?? _wrapLines,
          minimumSize ?? _minimumSize,
          maximumSize ?? _maximumSize);
    }
  }
}
