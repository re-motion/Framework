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
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Coypu;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utility method to capture screenshots of web test "situations".
  /// </summary>
  public class ScreenshotCapturer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ScreenshotCapturer));
    private static Size s_screenSize;

    static ScreenshotCapturer ()
    {
      DetermineScreenSize();
    }

    private static void DetermineScreenSize ()
    {
      s_screenSize = new Size();

      foreach (var screen in Screen.AllScreens)
      {
        s_screenSize.Width += screen.Bounds.Width;
        if (screen.Bounds.Height > s_screenSize.Height)
          s_screenSize.Height = screen.Bounds.Height;
      }
    }

    private readonly string _screenshotDirectory;

    public ScreenshotCapturer ([NotNull] string screenshotDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("screenshotDirectory", screenshotDirectory);

      _screenshotDirectory = screenshotDirectory;
      EnsureScreenshotDirectoryExists();
    }

    /// <summary>
    /// Takes a screenshot of the whole desktop.
    /// </summary>
    /// <param name="baseFileName">Base file name, will be combined with the screenshot directory, "_Desktop" and the file ending to the final file path.</param>
    public void TakeDesktopScreenshot ([NotNull] string baseFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseFileName", baseFileName);

      var fullFilePath = ScreenshotCapturerFileNameGenerator.GetFullScreenshotFilePath (_screenshotDirectory, baseFileName, "Desktop", "png");
      s_log.InfoFormat ("Saving screenshot of desktop to '{0}'.", fullFilePath);

      try
      {
        // Todo RM-6337: Capture the mouse cursor as well.
        using (var bitmap = new Bitmap (s_screenSize.Width, s_screenSize.Height))
        {
          using (var gfx = Graphics.FromImage (bitmap))
          {
            gfx.CopyFromScreen (0, 0, 0, 0, s_screenSize);
            bitmap.Save (fullFilePath, ImageFormat.Png);
          }
        }
      }
      catch (Exception ex)
      {
        s_log.Error (string.Format ("Could not save screenshot to '{0}'.", fullFilePath), ex);
      }
    }

    /// <summary>
    /// Takes a screenshot of the browser window by using the web driver's screenshot interface.
    /// </summary>
    /// <param name="baseFileName">Base file name, will be combined with the screenshot directory, "_Browser" and the file ending to the final file path.</param>
    /// <param name="browser">The browser session of which the screenshot should be taken.</param>
    public void TakeBrowserScreenshot ([NotNull] string baseFileName, [NotNull] BrowserSession browser)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseFileName", baseFileName);
      ArgumentUtility.CheckNotNull ("browser", browser);

      var fullFilePath = ScreenshotCapturerFileNameGenerator.GetFullScreenshotFilePath (_screenshotDirectory, baseFileName, "Browser", "png");
      s_log.InfoFormat ("Saving screenshot of browser to '{0}'.", fullFilePath);

      try
      {
        browser.SaveScreenshot (fullFilePath, ImageFormat.Png);
      }
      catch (Exception ex)
      {
        s_log.Error (string.Format ("Could not save screenshot to '{0}'.", fullFilePath), ex);
      }
    }

    private void EnsureScreenshotDirectoryExists ()
    {
      Directory.CreateDirectory (_screenshotDirectory);
    }

    private static class ScreenshotCapturerFileNameGenerator
    {
      // Todo RM-6337: Add unit testing!

      /// <summary>
      /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
      /// the file <paramref name="extension"/>. If the full file path would be longer than 260 characters, the <paramref name="baseFileName"/> is
      /// shortened accordingly.
      /// </summary>
      /// <exception cref="PathTooLongException">
      /// If the resulting file path would be longer than 260 characters (despite shortening of the <paramref name="baseFileName"/>).
      /// </exception>
      public static string GetFullScreenshotFilePath (string screenshotDirectory, string baseFileName, string suffix, string extension)
      {
        var sanitizedBaseFileName = SanitizeFileName (baseFileName);
        var filePath = GetFullScreenshotFilePathInternal (screenshotDirectory, sanitizedBaseFileName, suffix, extension);
        if (filePath.Length > 260)
        {
          var shortenedSanitizedBaseFileName = ShortenBaseFileName (filePath, sanitizedBaseFileName);
          filePath = GetFullScreenshotFilePathInternal (screenshotDirectory, shortenedSanitizedBaseFileName, suffix, extension);
        }

        return filePath;
      }

      /// <summary>
      /// Combines the given <paramref name="screenshotDirectory"/> with the <paramref name="baseFileName"/>, the suffix <paramref name="suffix"/> and
      /// the file <paramref name="extension"/>.
      /// </summary>
      private static string GetFullScreenshotFilePathInternal (string screenshotDirectory, string baseFileName, string suffix, string extension)
      {
        var fileName = string.Format ("{0}_{1}.{2}", baseFileName, suffix, extension);
        return Path.Combine (screenshotDirectory, fileName);
      }

      /// <summary>
      /// Removes all invalid file name characters from the given <paramref name="fileName"/>.
      /// </summary>
      private static string SanitizeFileName (string fileName)
      {
        foreach (var c in Path.GetInvalidFileNameChars())
          fileName = fileName.Replace (c, '_');

        return fileName;
      }

      /// <summary>
      /// Reduces the <paramref name="baseFileName"/> length such that the <paramref name="fullFilePath"/> is no longer than 260 characters. Throws an
      /// <see cref="PathTooLongException"/> if the <paramref name="baseFileName"/> would have to be reduced to zero characters.
      /// </summary>
      private static string ShortenBaseFileName (string fullFilePath, string baseFileName)
      {
        var overflow = fullFilePath.Length - 260;
        if (overflow > baseFileName.Length - 1)
        {
          throw new PathTooLongException (
              string.Format ("Could not save screenshot to '{0}', the file path is too long and cannot be reduced to 260 characters.", fullFilePath));
        }

        return baseFileName.Substring (0, baseFileName.Length - overflow);
      }
    }
  }
}