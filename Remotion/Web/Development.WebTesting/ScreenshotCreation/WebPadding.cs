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
using System.Drawing;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// Represents padding information associated with a web element.
  /// </summary>
  public struct WebPadding : IEquatable<WebPadding>
  {
    /// <summary>
    /// A <see cref="WebPadding"/> where all sides are set to <c>0</c>.
    /// </summary>
    public static readonly WebPadding None = new WebPadding(0);

    /// <summary>
    /// A <see cref="WebPadding"/> where all sides are set to <c>-1</c>.
    /// </summary>
    public static readonly WebPadding Inner = new WebPadding(-1);

    public static bool operator == (WebPadding left, WebPadding right)
    {
      return left.Equals(right);
    }

    public static bool operator != (WebPadding left, WebPadding right)
    {
      return !left.Equals(right);
    }

    private readonly int _left;
    private readonly int _top;
    private readonly int _right;
    private readonly int _bottom;

    public WebPadding (int all)
        : this(all, all, all, all)
    {
    }

    public WebPadding (int left, int top, int right, int bottom)
    {
      _left = left;
      _top = top;
      _right = right;
      _bottom = bottom;
    }

    /// <summary>
    /// The padding value on the bottom side.
    /// </summary>
    public int Bottom
    {
      get { return _bottom; }
    }

    /// <summary>
    /// The padding value on the left side.
    /// </summary>
    public int Left
    {
      get { return _left; }
    }

    /// <summary>
    /// The padding value on the right side.
    /// </summary>
    public int Right
    {
      get { return _right; }
    }

    /// <summary>
    /// The padding value on the top side.
    /// </summary>
    public int Top
    {
      get { return _top; }
    }

    /// <summary>
    /// The padding value <see cref="Left"/> and <see cref="Right"/> combined.
    /// </summary>
    public int Horizontal
    {
      get { return Left + Right; }
    }

    /// <summary>
    /// The padding value <see cref="Top"/> and <see cref="Bottom"/> combined.
    /// </summary>
    public int Vertical
    {
      get { return Top + Bottom; }
    }

    /// <summary>
    /// Applies the padding to the specified <paramref name="target"/>.
    /// </summary>
    [Pure]
    public Rectangle Apply (Rectangle target)
    {
      return new Rectangle(
          target.X - _left,
          target.Y - _top,
          target.Width + Horizontal,
          target.Height + Vertical);
    }

    /// <inheritdoc />
    public bool Equals (WebPadding other)
    {
      return _left == other._left && _top == other._top && _right == other._right && _bottom == other._bottom;
    }

    /// <inheritdoc />
    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      return obj is WebPadding && Equals((WebPadding)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = _left;
        hashCode = (hashCode * 397) ^ _top;
        hashCode = (hashCode * 397) ^ _right;
        hashCode = (hashCode * 397) ^ _bottom;
        return hashCode;
      }
    }
  }
}
