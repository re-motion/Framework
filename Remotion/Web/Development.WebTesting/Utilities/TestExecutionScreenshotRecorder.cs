﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using JetBrains.Annotations;
using log4net;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Screenshot = Remotion.Web.Development.WebTesting.ScreenshotCreation.Screenshot;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Captures screenshots of failed tests and saves them under a specified name.
  /// </summary>
  public class TestExecutionScreenshotRecorder
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (TestExecutionScreenshotRecorder));

    private readonly string _outputDirectory;

    private bool _isCursorCaptured;
    private CursorInformation _cursorInformation;

    public TestExecutionScreenshotRecorder ([NotNull] string outputDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("outputDirectory", outputDirectory);

      _outputDirectory = Path.GetFullPath (outputDirectory);
      Directory.CreateDirectory (_outputDirectory);
    }

    /// <summary>
    /// The output directory where taken screenshots will be saved.
    /// </summary>
    public string OutputDirectory
    {
      get { return _outputDirectory; }
    }

    /// <summary>
    /// Captures the current state of the mouse cursor. All subsequent calls to either <see cref="TakeDesktopScreenshot"/>
    /// or <see cref="TakeBrowserScreenshot"/> will use the captured cursor information instead of the current cursor information.
    /// </summary>
    public void CaptureCursor ()
    {
      _cursorInformation = CaptureCursorInformationWithLog();
      _isCursorCaptured = true;
    }

    /// <summary>
    /// Takes a screenshot of the desktop and saves it under the specified <paramref name="testName"/>.
    /// </summary>
    /// <remarks>
    /// Screenshot will be saved under: <c>&lt;testName&gt;.Desktop.png</c>
    /// Any <see cref="Path"/>.<see cref="Path.GetInvalidFileNameChars"/> in the <paramref name="testName"/> will be replaced by "_"
    /// If the full file path would be longer than 260 characters, the <paramref name="testName"/> is
    /// shortened accordingly.
    /// </remarks>
    /// <exception cref="PathTooLongException">
    /// If the resulting file path would be longer than 260 characters (despite shortening of the <paramref name="testName"/>).
    /// </exception>
    public void TakeDesktopScreenshot ([NotNull] string testName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("testName", testName);

      var filePath = ScreenshotRecorderPathUtility.GetFullScreenshotFilePath (_outputDirectory, testName, "Desktop", "png");

      try
      {
        var screenshot = Screenshot.TakeDesktopScreenshot();

        using (var graphics = Graphics.FromImage (screenshot.Image))
        {
          var transformMatrix = new Matrix();
          transformMatrix.Translate (-screenshot.DesktopOffset.Width, -screenshot.DesktopOffset.Height);

          graphics.Transform = transformMatrix;

          GetCursorInformation().Draw (graphics);
        }

        screenshot.Image.Save (filePath, ImageFormat.Png);

        s_log.InfoFormat ("Saved screenshot of desktop to '{0}'.", filePath);
      }
      catch (Exception ex)
      {
        s_log.Error (string.Format ("Could not save desktop screenshot to '{0}'.", filePath), ex);
      }
    }

    /// <summary>
    /// Takes a screenshot of the desktop, crops it to the browser content area and saves it under the specified <paramref name="testName"/>.
    /// </summary>
    /// <remarks>
    /// Screenshot will be saved under: <c>&lt;testName&gt;.Browser&lt;sessionID&gt;-&lt;windowID&gt;.png</c>.
    /// Any <see cref="Path"/>.<see cref="Path.GetInvalidFileNameChars"/> in the <paramref name="testName"/> will be replaced by "_".
    /// If the full file path would be longer than 260 characters, the <paramref name="testName"/> is
    /// shortened accordingly.
    /// </remarks>
    /// <exception cref="PathTooLongException">
    /// If the resulting file path would be longer than 260 characters (despite shortening of the <paramref name="testName"/>).
    /// </exception>
    public void TakeBrowserScreenshot (
        [NotNull] string testName,
        [NotNull] IBrowserSession[] browserSessions,
        [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("testName", testName);
      ArgumentUtility.CheckNotNullOrItemsNull ("browserSessions", browserSessions);
      ArgumentUtility.CheckNotNull ("locator", locator);
      if (browserSessions.Length == 0)
        throw new ArgumentException ("At least one browser session must be specified.", "browserSessions");

      var sessionID = 0;
      foreach (var browserSession in browserSessions)
      {
        SaveBrowserSessionScreenshot (testName, locator, browserSession, sessionID);
        sessionID++;
      }
    }

    private void SaveBrowserSessionScreenshot (string testName, IBrowserContentLocator locator, IBrowserSession browserSession, int sessionID)
    {
      var driver = browserSession.Driver;
      if (driver == null)
        return;

      var nativeDriver = (IWebDriver) driver.Native;
      if (nativeDriver == null)
        return;

      var baseSuffix = string.Concat ("Browser", sessionID);

      var windowID = 0;
      foreach (var windowHandle in nativeDriver.WindowHandles)
      {
        var windowSuffix = string.Concat (baseSuffix, "-", windowID);

        try
        {
          nativeDriver.SwitchTo().Window (windowHandle);

          var screenshot = Screenshot.TakeBrowserScreenshot (browserSession, locator);
          var browserContentBounds = locator.GetBrowserContentBounds (nativeDriver);

          using (var graphics = Graphics.FromImage (screenshot.Image))
          {
            var transformMatrix = new Matrix();
            transformMatrix.Translate (-browserContentBounds.X, -browserContentBounds.Y);

            graphics.Transform = transformMatrix;

            GetCursorInformation().Draw (graphics);
          }

          var filePath = ScreenshotRecorderPathUtility.GetFullScreenshotFilePath (_outputDirectory, testName, windowSuffix, "png");

          screenshot.Image.Save (filePath, ImageFormat.Png);
        }
        catch (Exception ex)
        {
          s_log.Error (string.Format ("Could not save screenshot of browser session window. (window: {0})", windowID), ex);
        }

        windowID++;
      }

      s_log.InfoFormat ("Saved screenshots for the browser session '{0}'.", browserSession.Window.Text);
    }

    private CursorInformation CaptureCursorInformationWithLog ()
    {
      try
      {
        return CursorInformation.Capture();
      }
      catch (Exception ex)
      {
        s_log.ErrorFormat ("Could not capture CursorInformation. Exception: \n{0}", ex);
        return CursorInformation.Empty;
      }
    }

    private CursorInformation GetCursorInformation ()
    {
      if (_isCursorCaptured)
        return _cursorInformation;
      return CaptureCursorInformationWithLog();
    }
  }
}