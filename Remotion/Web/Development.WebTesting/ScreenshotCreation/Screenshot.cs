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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Drawing;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// A screenshot which can be annotated by a <see cref="ScreenshotBuilder"/>.
  /// </summary>
  public class Screenshot : IDisposable
  {
    /// <summary>
    /// Takes a screenshot of the specified <paramref name="browserSession"/>.
    /// </summary>
    [NotNull]
    public static Screenshot TakeBrowserScreenshot ([NotNull] IBrowserSession browserSession, [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);
      ArgumentUtility.CheckNotNull("locator", locator);

      return CreateBrowserScreenshotBasedOnDriver(browserSession);
    }

    [NotNull]
    private static Screenshot CreateBrowserScreenshotBasedOnDriver (IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      var screenshot = ((ITakesScreenshot)browserSession.Driver.Native).GetScreenshot();

      Image image;
      using (var memoryStream = new MemoryStream(screenshot.AsByteArray, false))
      {
        image = Image.FromStream(memoryStream);
      }

      return new Screenshot(
          image,
          Size.Empty,
          new[] { new Rectangle(Point.Empty, image.Size) },
          EmptyCursorInformation.Instance,
          CoordinateSystem.Browser);
    }

    private readonly ICursorInformation _cursorInformation;
    private readonly Size _desktopOffset;
    private readonly Image _image;
    private readonly Rectangle[] _screenshotBounds;
    private readonly CoordinateSystem _coordinateSystem;

    private bool _disposed;

    public Screenshot (
        [NotNull] Image image,
        Size desktopOffset,
        Rectangle[] screenshotBounds,
        [NotNull] ICursorInformation cursorInformation,
        CoordinateSystem coordinateSystem)
    {
      ArgumentUtility.CheckNotNull("image", image);
      ArgumentUtility.CheckNotNull("screenshotBounds", screenshotBounds);
      ArgumentUtility.CheckNotNull("cursorInformation", cursorInformation);

      _image = image;
      _desktopOffset = desktopOffset;
      _screenshotBounds = screenshotBounds;
      _cursorInformation = cursorInformation;
      _coordinateSystem = coordinateSystem;
    }

    /// <summary>
    /// The <see cref="CursorInformation"/> associated with the screenshot.
    /// </summary>
    [NotNull]
    public ICursorInformation CursorInformation
    {
      get
      {
        ThrowIfDisposed();

        return _cursorInformation;
      }
    }

    /// <summary>
    /// Desktop coordinates of the upper left corner of the image.
    /// </summary>
    public Size DesktopOffset
    {
      get
      {
        ThrowIfDisposed();

        return _desktopOffset;
      }
    }

    /// <summary>
    /// Returns the screenshot as image.
    /// </summary>
    [NotNull]
    public Image Image
    {
      get
      {
        ThrowIfDisposed();

        return _image;
      }
    }

    /// <summary>
    /// If <see langword="true" /> the screenshot is aligned to desktop and therefore requires the desktop coordinates when drawing, <see langword="false" /> otherwise.
    /// </summary>
    public CoordinateSystem CoordinateSystem
    {
      get
      {
        ThrowIfDisposed();

        return _coordinateSystem;
      }
    }

    /// <summary>
    /// Bounds of all desktops on the screenshot.
    /// </summary>
    [NotNull]
    public Rectangle[] ScreenshotBounds
    {
      get
      {
        ThrowIfDisposed();

        return _screenshotBounds;
      }
    }

    /// <summary>
    /// Checks if <paramref name="target"/> is inside of <see cref="ScreenshotBounds"/>.
    /// </summary>
    public bool Contains (Rectangle target)
    {
      ThrowIfDisposed();

      return _screenshotBounds.Any(t => t.Contains(target));
    }

    public void Dispose ()
    {
      if (!_disposed)
        _image.Dispose();
      _disposed = true;
    }

    private void ThrowIfDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException(GetType().FullName);
    }
  }
}
