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
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;

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

      if (browserSession.Headless)
        return CreateBrowserScreenshotBasedOnDriver(browserSession);
      else
        return CreateBrowserScreenshotBasedOnScreen(browserSession, locator);
    }

    /// <summary>
    /// Takes a screenshot of all desktops.
    /// </summary>
    [NotNull]
    public static Screenshot TakeDesktopScreenshot ()
    {
      return TakeDesktopScreenshot(Screen.AllScreens);
    }

    /// <summary>
    /// Takes a screenshot of the specified <paramref name="screens"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="screens"/> has no elements.</exception>
    [NotNull]
    public static Screenshot TakeDesktopScreenshot ([NotNull] Screen[] screens)
    {
      ArgumentUtility.CheckNotNull("screens", screens);

      if (screens.Length == 0)
        throw new ArgumentException("At least one screen must be specified in order to take a screenshot.", "screens");

      return CreateDesktopScreenshot(screens);
    }

    /// <summary>
    /// Takes a screenshot of the primary screen.
    /// </summary>
    [NotNull]
    public static Screenshot TakePrimaryDesktopScreenshot ()
    {
      return TakeDesktopScreenshot(new[] { Assertion.IsNotNull(Screen.PrimaryScreen, "Screen.PrimaryScreen may not be null when taking a screen shot.") });
    }

    [NotNull]
    private static Screenshot CreateBrowserScreenshotBasedOnScreen (IBrowserSession browserSession, IBrowserContentLocator locator)
    {
      var browserBounds = locator.GetBrowserContentBounds((IWebDriver)browserSession.Driver.Native);
      var image = new Bitmap(browserBounds.Width, browserBounds.Height);
      using (var graphics = Graphics.FromImage(image))
      {
        graphics.CopyFromScreen(browserBounds.Location, Point.Empty, browserBounds.Size);

        graphics.Flush();
      }

      return new Screenshot(
          image,
          Size.Empty,
          new[] { new Rectangle(Point.Empty, browserBounds.Size) },
          CursorInformation.Capture(),
          CoordinateSystem.Browser);
    }

    [NotNull]
    private static Screenshot CreateBrowserScreenshotBasedOnDriver (IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      var screenshot = ((ITakesScreenshot)browserSession.Driver.Native).GetScreenshot();

      Bitmap image;
      using (var memoryStream = new MemoryStream(screenshot.AsByteArray, false))
      {
        // For some reasons GDI+ might throw an OutOfMemory exception when the image provided by the driver is used
        // As such, we copy the image which is quite fast (~5ms) and prevents any issues down the line
        var corruptedImage = (Bitmap)Image.FromStream(memoryStream);
        image = new Bitmap(corruptedImage);
      }

      return new Screenshot(
          image,
          Size.Empty,
          new[] { new Rectangle(Point.Empty, image.Size) },
          CursorInformation.Empty,
          CoordinateSystem.Browser);
    }

    [NotNull]
    private static Screenshot CreateDesktopScreenshot (Screen[] screens)
    {
      var screenBounds = screens.Select(s => s.Bounds).ToArray();
      var requiredBounds = screenBounds.Aggregate(Rectangle.Union);
      var offset = new Size(-requiredBounds.X, -requiredBounds.Y);

      var image = new Bitmap(requiredBounds.Width, requiredBounds.Height);
      using (var graphics = Graphics.FromImage(image))
      {
        graphics.Clear(Color.Transparent);

        foreach (var screen in screens)
        {
          graphics.CopyFromScreen(screen.Bounds.Location, screen.Bounds.Location + offset, screen.Bounds.Size);
        }

        graphics.Flush();
      }

      return new Screenshot(
          image,
          new Size(requiredBounds.X, requiredBounds.Y),
          screenBounds,
          CursorInformation.Capture(),
          CoordinateSystem.Desktop);
    }

    private readonly CursorInformation _cursorInformation;
    private readonly Size _desktopOffset;
    private readonly Image _image;
    private readonly Rectangle[] _screenshotBounds;
    private readonly CoordinateSystem _coordinateSystem;

    private bool _disposed;

    public Screenshot (
        [NotNull] Image image,
        Size desktopOffset,
        Rectangle[] screenshotBounds,
        [NotNull] CursorInformation cursorInformation,
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
    public CursorInformation CursorInformation
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
